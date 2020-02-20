using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Workflows;

namespace Toems_Service.Entity
{
    public class ServiceActiveMulticastSession
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _uow;
        private readonly ServiceUser _userServices;

        public ServiceActiveMulticastSession()
        {
            _uow = new UnitOfWork();
            _userServices = new ServiceUser();
        }

        public string ActiveCount(int userId)
        {
            return _userServices.IsAdmin(userId)
                ? _uow.ActiveMulticastSessionRepository.Count()
                : _uow.ActiveMulticastSessionRepository.Count(x => x.UserId == userId);
        }

        public bool AddActiveMulticastSession(EntityActiveMulticastSession activeMulticastSession)
        {
            if (_uow.ActiveMulticastSessionRepository.Exists(h => h.Name == activeMulticastSession.Name))
            {
                //Message.Text = "A Multicast Is Already Running For This Group";
                return false;
            }
            _uow.ActiveMulticastSessionRepository.Insert(activeMulticastSession);
            _uow.Save();
            return true;
        }

        public DtoActionResult Delete(int multicastId)
        {
            var multicast = _uow.ActiveMulticastSessionRepository.GetById(multicastId);
            if (multicast == null) return new DtoActionResult { ErrorMessage = "Multicast Not Found", Id = 0 };
            var computers = _uow.ActiveImagingTaskRepository.MulticastComputers(multicastId);

            var actionResult = new DtoActionResult();
            _uow.ActiveMulticastSessionRepository.Delete(multicastId);
            _uow.Save();
            actionResult.Id = multicast.Id;
            actionResult.Success = true;

            new ServiceActiveImagingTask().DeleteForMulticast(multicastId);

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
                Logger.Error("Could Not find com Server With ID " + multicast.ComServerId);
                return actionResult;
            }

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

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
                Logger.Error(ex.ToString());
                return false;
                //Message.Text = "Could Not Kill Process.  Check The Exception Log For More Info";

            }

            return true;
        }

        public void DeleteAll()
        {
            _uow.ActiveMulticastSessionRepository.DeleteRange();
            _uow.Save();
        }

        public EntityActiveMulticastSession Get(int multicastId)
        {
            return _uow.ActiveMulticastSessionRepository.GetById(multicastId);
        }

        public List<EntityActiveMulticastSession> GetAll()
        {
            return _uow.ActiveMulticastSessionRepository.Get();
        }

        public List<EntityActiveMulticastSession> GetAllMulticastSessions(int userId)
        {
            if (_userServices.IsAdmin(userId))
                return _uow.ActiveMulticastSessionRepository.Get(orderBy: q => q.OrderBy(t => t.Name));
            return _uow.ActiveMulticastSessionRepository.Get(x => x.UserId == userId, q => q.OrderBy(t => t.Name));
        }

        public EntityActiveMulticastSession GetFromPort(int port)
        {
            return _uow.ActiveMulticastSessionRepository.GetFirstOrDefault(x => x.Port == port);
        }

        public List<EntityActiveMulticastSession> GetOnDemandList()
        {
            return _uow.ActiveMulticastSessionRepository.Get(x => x.ImageProfileId != -1, q => q.OrderBy(t => t.Name));
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
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public static void KillProcessLinux(int pid)
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
                Logger.Error(ex.ToString());
            }
        }

        public void SendMulticastCompletedEmail(EntityActiveMulticastSession session)
        {
            //Mail not enabled
            if (ServiceSetting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;

            foreach (
                var user in
                    _userServices.GetAll().Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                var rights = new ServiceUser().GetUserRights(user.Id).Select(right => right.Right).ToList();
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
            _uow.ActiveMulticastSessionRepository.Update(activeMulticastSession, activeMulticastSession.Id);
            _uow.Save();
            return true;
        }
    }
}
