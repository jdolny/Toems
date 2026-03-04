using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class Unicast(InfrastructureContext ictx, ServiceComputer serviceComputer, GroupService groupService, ServiceImageProfile serviceImageProfile,
        ServiceUser serviceUser, TaskBootMenu taskBootMenu, CreateTaskArguments createTaskArguments, ServiceActiveImagingTask serviceActiveImagingTask)
    {
        private EntityComputer _computer;
        private string _direction;
        private int _userId;
        private EntityActiveImagingTask _activeTask;
        private ImageProfileWithImage _imageProfile;
        private EntityGroup _group;
        private UnitOfWork _uow = new();

        public void InitSingle(int computerId, string direction, int userId)
        {
            _direction = direction;
            _computer = serviceComputer.GetComputer(computerId);
            _userId = userId;

        }

        public void InitGroup(int computerId, string direction, int userId, int groupId)
        {
            _direction = direction;
            _computer = serviceComputer.GetComputer(computerId);
            _group = groupService.GetGroup(groupId);
            _userId = userId;
        }

        public string Start()
        {
            if (_computer == null)
                return "The Computer Does Not Exist";

            if (_group != null)
            {
                //unicast started via group, use that groups assigned image
                _imageProfile = serviceImageProfile.ReadProfile(_group.ImageProfileId);
                if (_imageProfile == null) return "The Image Profile Doesn't Exist";
            }
            else
            {
                _imageProfile = serviceComputer.GetEffectiveImage(_computer.Id);
            }

            if(_imageProfile == null)
            {
                return "No Image Has Been Selected";
            }

            if (_imageProfile.Image == null) return "The Image Does Not Exist";

            if (serviceComputer.IsComputerActive(_computer.Id))
                return "This Computer Is Already Part Of An Active Task";

            _activeTask = new EntityActiveImagingTask
            {
                ComputerId = _computer.Id,
                Direction = _direction,
                UserId = _userId,
                ImageProfileId = _imageProfile.Id,
                WebTaskToken = Guid.NewGuid().ToString("N").ToUpper()
            };

            _activeTask.Type = _direction;


            if (!serviceActiveImagingTask.AddActiveImagingTask(_activeTask))
                return "Could Not Create The Database Entry For This Task";

            if (!taskBootMenu.RunAllServers(_computer, _imageProfile))
            {
                serviceActiveImagingTask.DeleteActiveImagingTask(_activeTask.Id);
                return "Could Not Create PXE Boot File";
            }

            createTaskArguments.InitUnicast(_computer, _imageProfile, _direction);
            _activeTask.Arguments = createTaskArguments.Execute();
            if (!serviceActiveImagingTask.UpdateActiveImagingTask(_activeTask))
            {
                serviceActiveImagingTask.DeleteActiveImagingTask(_activeTask.Id);
                return "Could Not Create Task Arguments";
            }

            serviceComputer.Wakeup(_computer.Id);

            var auditLog = new EntityAuditLog();
            switch (_direction)
            {
                case "deploy":
                    auditLog.AuditType = EnumAuditEntry.AuditType.Deploy;
                    break;
                default:
                    auditLog.AuditType = EnumAuditEntry.AuditType.Upload;
                    break;
            }

            auditLog.ObjectId = _computer.Id;
            var user = serviceUser.GetUser(_userId);
            if (user != null)
                auditLog.UserName = user.Name;
            auditLog.ObjectName = _computer.Name;
            auditLog.UserId = _userId;
            auditLog.ObjectType = "Computer";
            auditLog.ObjectJson = JsonConvert.SerializeObject(_activeTask);
            ictx.AuditLog.AddAuditLog(auditLog);

            auditLog.ObjectId = _imageProfile.ImageId;
            auditLog.ObjectName = _imageProfile.Image.Name;
            auditLog.ObjectType = "Image";
            ictx.AuditLog.AddAuditLog(auditLog);

            return "Successfully Started Task For " + _computer.Name;
        }
    }
}
