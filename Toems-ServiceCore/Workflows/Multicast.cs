
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class Multicast(InfrastructureContext ictx, GroupService serviceGroup, ServiceImageProfile serviceImageProfile,
        ServiceClientComServer serviceClientComServer, ServicePort servicePort, ServiceActiveMulticastSession serviceActiveMulticastSession, 
        ServiceActiveImagingTask serviceActiveImagingTask, ServiceClientPartition serviceClientPartition, ServiceUser serviceUser, CreateTaskArguments createTaskArguments,
        GetMulticastServer getMulticastServer, MulticastArguments multicastArguments, TaskBootMenu taskBootMenu)
    {
        private string _clientCount;
        private ServiceComputer _computerServices;
        private EntityGroup _group;
        private bool _isOnDemand;
        private EntityActiveMulticastSession _multicastSession;
        private int _userId;

        private List<EntityComputer> _computers;
        private ImageProfileWithImage _imageProfile;
        private int? _multicastServerId;
        private int _comServerId;

        //Constructor For Starting Multicast For Group
        public void Init(int groupId, int userId)
        {
            _computers = new List<EntityComputer>();
            _multicastSession = new EntityActiveMulticastSession();
            _isOnDemand = false;
            _group = serviceGroup.GetGroup(groupId);
            _userId = userId;
        }

        //Constructor For Starting Multicast For On Demand
        public void InitOnDemand(int imageProfileId, string clientCount, string sessionName, int userId, int comServerId)
        {
            _multicastSession = new EntityActiveMulticastSession();
            _isOnDemand = true;
            _imageProfile = serviceImageProfile.ReadProfile(imageProfileId);
            _clientCount = clientCount;
            _group = new EntityGroup { ImageProfileId = _imageProfile.Id };
            _userId = userId;
            _multicastSession.ImageProfileId = _imageProfile.Id;
            _comServerId = comServerId;
            _multicastSession.Name = sessionName;
        }

        public string Create()
        {
            _imageProfile = serviceImageProfile.ReadProfile(_group.ImageProfileId);
            if (_imageProfile == null) return "The Image Profile Does Not Exist";

            if (_imageProfile.Image == null) return "The Image Does Not Exist";

            _multicastServerId = _isOnDemand
                ? _comServerId
                : getMulticastServer.Run(_group);

            if (_multicastServerId == null)
                return "Could Not Find Any Available Multicast Servers";


            var comServer = serviceClientComServer.GetServer(Convert.ToInt32(_multicastServerId));
            if(string.IsNullOrEmpty(comServer.MulticastInterfaceIp))
            {
                return "The Com Server's Multicast Interface IP Address Is Not Populated";
            }


            _multicastSession.Port = servicePort.GetNextPort(_multicastServerId);
            if (_multicastSession.Port == 0)
            {
                return "Could Not Determine Current Port Base";
            }

            _multicastSession.ComServerId = Convert.ToInt32(_multicastServerId);
            _multicastSession.UserId = _userId;
           
            if (_isOnDemand)
            {
                if (string.IsNullOrEmpty(_multicastSession.Name))
                    _multicastSession.Name = _multicastSession.Port.ToString();

                if (string.IsNullOrEmpty(_multicastSession.Name) || !_multicastSession.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
                    return "Multicast Session Name Is Not Valid";

                _group.Name = _multicastSession.Name;
                var onDemandprocessArguments = GenerateProcessArguments();
                if (onDemandprocessArguments == 0)
                    return "Could Not Start The Multicast Application";

                var ondAuditLog = new EntityAuditLog();
                ondAuditLog.AuditType = EnumAuditEntry.AuditType.OnDemandMulticast;
                ondAuditLog.ObjectId = _imageProfile.ImageId;
                var ondUser = serviceUser.GetUser(_userId);
                if (ondUser != null)
                    ondAuditLog.UserName = ondUser.Name;
                ondAuditLog.ObjectName = _imageProfile.Image.Name;
                ondAuditLog.UserId = _userId;
                ondAuditLog.ObjectType = "Image";
                ondAuditLog.ObjectJson = JsonConvert.SerializeObject(_multicastSession);
                ictx.AuditLog.AddAuditLog(ondAuditLog);
                return "Successfully Started Multicast " + _group.Name;
            }
            //End of the road for starting an on demand multicast


            //Continue On If multicast is for a group
            _multicastSession.Name = _group.Name;
            _computers = serviceGroup.GetGroupMembers(_group.Id);
            if (_computers.Count < 1)
            {
                return "The Group Does Not Have Any Members";
            }


            if (!serviceActiveMulticastSession.AddActiveMulticastSession(_multicastSession))
            {
                return "Could Not Create Multicast Database Task.  An Existing Task May Be Running.";
            }

            if (!CreateComputerTasks())
            {
                serviceActiveMulticastSession.Delete(_multicastSession.Id);
                return "Could Not Create Computer Database Tasks.  A Computer May Have An Existing Task.";
            }

            if (!CreatePxeFiles())
            {
                serviceActiveMulticastSession.Delete(_multicastSession.Id);
                return "Could Not Create Computer Boot Files";
            }

            if (!CreateTaskArguments())
            {
                serviceActiveMulticastSession.Delete(_multicastSession.Id);
                return "Could Not Create Computer Task Arguments";
            }

            var processArguments = GenerateProcessArguments();
            if (processArguments == 0)
            {
                serviceActiveMulticastSession.Delete(_multicastSession.Id);
                return "Could Not Start The Multicast Application";
            }

            foreach (var computer in _computers)
                _computerServices.Wakeup(computer.Id);

            var auditLog = new EntityAuditLog();
            auditLog.AuditType = EnumAuditEntry.AuditType.Multicast;
            auditLog.ObjectId = _group.Id;
            var user = serviceUser.GetUser(_userId);
            if (user != null)
                auditLog.UserName = user.Name;
            auditLog.ObjectName = _group.Name;
            auditLog.UserId = _userId;
            auditLog.ObjectType = "Group";
            auditLog.ObjectJson = JsonConvert.SerializeObject(_multicastSession);
            ictx.AuditLog.AddAuditLog(auditLog);

            auditLog.ObjectId = _imageProfile.ImageId;
            auditLog.ObjectName = _imageProfile.Image.Name;
            auditLog.ObjectType = "Image";
            ictx.AuditLog.AddAuditLog(auditLog);

            return "Successfully Started Multicast " + _group.Name;
        }

        private bool CreateComputerTasks()
        {
            var error = false;
            var activeTaskIds = new List<int>();

            foreach (var computer in _computers)
            {
                if (_computerServices.IsComputerActive(computer.Id)) return false;
                var activeTask = new EntityActiveImagingTask
                {
                    Type = "multicast",
                    ComputerId = computer.Id,
                    Direction = "deploy",
                    MulticastId = _multicastSession.Id,
                    UserId = _userId,
                    ImageProfileId = _imageProfile.Id,
                    WebTaskToken = Guid.NewGuid().ToString("N").ToUpper()
                };

                if (serviceActiveImagingTask.AddActiveImagingTask(activeTask))
                {
                    activeTaskIds.Add(activeTask.Id);
                }
                else
                {
                    error = true;
                    break;
                }
            }
            if (error)
            {
                foreach (var taskId in activeTaskIds)
                    serviceActiveImagingTask.DeleteActiveImagingTask(taskId);

                return false;
            }
            return true;
        }

        private bool CreatePxeFiles()
        {
            foreach (var computer in _computers)
            {
                if (!taskBootMenu.RunAllServers(computer, _imageProfile))
                    return false;
            }
            return true;
        }

        private bool CreateTaskArguments()
        {
            foreach (var computer in _computers)
            {
                var activeTask =  _computerServices.GetTaskForComputer(computer.Id);
                createTaskArguments.InitMulticast(computer, _imageProfile, "multicast", _multicastSession.ComServerId);
                activeTask.Arguments = createTaskArguments.Execute(_multicastSession.Port.ToString());
                if (!serviceActiveImagingTask.UpdateActiveImagingTask(activeTask))
                    return false;
            }
            return true;
        }

        private int GenerateProcessArguments()
        {
            var multicastArgs = new DtoMulticastArgs();
            serviceClientPartition.SetImageSchema(_imageProfile);
            multicastArgs.schema = serviceClientPartition.GetImageSchema();
            multicastArgs.Environment = _imageProfile.Image.Environment;
            multicastArgs.ImageName = _imageProfile.Image.Name;
            multicastArgs.Port = _multicastSession.Port.ToString();

            var comServer = serviceClientComServer.GetServer(_multicastSession.ComServerId);

            if (_isOnDemand)
            {
                multicastArgs.ExtraArgs = string.IsNullOrEmpty(_imageProfile.SenderArguments)
                    ? comServer.MulticastSenderArguments
                    : _imageProfile.SenderArguments;
                if (!string.IsNullOrEmpty(_clientCount))
                    multicastArgs.clientCount = _clientCount;
            }
            else
            {
                multicastArgs.ExtraArgs = string.IsNullOrEmpty(_imageProfile.SenderArguments)
                    ? comServer.MulticastSenderArguments
                    : _imageProfile.SenderArguments;
                multicastArgs.clientCount = _computers.Count.ToString();
            }

            if (_multicastServerId == null)
                return 0;

            
            var pid = multicastArguments.RunOnComServer(multicastArgs,comServer);
          

            if (pid == 0) return pid;
            
            if (_isOnDemand)
            {
                _multicastSession.Pid = pid;
                _multicastSession.Name = _group.Name;
                serviceActiveMulticastSession.AddActiveMulticastSession(_multicastSession);
            }
            else
            {
                _multicastSession.Pid = pid;
                serviceActiveMulticastSession.UpdateActiveMulticastSession(_multicastSession);
            }

            return pid;
        }
    }
}
