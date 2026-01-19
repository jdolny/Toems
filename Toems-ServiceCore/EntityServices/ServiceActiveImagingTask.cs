using System.Diagnostics;
using System.Management;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service;
using Toems_Service.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveImagingTask(EntityContext ectx, ServiceUser userService)
    {
        public string ActiveCountNotOwnedByuser(int userId)
        {
            return userService.IsAdmin(userId)
                ? "0"
                : ectx.Uow.ActiveImagingTaskRepository.Count(x => x.UserId != userId);
        }

        public string ActiveUnicastCount(int userId, string taskType = "")
        {
           
            return userService.IsAdmin(userId)
                ? ectx.Uow.ActiveImagingTaskRepository.Count(t => t.Type == "deploy" || t.Type == "upload")
                : ectx.Uow.ActiveImagingTaskRepository.Count(
                    t => (t.Type == "deploy" || t.Type == "upload") && t.UserId == userId);
        }

        public bool AddActiveImagingTask(EntityActiveImagingTask activeImagingTask)
        {
            ectx.Uow.ActiveImagingTaskRepository.Insert(activeImagingTask);
            ectx.Uow.Save();
            return true;
        }

        public string AllActiveCount(int userId)
        {
            return userService.IsAdmin(userId)
                ? ectx.Uow.ActiveImagingTaskRepository.Count()
                : ectx.Uow.ActiveImagingTaskRepository.Count(x => x.UserId == userId);
        }

        public int AllActiveCountAdmin()
        {
            return Convert.ToInt32(ectx.Uow.ActiveImagingTaskRepository.Count());
        }

        public void CancelTimedOutTasks()
        {
            var timeout = ectx.Settings.GetSettingValue(SettingStrings.ImageTaskTimeoutMinutes);
            if (string.IsNullOrEmpty(timeout)) return;
            if (timeout == "0") return;
            var tasks = GetAll();

            foreach (var task in tasks)
            {
                if (DateTime.UtcNow > task.LastUpdateTimeUTC.AddMinutes(Convert.ToInt32(timeout)))
                {
                    DeleteActiveImagingTask(task.Id);
                    ectx.Log.Debug("Task Timeout Hit. Task " + task.Id + "Cancelled.  Computer Id " + task.ComputerId);
                }
            }
        }

        public DtoActionResult DeleteActiveImagingTask(int activeImagingTaskId)
        {
            var activeImagingTask = ectx.Uow.ActiveImagingTaskRepository.GetById(activeImagingTaskId);
            if (activeImagingTask == null) return new DtoActionResult { ErrorMessage = "Task Not Found", Id = 0 };
            var computer = ectx.Uow.ComputerRepository.GetById(activeImagingTask.ComputerId);

            ectx.Uow.ActiveImagingTaskRepository.Delete(activeImagingTask.Id);
            ectx.Uow.Save();

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = activeImagingTaskId;

            if (computer != null)
                new CleanTaskBootFiles().RunAllServers(computer);

            if(activeImagingTask.Type.Contains("upload"))
            {
                var comServer = ectx.Uow.ClientComServerRepository.GetById(activeImagingTask.ComServerId);
                if (comServer == null)
                    return actionResult;

                var receiverPids = ectx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == activeImagingTask.Id).Select(x => x.Pid).ToList();
                ectx.Uow.ActiveMulticastSessionRepository.DeleteRange(x => x.UploadTaskId == activeImagingTask.Id);
                ectx.Uow.Save();
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);

                new APICall().ClientComServerApi.KillUdpReceiver(comServer.Url, "", decryptedKey, receiverPids);              
            }

            return actionResult;
        }

        public bool KillUdpReceiver(List<int> pids)
        {
            foreach(var pid in pids)
            {
                try
                {
                    var prs = Process.GetProcessById(Convert.ToInt32(pid));
                    var processName = prs.ProcessName;

                    if (processName == "cmd")
                        KillProcess(Convert.ToInt32(pid));
                }
                catch
                {
                    //ignored

                }
            }
            return true;
        }

        private static void KillProcess(int pid)
        {
            var searcher =
                new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            var moc = searcher.Get();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                KillProcess(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                var proc = Process.GetProcessById(Convert.ToInt32(pid));
                proc.Kill();
            }
            catch
            {
                //ignored
            }
        }

        public void DeleteAll()
        {
            ectx.Uow.ActiveImagingTaskRepository.DeleteRange();
            ectx.Uow.Save();
        }

        public void DeleteForMulticast(int multicastId)
        {
            ectx.Uow.ActiveImagingTaskRepository.DeleteRange(t => t.MulticastId == multicastId);
            ectx.Uow.Save();
        }

        public DtoActionResult DeleteUnregisteredOndTask(int activeImagingTaskId)
        {
            var activeImagingTask = ectx.Uow.ActiveImagingTaskRepository.GetById(activeImagingTaskId);
            if (activeImagingTask == null) return new DtoActionResult { ErrorMessage = "Task Not Found", Id = 0 };
            if (activeImagingTask.ComputerId > -1)
                return new DtoActionResult
                {
                    ErrorMessage = "This Task Is Not An On Demand Task And Cannot Be Cancelled",
                    Id = 0
                };

            ectx.Uow.ActiveImagingTaskRepository.Delete(activeImagingTask.Id);
            ectx.Uow.Save();

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = activeImagingTaskId;

            if (activeImagingTask.Type.Contains("upload"))
            {
                var comServer = ectx.Uow.ClientComServerRepository.GetById(activeImagingTask.ComServerId);
                if (comServer == null)
                    return actionResult;

                var receiverPids = ectx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == activeImagingTask.Id).Select(x => x.Pid).ToList();
                ectx.Uow.ActiveMulticastSessionRepository.DeleteRange(x => x.UploadTaskId == activeImagingTask.Id);
                ectx.Uow.Save();
                var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ectx.Encryption.DecryptText(intercomKey);

                new APICall().ClientComServerApi.KillUdpReceiver(comServer.Url, "", decryptedKey, receiverPids);
            }

            return actionResult;
        }

        public List<EntityActiveImagingTask> GetAll()
        {
            return ectx.Uow.ActiveImagingTaskRepository.Get();
        }

        public EntityActiveImagingTask GetFromWebToken(string token)
        {
            return ectx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(x => x.WebTaskToken.Equals(token));
        }

        public EntityActiveImagingTask GetForComputer(int computerId)
        {
            return ectx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(x => x.ComputerId.Equals(computerId));
        }

        public List<EntityActiveImagingTask> GetAllOnDemandUnregistered()
        {
            var tasks = ectx.Uow.ActiveImagingTaskRepository.Get(x => x.ComputerId < -1);
            foreach(var task in tasks)
            {
                var c = ectx.Uow.ClientComServerRepository.GetById(task.ComServerId);
                if (c != null)
                    task.ComServerName = c.DisplayName;
            }

            return tasks;
        }

        public int GetCurrentQueue(EntityActiveImagingTask activeTask)
        {
            return
                Convert.ToInt32(
                    ectx.Uow.ActiveImagingTaskRepository.Count(
                        x => x.Status == EnumTaskStatus.ImagingStatus.Imaging && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId));
        }

        public EntityActiveImagingTask GetLastQueuedTask(EntityActiveImagingTask activeTask)
        {
            return
                ectx.Uow.ActiveImagingTaskRepository.Get(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId,
                    q => q.OrderByDescending(t => t.QueuePosition)).FirstOrDefault();
        }

        public List<EntityComputer> GetMulticastComputers(int multicastId)
        {
            return ectx.Uow.ActiveImagingTaskRepository.MulticastComputers(multicastId);
        }

        public EntityActiveImagingTask GetNextComputerInQueue(EntityActiveImagingTask activeTask)
        {
            return
                ectx.Uow.ActiveImagingTaskRepository.Get(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId,
                    q => q.OrderBy(t => t.QueuePosition)).FirstOrDefault();
        }

        public string GetQueuePosition(EntityActiveImagingTask task)
        {
            return
                ectx.Uow.ActiveImagingTaskRepository.Count(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.QueuePosition < task.QueuePosition);
        }

        public EntityActiveImagingTask GetTask(int taskId)
        {
            return ectx.Uow.ActiveImagingTaskRepository.GetById(taskId);
        }

        public List<TaskWithComputer> MulticastMemberStatus(int multicastId)
        {
            return ectx.Uow.ActiveImagingTaskRepository.GetMulticastMembers(multicastId);
        }

        public List<EntityActiveImagingTask> MulticastProgress(int multicastId)
        {
            return ectx.Uow.ActiveImagingTaskRepository.MulticastProgress(multicastId);
        }

        public int OnDemandCount()
        {
            return Convert.ToInt32(ectx.Uow.ActiveImagingTaskRepository.Count(x => x.ComputerId < -1));
        }

        public List<TaskWithComputer> ReadAll(int userId)
        {
            //Admins see all tasks
            return userService.IsAdmin(userId)
                ? ectx.Uow.ActiveImagingTaskRepository.GetAllTaskWithComputersForAdmin()
                : ectx.Uow.ActiveImagingTaskRepository.GetAllTaskWithComputers(userId);
        }

    

        public List<TaskWithComputer> ReadUnicasts(int userId)
        {
            //Admins see all tasks
            var activeImagingTasks = userService.IsAdmin(userId)
                ? ectx.Uow.ActiveImagingTaskRepository.GetUnicastsWithComputersForAdmin()
                : ectx.Uow.ActiveImagingTaskRepository.GetUnicastsWithComputers(userId);

            return activeImagingTasks;
        }

        public void SendTaskCompletedEmail(EntityActiveImagingTask task)
        {
            //Mail not enabled
            if (ectx.Settings.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;
            var computer = new ServiceComputer().GetComputer(task.ComputerId);
            if (computer == null) return;
            foreach (
                var user in
                    userService.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = userService.GetUserRights(user.Id).Select(right => right.Right).ToList();
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
            if (ectx.Settings.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;
            var computer = new ServiceComputer().GetComputer(task.ComputerId);
            foreach (
                var user in
                    userService.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = userService.GetUserRights(user.Id).Select(right => right.Right).ToList();
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
            ectx.Uow.ActiveImagingTaskRepository.Update(activeImagingTask, activeImagingTask.Id);
            ectx.Uow.Save();
            return true;
        }

    }
}
