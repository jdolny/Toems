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

namespace Toems_Service.Workflows
{
    public class Unicast
    {
        private readonly EntityComputer _computer;
        private readonly string _direction;
        private readonly int _userId;
        private EntityActiveImagingTask _activeTask;
        private ImageProfileWithImage _imageProfile;
        private readonly EntityGroup _group;
        private readonly UnitOfWork _uow;

        public Unicast(int computerId, string direction, int userId)
        {
            _direction = direction;
            _computer = new ServiceComputer().GetComputer(computerId);
            _userId = userId;
            _uow = new UnitOfWork();
        }

        public Unicast(int computerId, string direction, int userId, int groupId)
        {
            _direction = direction;
            _computer = new ServiceComputer().GetComputer(computerId);
            _group = new ServiceGroup().GetGroup(groupId);
            _userId = userId;
            _uow = new UnitOfWork();
        }

        public string Start()
        {
            if (_computer == null)
                return "The Computer Does Not Exist";

            if (_group != null)
            {
                //unicast started via group, use that groups assigned image
                _imageProfile = new ServiceImageProfile().ReadProfile(_group.ImageProfileId);
                if (_imageProfile == null) return "The Image Profile Doesn't Exist";
            }
            else
            {
                _imageProfile = new ServiceComputer().GetEffectiveImage(_computer.Id);
            }

            if(_imageProfile == null)
            {
                return "No Image Has Been Selected";
            }

            if (_imageProfile.Image == null) return "The Image Does Not Exist";

            if (new ServiceComputer().IsComputerActive(_computer.Id))
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

            var activeImagingTaskServices = new ServiceActiveImagingTask();

            if (!activeImagingTaskServices.AddActiveImagingTask(_activeTask))
                return "Could Not Create The Database Entry For This Task";

            if (!new TaskBootMenu().RunAllServers(_computer, _imageProfile))
            {
                activeImagingTaskServices.DeleteActiveImagingTask(_activeTask.Id);
                return "Could Not Create PXE Boot File";
            }

            _activeTask.Arguments = new CreateTaskArguments(_computer, _imageProfile, _direction).Execute();
            if (!activeImagingTaskServices.UpdateActiveImagingTask(_activeTask))
            {
                activeImagingTaskServices.DeleteActiveImagingTask(_activeTask.Id);
                return "Could Not Create Task Arguments";
            }

            new ServiceComputer().Wakeup(_computer.Id);

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
            var user = new ServiceUser().GetUser(_userId);
            if (user != null)
                auditLog.UserName = user.Name;
            auditLog.ObjectName = _computer.Name;
            auditLog.UserId = _userId;
            auditLog.ObjectType = "Computer";
            auditLog.ObjectJson = JsonConvert.SerializeObject(_activeTask);
            new ServiceAuditLog().AddAuditLog(auditLog);

            auditLog.ObjectId = _imageProfile.ImageId;
            auditLog.ObjectName = _imageProfile.Image.Name;
            auditLog.ObjectType = "Image";
            new ServiceAuditLog().AddAuditLog(auditLog);

            return "Successfully Started Task For " + _computer.Name;
        }
    }
}
