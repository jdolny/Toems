using System.Diagnostics;
using System.Management;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveMulticastSession(EntityContext ectx, ServiceUser userService, ServiceActiveImagingTask activeImagingTaskService)
    {
        public string ActiveCount(int userId)
        {
            return userService.IsAdmin(userId)
                ? ectx.Uow.ActiveMulticastSessionRepository.Count()
                : ectx.Uow.ActiveMulticastSessionRepository.Count(x => x.UserId == userId && x.UploadTaskId == null);
        }

        public bool AddActiveMulticastSession(EntityActiveMulticastSession activeMulticastSession)
        {
            ectx.Uow.ActiveMulticastSessionRepository.Insert(activeMulticastSession);
            ectx.Uow.Save();
            return true;
        }

        public DtoActionResult DeleteUpload(int multicastId)
        {
            var upload = ectx.Uow.ActiveMulticastSessionRepository.GetById(multicastId);
            if (upload == null) return new DtoActionResult { ErrorMessage = "Upload Session Not Found", Id = 0 };

            var actionResult = new DtoActionResult();
            ectx.Uow.ActiveMulticastSessionRepository.Delete(multicastId);
            ectx.Uow.Save();
            actionResult.Id = upload.Id;
            actionResult.Success = true;

            return actionResult;
        }

        public DtoActionResult Delete(int multicastId)
        {
            var multicast = ectx.Uow.ActiveMulticastSessionRepository.GetById(multicastId);
            if (multicast == null) return new DtoActionResult { ErrorMessage = "Multicast Not Found", Id = 0 };
            var computers = ectx.Uow.ActiveImagingTaskRepository.MulticastComputers(multicastId);

            var actionResult = new DtoActionResult();
            ectx.Uow.ActiveMulticastSessionRepository.Delete(multicastId);
            ectx.Uow.Save();
            actionResult.Id = multicast.Id;
            actionResult.Success = true;

            activeImagingTaskService.DeleteForMulticast(multicastId);

            if (computers != null)
            {
                foreach (var computer in computers)
                {
                    if (computer != null)
                        new CleanTaskBootFiles().Execute(computer);
                }
            }


            var comServer = new ServiceClientComServer().GetServer(multicast.ComServerId);
            if(comServer == null)
            {
                actionResult.Success = false;
                ectx.Log.Error("Could Not find com Server With ID " + multicast.ComServerId);
                return actionResult;
            }

            var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ectx.Encryption.DecryptText(intercomKey);

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
                ectx.Log.Error(ex.ToString());
                return false;
                //Message.Text = "Could Not Kill Process.  Check The Exception Log For More Info";

            }

            return true;
        }

        public void DeleteAll()
        {
            ectx.Uow.ActiveMulticastSessionRepository.DeleteRange(x => x.UploadTaskId == null);
            ectx.Uow.Save();
        }

        public EntityActiveMulticastSession Get(int multicastId)
        {
            return ectx.Uow.ActiveMulticastSessionRepository.GetById(multicastId);
        }

        public List<EntityActiveMulticastSession> GetAll()
        {
            return ectx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == null);
        }

        public List<EntityActiveMulticastSession> GetAllMulticastSessions(int userId)
        {
            if (userService.IsAdmin(userId))
                return ectx.Uow.ActiveMulticastSessionRepository.Get(x => x.UploadTaskId == null, orderBy: q => q.OrderBy(t => t.Name));
            return ectx.Uow.ActiveMulticastSessionRepository.Get(x => x.UserId == userId && x.UploadTaskId == null, q => q.OrderBy(t => t.Name));
        }

      
        public List<EntityActiveMulticastSession> GetOnDemandList()
        {
            return ectx.Uow.ActiveMulticastSessionRepository.Get(x => x.ImageProfileId != -1 && x.UploadTaskId == null, q => q.OrderBy(t => t.Name));
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
                ectx.Log.Error(ex.ToString());
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
                ectx.Log.Error(ex.ToString());
            }
        }

        public void SendMulticastCompletedEmail(EntityActiveMulticastSession session)
        {
            //Mail not enabled
            if (ectx.Settings.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;

            foreach (
                var user in
                    userService.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = userService.GetUserRights(user.Id).Select(right => right.Right).ToList();
                if (rights.Any(right => right == AuthorizationStrings.EmailImagingTaskCompleted))
                {
                    if (session.UserId == user.Id)
                    {
                        var mail = new MailServices
                        {
                            MailTo = user.Email,
                            Body = session.Name + " Multicast Task Has Completed.",
                            Subject = "Multicast Completed"
                        };
                        mail.Send();
                    }
                }
            }
        }

        public bool UpdateActiveMulticastSession(EntityActiveMulticastSession activeMulticastSession)
        {
            ectx.Uow.ActiveMulticastSessionRepository.Update(activeMulticastSession, activeMulticastSession.Id);
            ectx.Uow.Save();
            return true;
        }
    }
}
