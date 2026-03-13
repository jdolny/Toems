using System.Diagnostics;
using System.Management;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveImagingTask(ServiceContext ctx)
    {
        public string ActiveCountNotOwnedByuser(int userId)
        {
            return ctx.User.IsAdmin(userId)
                ? "0"
                : ctx.Uow.ActiveImagingTaskRepository.Count(x => x.UserId != userId);
        }

        public string ActiveUnicastCount(int userId, string taskType = "")
        {
           
            return ctx.User.IsAdmin(userId)
                ? ctx.Uow.ActiveImagingTaskRepository.Count(t => t.Type == "deploy" || t.Type == "upload")
                : ctx.Uow.ActiveImagingTaskRepository.Count(
                    t => (t.Type == "deploy" || t.Type == "upload") && t.UserId == userId);
        }

        public bool AddActiveImagingTask(EntityActiveImagingTask activeImagingTask)
        {
            ctx.Uow.ActiveImagingTaskRepository.Insert(activeImagingTask);
            ctx.Uow.Save();
            return true;
        }

        public string AllActiveCount(int userId)
        {
            return ctx.User.IsAdmin(userId)
                ? ctx.Uow.ActiveImagingTaskRepository.Count()
                : ctx.Uow.ActiveImagingTaskRepository.Count(x => x.UserId == userId);
        }

        public int AllActiveCountAdmin()
        {
            return Convert.ToInt32(ctx.Uow.ActiveImagingTaskRepository.Count());
        }

        public void CancelTimedOutTasks()
        {
            var timeout = ctx.Setting.GetSettingValue(SettingStrings.ImageTaskTimeoutMinutes);
            if (string.IsNullOrEmpty(timeout)) return;
            if (timeout == "0") return;
            var tasks = GetAll();

            foreach (var task in tasks)
            {
                if (DateTime.UtcNow > task.LastUpdateTimeUTC.AddMinutes(Convert.ToInt32(timeout)))
                {
                    DeleteActiveImagingTask(task.Id);
                    ctx.Log.Debug("Task Timeout Hit. Task " + task.Id + "Cancelled.  Computer Id " + task.ComputerId);
                }
            }
        }

        public DtoActionResult DeleteActiveImagingTask(int activeImagingTaskId)
        {
            var activeImagingTask = ctx.Uow.ActiveImagingTaskRepository.GetById(activeImagingTaskId);
            if (activeImagingTask == null) return new DtoActionResult { ErrorMessage = "Task Not Found", Id = 0 };
            var computer = ctx.Uow.ComputerRepository.GetById(activeImagingTask.ComputerId);

            ctx.Uow.ActiveImagingTaskRepository.Delete(activeImagingTask.Id);
            ctx.Uow.Save();

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = activeImagingTaskId;

            if (computer != null)
                ctx.CleanTaskBootFiles.RunAllServers(computer);
    
            if(activeImagingTask.Type.Contains("upload"))
            {
                var comServer = ctx.Uow.ClientComServerRepository.GetById(activeImagingTask.ComServerId);
                if (comServer == null)
                    return actionResult;

                var receiverPids = ctx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == activeImagingTask.Id).Select(x => x.Pid).ToList();
                ctx.Uow.ActiveMulticastSessionRepository.DeleteRange(x => x.UploadTaskId == activeImagingTask.Id);
                ctx.Uow.Save();
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);

                //todo - fix
                //new APICall().ClientComServerApi.KillUdpReceiver(comServer.Url, "", decryptedKey, receiverPids);              
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
            ctx.Uow.ActiveImagingTaskRepository.DeleteRange();
            ctx.Uow.Save();
        }

        public void DeleteForMulticast(int multicastId)
        {
            ctx.Uow.ActiveImagingTaskRepository.DeleteRange(t => t.MulticastId == multicastId);
            ctx.Uow.Save();
        }

        public DtoActionResult DeleteUnregisteredOndTask(int activeImagingTaskId)
        {
            var activeImagingTask = ctx.Uow.ActiveImagingTaskRepository.GetById(activeImagingTaskId);
            if (activeImagingTask == null) return new DtoActionResult { ErrorMessage = "Task Not Found", Id = 0 };
            if (activeImagingTask.ComputerId > -1)
                return new DtoActionResult
                {
                    ErrorMessage = "This Task Is Not An On Demand Task And Cannot Be Cancelled",
                    Id = 0
                };

            ctx.Uow.ActiveImagingTaskRepository.Delete(activeImagingTask.Id);
            ctx.Uow.Save();

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = activeImagingTaskId;

            if (activeImagingTask.Type.Contains("upload"))
            {
                var comServer = ctx.Uow.ClientComServerRepository.GetById(activeImagingTask.ComServerId);
                if (comServer == null)
                    return actionResult;

                var receiverPids = ctx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == activeImagingTask.Id).Select(x => x.Pid).ToList();
                ctx.Uow.ActiveMulticastSessionRepository.DeleteRange(x => x.UploadTaskId == activeImagingTask.Id);
                ctx.Uow.Save();
                var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = ctx.Encryption.DecryptText(intercomKey);

                //todo - fix
                //new APICall().ClientComServerApi.KillUdpReceiver(comServer.Url, "", decryptedKey, receiverPids);
            }

            return actionResult;
        }

        public List<EntityActiveImagingTask> GetAll()
        {
            return ctx.Uow.ActiveImagingTaskRepository.Get();
        }

        public EntityActiveImagingTask GetFromWebToken(string token)
        {
            return ctx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(x => x.WebTaskToken.Equals(token));
        }

        public EntityActiveImagingTask GetForComputer(int computerId)
        {
            return ctx.Uow.ActiveImagingTaskRepository.GetFirstOrDefault(x => x.ComputerId.Equals(computerId));
        }

        public List<EntityActiveImagingTask> GetAllOnDemandUnregistered()
        {
            var tasks = ctx.Uow.ActiveImagingTaskRepository.Get(x => x.ComputerId < -1);
            foreach(var task in tasks)
            {
                var c = ctx.Uow.ClientComServerRepository.GetById(task.ComServerId);
                if (c != null)
                    task.ComServerName = c.DisplayName;
            }

            return tasks;
        }

        public int GetCurrentQueue(EntityActiveImagingTask activeTask)
        {
            return
                Convert.ToInt32(
                    ctx.Uow.ActiveImagingTaskRepository.Count(
                        x => x.Status == EnumTaskStatus.ImagingStatus.Imaging && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId));
        }

        public EntityActiveImagingTask GetLastQueuedTask(EntityActiveImagingTask activeTask)
        {
            return
                ctx.Uow.ActiveImagingTaskRepository.Get(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId,
                    q => q.OrderByDescending(t => t.QueuePosition)).FirstOrDefault();
        }

        public List<EntityComputer> GetMulticastComputers(int multicastId)
        {
            return ctx.Uow.ActiveImagingTaskRepository.MulticastComputers(multicastId);
        }

        public EntityActiveImagingTask GetNextComputerInQueue(EntityActiveImagingTask activeTask)
        {
            return
                ctx.Uow.ActiveImagingTaskRepository.Get(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.Type == activeTask.Type && x.ComServerId == activeTask.ComServerId,
                    q => q.OrderBy(t => t.QueuePosition)).FirstOrDefault();
        }

        public string GetQueuePosition(EntityActiveImagingTask task)
        {
            return
                ctx.Uow.ActiveImagingTaskRepository.Count(
                    x => x.Status == EnumTaskStatus.ImagingStatus.InImagingQueue && x.QueuePosition < task.QueuePosition);
        }

        public EntityActiveImagingTask GetTask(int taskId)
        {
            return ctx.Uow.ActiveImagingTaskRepository.GetById(taskId);
        }

        public List<TaskWithComputer> MulticastMemberStatus(int multicastId)
        {
            return ctx.Uow.ActiveImagingTaskRepository.GetMulticastMembers(multicastId);
        }

        public List<EntityActiveImagingTask> MulticastProgress(int multicastId)
        {
            return ctx.Uow.ActiveImagingTaskRepository.MulticastProgress(multicastId);
        }

        public int OnDemandCount()
        {
            return Convert.ToInt32(ctx.Uow.ActiveImagingTaskRepository.Count(x => x.ComputerId < -1));
        }

        public async Task<List<TaskWithComputer>> ReadAll(int userId)
        {
            //Admins see all tasks
            
            return ctx.User.IsAdmin(userId) ? ctx.Uow.ActiveImagingTaskRepository.GetAllTaskWithComputersForAdmin()
                : ctx.Uow.ActiveImagingTaskRepository.GetAllTaskWithComputers(userId);

         
            
        }

    

        public List<TaskWithComputer> ReadUnicasts(int userId)
        {
            //Admins see all tasks
            var activeImagingTasks = ctx.User.IsAdmin(userId)
                ? ctx.Uow.ActiveImagingTaskRepository.GetUnicastsWithComputersForAdmin()
                : ctx.Uow.ActiveImagingTaskRepository.GetUnicastsWithComputers(userId);

            return activeImagingTasks;
        }

        public async Task SendTaskCompletedEmail(EntityActiveImagingTask task)
        {
            //Mail not enabled
            if (ctx.Setting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;
            var computer = ctx.Computer.GetComputer(task.ComputerId);
            if (computer == null) return;
            foreach (
                var user in
                    ctx.User.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = ctx.User.GetUserRights(user.Id).Select(right => right.Right).ToList();
                if (rights.Any(right => right == AuthorizationStrings.EmailImagingTaskCompleted))
                {
                    if (task.UserId == user.Id)
                    {
                        await ctx.Mail.SendMailAsync(computer.Name + " Image Task Has Completed.",user.Email, "Task Completed");
                    }
                }
            }
        }

        public async Task SendTaskErrorEmail(EntityActiveImagingTask task, string error)
        {
            //Mail not enabled
            if (ctx.Setting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;
            var computer = ctx.Computer.GetComputer(task.ComputerId);
            foreach (
                var user in
                    ctx.User.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = ctx.User.GetUserRights(user.Id).Select(right => right.Right).ToList();
                if (rights.Any(right => right == AuthorizationStrings.EmailImagingTaskFailed))
                {
                    if (task.UserId == user.Id)
                    {
                        if (computer == null)
                        {
                            computer = new EntityComputer();
                            computer.Name = "Unknown Computer";
                        }
                        await ctx.Mail.SendMailAsync(computer.Name + " Image Task Has Failed. " + error,user.Email, "Task Failed");
                    }
                }
            }
        }

        public bool UpdateActiveImagingTask(EntityActiveImagingTask activeImagingTask)
        {
            ctx.Uow.ActiveImagingTaskRepository.Update(activeImagingTask, activeImagingTask.Id);
            ctx.Uow.Save();
            return true;
        }

    }
}
