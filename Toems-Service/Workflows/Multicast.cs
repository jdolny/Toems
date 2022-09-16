
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class Multicast
    {
        private readonly string _clientCount;
        private readonly ServiceComputer _computerServices;
        private readonly EntityGroup _group;
        private readonly bool _isOnDemand;
        private readonly EntityActiveMulticastSession _multicastSession;
        private readonly int _userId;

        private List<EntityComputer> _computers;
        private ImageProfileWithImage _imageProfile;
        private int? _multicastServerId;
        private readonly int _comServerId;

        //Constructor For Starting Multicast For Group
        public Multicast(int groupId, int userId)
        {
            _computers = new List<EntityComputer>();
            _multicastSession = new EntityActiveMulticastSession();
            _isOnDemand = false;
            _group = new ServiceGroup().GetGroup(groupId);
            _userId = userId;
            _computerServices = new ServiceComputer();
        }

        //Constructor For Starting Multicast For On Demand
        public Multicast(int imageProfileId, string clientCount, string sessionName, int userId, int comServerId)
        {
            _multicastSession = new EntityActiveMulticastSession();
            _isOnDemand = true;
            _imageProfile = new ServiceImageProfile().ReadProfile(imageProfileId);
            _clientCount = clientCount;
            _group = new EntityGroup { ImageProfileId = _imageProfile.Id };
            _userId = userId;
            _multicastSession.ImageProfileId = _imageProfile.Id;
            _computerServices = new ServiceComputer();
            _comServerId = comServerId;
            _multicastSession.Name = sessionName;
        }

        public string Create()
        {
            _imageProfile = new ServiceImageProfile().ReadProfile(_group.ImageProfileId);
            if (_imageProfile == null) return "The Image Profile Does Not Exist";

            if (_imageProfile.Image == null) return "The Image Does Not Exist";  
          
            _multicastServerId = _isOnDemand
                ? _comServerId
                : new GetMulticastServer(_group).Run();

            if (_multicastServerId == null)
                return "Could Not Find Any Available Multicast Servers";


            var comServer = new ServiceClientComServer().GetServer(Convert.ToInt32(_multicastServerId));
            if(string.IsNullOrEmpty(comServer.MulticastInterfaceIp))
            {
                return "The Com Server's Multicast Interface IP Address Is Not Populated";
            }


            _multicastSession.Port = new ServicePort().GetNextPort(_multicastServerId);
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
                var ondUser = new ServiceUser().GetUser(_userId);
                if (ondUser != null)
                    ondAuditLog.UserName = ondUser.Name;
                ondAuditLog.ObjectName = _imageProfile.Image.Name;
                ondAuditLog.UserId = _userId;
                ondAuditLog.ObjectType = "Image";
                ondAuditLog.ObjectJson = JsonConvert.SerializeObject(_multicastSession);
                new ServiceAuditLog().AddAuditLog(ondAuditLog);
                return "Successfully Started Multicast " + _group.Name;
            }
            //End of the road for starting an on demand multicast


            //Continue On If multicast is for a group
            _multicastSession.Name = _group.Name;
            _computers = new ServiceGroup().GetGroupMembers(_group.Id);
            if (_computers.Count < 1)
            {
                return "The Group Does Not Have Any Members";
            }

            var activeMulticastSessionServices = new ServiceActiveMulticastSession();
            if (!activeMulticastSessionServices.AddActiveMulticastSession(_multicastSession))
            {
                return "Could Not Create Multicast Database Task.  An Existing Task May Be Running.";
            }

            if (!CreateComputerTasks())
            {
                activeMulticastSessionServices.Delete(_multicastSession.Id);
                return "Could Not Create Computer Database Tasks.  A Computer May Have An Existing Task.";
            }

            if (!CreatePxeFiles())
            {
                activeMulticastSessionServices.Delete(_multicastSession.Id);
                return "Could Not Create Computer Boot Files";
            }

            if (!CreateTaskArguments())
            {
                activeMulticastSessionServices.Delete(_multicastSession.Id);
                return "Could Not Create Computer Task Arguments";
            }

            var processArguments = GenerateProcessArguments();
            if (processArguments == 0)
            {
                activeMulticastSessionServices.Delete(_multicastSession.Id);
                return "Could Not Start The Multicast Application";
            }

            foreach (var computer in _computers)
                _computerServices.Wakeup(computer.Id);

            var auditLog = new EntityAuditLog();
            auditLog.AuditType = EnumAuditEntry.AuditType.Multicast;
            auditLog.ObjectId = _group.Id;
            var user = new ServiceUser().GetUser(_userId);
            if (user != null)
                auditLog.UserName = user.Name;
            auditLog.ObjectName = _group.Name;
            auditLog.UserId = _userId;
            auditLog.ObjectType = "Group";
            auditLog.ObjectJson = JsonConvert.SerializeObject(_multicastSession);
            new ServiceAuditLog().AddAuditLog(auditLog);

            auditLog.ObjectId = _imageProfile.ImageId;
            auditLog.ObjectName = _imageProfile.Image.Name;
            auditLog.ObjectType = "Image";
            new ServiceAuditLog().AddAuditLog(auditLog);

            return "Successfully Started Multicast " + _group.Name;
        }

        private bool CreateComputerTasks()
        {
            var error = false;
            var activeTaskIds = new List<int>();
            var activeImagingTaskServices = new ServiceActiveImagingTask();
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
                    ImageProfileId = _imageProfile.Id
                };

                if (activeImagingTaskServices.AddActiveImagingTask(activeTask))
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
                    activeImagingTaskServices.DeleteActiveImagingTask(taskId);

                return false;
            }
            return true;
        }

        private bool CreatePxeFiles()
        {
            foreach (var computer in _computers)
            {
                if (!new TaskBootMenu().RunAllServers(computer, _imageProfile))
                    return false;
            }
            return true;
        }

        private bool CreateTaskArguments()
        {
            foreach (var computer in _computers)
            {
                var activeTask =  _computerServices.GetTaskForComputer(computer.Id);
                activeTask.Arguments =
                    new CreateTaskArguments(computer, _imageProfile, "multicast",_multicastSession.ComServerId).Execute(
                        _multicastSession.Port.ToString());
                if (!new ServiceActiveImagingTask().UpdateActiveImagingTask(activeTask))
                    return false;
            }
            return true;
        }

        private int GenerateProcessArguments()
        {
            var multicastArgs = new DtoMulticastArgs();
            multicastArgs.schema = new ServiceClientPartition(_imageProfile).GetImageSchema();
            multicastArgs.Environment = _imageProfile.Image.Environment;
            multicastArgs.ImageName = _imageProfile.Image.Name;
            multicastArgs.Port = _multicastSession.Port.ToString();

            var comServer = new ServiceClientComServer().GetServer(_multicastSession.ComServerId);

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

            
            var pid = new MulticastArguments().RunOnComServer(multicastArgs,comServer);
          

            if (pid == 0) return pid;

            var activeMulticastSessionServices = new ServiceActiveMulticastSession();
            if (_isOnDemand)
            {
                _multicastSession.Pid = pid;
                _multicastSession.Name = _group.Name;
                activeMulticastSessionServices.AddActiveMulticastSession(_multicastSession);
            }
            else
            {
                _multicastSession.Pid = pid;
                activeMulticastSessionServices.UpdateActiveMulticastSession(_multicastSession);
            }

            return pid;
        }
    }
}
