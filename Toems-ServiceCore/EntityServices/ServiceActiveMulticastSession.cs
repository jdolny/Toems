using System.Diagnostics;
using System.Management;
using log4net;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveMulticastSession(ServiceContext ctx)
    {
        public string ActiveCount(int userId)
        {
            return ctx.User.IsAdmin(userId)
                ? ctx.Uow.ActiveMulticastSessionRepository.Count()
                : ctx.Uow.ActiveMulticastSessionRepository.Count(x => x.UserId == userId && x.UploadTaskId == null);
        }

        public bool AddActiveMulticastSession(EntityActiveMulticastSession activeMulticastSession)
        {
            ctx.Uow.ActiveMulticastSessionRepository.Insert(activeMulticastSession);
            ctx.Uow.Save();
            return true;
        }

        public DtoActionResult DeleteUpload(int multicastId)
        {
            var upload = ctx.Uow.ActiveMulticastSessionRepository.GetById(multicastId);
            if (upload == null) return new DtoActionResult { ErrorMessage = "Upload Session Not Found", Id = 0 };

            var actionResult = new DtoActionResult();
            ctx.Uow.ActiveMulticastSessionRepository.Delete(multicastId);
            ctx.Uow.Save();
            actionResult.Id = upload.Id;
            actionResult.Success = true;

            return actionResult;
        }

        public DtoActionResult Delete(int multicastId)
        {
            var multicast = ctx.Uow.ActiveMulticastSessionRepository.GetById(multicastId);
            if (multicast == null) return new DtoActionResult { ErrorMessage = "Multicast Not Found", Id = 0 };
            var computers = ctx.Uow.ActiveImagingTaskRepository.MulticastComputers(multicastId);

            var actionResult = new DtoActionResult();
            ctx.Uow.ActiveMulticastSessionRepository.Delete(multicastId);
            ctx.Uow.Save();
            actionResult.Id = multicast.Id;
            actionResult.Success = true;

            ctx.ActiveImagingTask.DeleteForMulticast(multicastId);

            if (computers != null)
            {
                foreach (var computer in computers)
                {
                    if (computer != null)
                        ctx.CleanTaskBootFiles.Execute(computer);
                }
            }


            var comServer = ctx.ClientComServer.GetServer(multicast.ComServerId);
            if(comServer == null)
            {
                actionResult.Success = false;
                ctx.Log.Error("Could Not find com Server With ID " + multicast.ComServerId);
                return actionResult;
            }

            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);

            if (!new APICall().ClientComServerApi.TerminateMulticast(comServer.Url, "", decryptedKey, multicast))
                  actionResult.Success = false;
            

            return actionResult;
        }

        public bool TerminateMulticastProcesses(EntityActiveMulticastSession multicast)
        {
            try
            {
                var prs = Process.GetProcessById(Convert.ToInt32(multicast.Pid));
                var processName = prs.ProcessName;
                if (Environment.OSVersion.ToString().Contains("Unix"))
                {
                    for (var x = 1; x <= 5; x++)
                    {
                        KillProcessLinux(Convert.ToInt32(multicast.Pid));
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    if (processName == "cmd")
                        KillProcess(Convert.ToInt32(multicast.Pid));
                }
                //Message.Text = "Successfully Deleted Task";
            }
            catch (Exception ex)
            {
                ctx.Log.Error(ex.ToString());
                return false;
                //Message.Text = "Could Not Kill Process.  Check The Exception Log For More Info";

            }

            return true;
        }

        public void DeleteAll()
        {
            ctx.Uow.ActiveMulticastSessionRepository.DeleteRange(x => x.UploadTaskId == null);
            ctx.Uow.Save();
        }

        public EntityActiveMulticastSession Get(int multicastId)
        {
            return ctx.Uow.ActiveMulticastSessionRepository.GetById(multicastId);
        }

        public List<EntityActiveMulticastSession> GetAll()
        {
            return ctx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == null);
        }

        public List<EntityActiveMulticastSession> GetAllMulticastSessions(int userId)
        {
            if (ctx.User.IsAdmin(userId))
                return ctx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == null, orderBy: q => q.OrderBy(t => t.Name));
            return ctx.Uow.ActiveMulticastSessionRepository.Get(x => x.UserId == userId && x.UploadTaskId == null, q => q.OrderBy(t => t.Name));
        }

      
        public List<EntityActiveMulticastSession> GetOnDemandList()
        {
            return ctx.Uow.ActiveMulticastSessionRepository.Get(x => x.ImageProfileId != -1 && x.UploadTaskId == null, q => q.OrderBy(t => t.Name));
        }

        private void KillProcess(int pid)
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
            catch (Exception ex)
            {
                ctx.Log.Error(ex.ToString());
            }
        }

        public void KillProcessLinux(int pid)
        {
            try
            {
                var killProcInfo = new ProcessStartInfo
                {
                    FileName = "pkill",
                    Arguments = " -SIGKILL -P " + pid
                };
                Process.Start(killProcInfo);
            }
            catch (Exception ex)
            {
                ctx.Log.Error(ex.ToString());
            }
        }

        public async Task SendMulticastCompletedEmail(EntityActiveMulticastSession session)
        {
            //Mail not enabled
            if (ctx.Setting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;

            foreach (
                var user in
                    ctx.User.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = ctx.User.GetUserRights(user.Id).Select(right => right.Right).ToList();
                if (rights.Any(right => right == AuthorizationStrings.EmailImagingTaskCompleted))
                {
                    if (session.UserId == user.Id)
                    {
                        await ctx.Mail.SendMailAsync(session.Name + " Multicast Task Has Completed.",user.Email, "Multicast Completed");
                    }
                }
            }
        }

        public bool UpdateActiveMulticastSession(EntityActiveMulticastSession activeMulticastSession)
        {
            ctx.Uow.ActiveMulticastSessionRepository.Update(activeMulticastSession, activeMulticastSession.Id);
            ctx.Uow.Save();
            return true;
        }
    }
}
