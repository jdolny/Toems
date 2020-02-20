using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Workflows;

namespace Toems_Service.Entity
{
    public class ServiceActiveImagingTask
    {
        private readonly UnitOfWork _uow;
        private readonly ServiceUser _userServices;
        private readonly ILog log = LogManager.GetLogger(typeof(ServiceActiveImagingTask));

        public ServiceActiveImagingTask()
        {
            _uow = new UnitOfWork();
            _userServices = new ServiceUser();
        }

        public string ActiveCountNotOwnedByuser(int userId)
        {
            return _userServices.IsAdmin(userId)
                ? "0"
                : _uow.ActiveImagingTaskRepository.Count(x => x.UserId != userId);
        }

        public string ActiveUnicastCount(int userId, string taskType = "")
        {
           
            return _userServices.IsAdmin(userId)
                ? _uow.ActiveImagingTaskRepository.Count(t => t.Type == "deploy" || t.Type == "upload")
                : _uow.ActiveImagingTaskRepository.Count(
                    t => (t.Type == "deploy" || t.Type == "upload") && t.UserId == userId);
        }

        public bool AddActiveImagingTask(EntityActiveImagingTask activeImagingTask)
        {
            _uow.ActiveImagingTaskRepository.Insert(activeImagingTask);
            _uow.Save();
            return true;
        }

        public string AllActiveCount(int userId)
        {
            return _userServices.IsAdmin(userId)
                ? _uow.ActiveImagingTaskRepository.Count()
                : _uow.ActiveImagingTaskRepository.Count(x => x.UserId == userId);
        }

        public int AllActiveCountAdmin()
        {
            return Convert.ToInt32(_uow.ActiveImagingTaskRepository.Count());
        }

        public void CancelTimedOutTasks()
        {
            var timeout = ServiceSetting.GetSettingValue(SettingStrings.TaskTimeout);
            if (string.IsNullOrEmpty(timeout)) return;
            if (timeout == "0") return;
            var tasks = GetAll();

            foreach (var task in tasks.Where(task => task.Status == EnumTaskStatus.ImagingStatus.Imaging && !task.Type.ToLower().Contains("upload")))
            {
                if (DateTime.Now > task.LastUpdateTime.AddMinutes(Convert.ToInt32(timeout)))
                {
                    DeleteActiveImagingTask(task.Id);
                    log.Debug("Task Timeout Hit. Task " + task.Id + "Cancelled.  Computer Id " + task.ComputerId);
                }
            }
        }

        public DtoActionResult DeleteActiveImagingTask(int activeImagingTaskId)
        {
            var activeImagingTask = _uow.ActiveImagingTaskRepository.GetById(activeImagingTaskId);
            if (activeImagingTask == null) return new DtoActionResult { ErrorMessage = "Task Not Found", Id = 0 };
            var computer = _uow.ComputerRepository.GetById(activeImagingTask.ComputerId);

            _uow.ActiveImagingTaskRepository.Delete(activeImagingTask.Id);
            _uow.Save();

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = activeImagingTaskId;

            if (computer != null)
                new CleanTaskBootFiles().RunAllServers(computer);

            return actionResult;
        }

        public void DeleteAll()
        {
            _uow.ActiveImagingTaskRepository.DeleteRange();
            _uow.Save();
        }

        public void DeleteForMulticast(int multicastId)
        {
            _uow.ActiveImagingTaskRepository.DeleteRange(t => t.MulticastId == multicastId);
            _uow.Save();
        }

        public DtoActionResult DeleteUnregisteredOndTask(int activeImagingTaskId)
        {
            var activeImagingTask = _uow.ActiveImagingTaskRepository.GetById(activeImagingTaskId);
            if (activeImagingTask == null) return new DtoActionResult { ErrorMessage = "Task Not Found", Id = 0 };
            if (activeImagingTask.ComputerId > -1)
                return new DtoActionResult
                {
                    ErrorMessage = "This Task Is Not An On Demand Task And Cannot Be Cancelled",
                    Id = 0
                };

            _uow.ActiveImagingTaskRepository.Delete(activeImagingTask.Id);
            _uow.Save();

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = activeImagingTaskId;

            return actionResult;
        }

        public List<EntityActiveImagingTask> GetAll()
        {
            return _uow.ActiveImagingTaskRepository.Get();
        }

        public List<EntityActiveImagingTask> GetAllOnDemandUnregistered()
        {
            return _uow.ActiveImagingTaskRepository.Get(x => x.ComputerId < -1);
        }

        public int GetCurrentQueue(EntityActiveImagingTask activeTask)
        {
            return
                Convert.ToInt32(
                    _uow.ActiveImagingTaskRepository.Count(
                        x => x.Status == EnumTaskStatus.ImagingStatus.Imaging && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId));
        }

        public EntityActiveImagingTask GetLastQueuedTask(EntityActiveImagingTask activeTask)
        {
            return
                _uow.ActiveImagingTaskRepository.Get(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId,
                    q => q.OrderByDescending(t => t.QueuePosition)).FirstOrDefault();
        }

        public List<EntityComputer> GetMulticastComputers(int multicastId)
        {
            return _uow.ActiveImagingTaskRepository.MulticastComputers(multicastId);
        }

        public EntityActiveImagingTask GetNextComputerInQueue(EntityActiveImagingTask activeTask)
        {
            return
                _uow.ActiveImagingTaskRepository.Get(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId,
                    q => q.OrderBy(t => t.QueuePosition)).FirstOrDefault();
        }

        public string GetQueuePosition(EntityActiveImagingTask task)
        {
            return
                _uow.ActiveImagingTaskRepository.Count(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.QueuePosition < task.QueuePosition);
        }

        public EntityActiveImagingTask GetTask(int taskId)
        {
            return _uow.ActiveImagingTaskRepository.GetById(taskId);
        }

        public List<TaskWithComputer> MulticastMemberStatus(int multicastId)
        {
            return _uow.ActiveImagingTaskRepository.GetMulticastMembers(multicastId);
        }

        public List<EntityActiveImagingTask> MulticastProgress(int multicastId)
        {
            return _uow.ActiveImagingTaskRepository.MulticastProgress(multicastId);
        }

        public int OnDemandCount()
        {
            return Convert.ToInt32(_uow.ActiveImagingTaskRepository.Count(x => x.ComputerId < -1));
        }

        public List<TaskWithComputer> ReadAll(int userId)
        {
            //Admins see all tasks
            return _userServices.IsAdmin(userId)
                ? _uow.ActiveImagingTaskRepository.GetAllTaskWithComputersForAdmin()
                : _uow.ActiveImagingTaskRepository.GetAllTaskWithComputers(userId);
        }

    

        public List<TaskWithComputer> ReadUnicasts(int userId)
        {
            //Admins see all tasks
            var activeImagingTasks = _userServices.IsAdmin(userId)
                ? _uow.ActiveImagingTaskRepository.GetUnicastsWithComputersForAdmin()
                : _uow.ActiveImagingTaskRepository.GetUnicastsWithComputers(userId);

            return activeImagingTasks;
        }

        public void SendTaskCompletedEmail(EntityActiveImagingTask task)
        {
            //Mail not enabled
            if (ServiceSetting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;
            var computer = new ServiceComputer().GetComputer(task.ComputerId);
            if (computer == null) return;
            foreach (
                var user in
                    _userServices.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = new ServiceUser().GetUserRights(user.Id).Select(right => right.Right).ToList();
                if (rights.Any(right => right == AuthorizationStrings.EmailImagingTaskCompleted))
                {
                    if (task.UserId == user.Id)
                    {
                        var mail = new MailServices
                        {
                            MailTo = user.Email,
                            Body = computer.Name + " Image Task Has Completed.",
                            Subject = "Task Completed"
                        };
                        mail.Send();
                    }
                }
            }
        }

        public void SendTaskErrorEmail(EntityActiveImagingTask task, string error)
        {
            //Mail not enabled
            if (ServiceSetting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;
            var computer = new ServiceComputer().GetComputer(task.ComputerId);
            foreach (
                var user in
                    _userServices.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = new ServiceUser().GetUserRights(user.Id).Select(right => right.Right).ToList();
                if (rights.Any(right => right == AuthorizationStrings.EmailImagingTaskFailed))
                {
                    if (task.UserId == user.Id)
                    {
                        if (computer == null)
                        {
                            computer = new EntityComputer();
                            computer.Name = "Unknown Computer";
                        }
                        var mail = new MailServices
                        {
                            MailTo = user.Email,
                            Body = computer.Name + " Image Task Has Failed. " + error,
                            Subject = "Task Failed"
                        };
                        mail.Send();
                    }
                }
            }
        }

        public bool UpdateActiveImagingTask(EntityActiveImagingTask activeImagingTask)
        {
            _uow.ActiveImagingTaskRepository.Update(activeImagingTask, activeImagingTask.Id);
            _uow.Save();
            return true;
        }

    }
}
