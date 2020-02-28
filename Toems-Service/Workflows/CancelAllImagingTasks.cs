using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class CancelAllImagingTasks
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private EntityClientComServer _thisComServer;

        public bool RunAllServers()
        {

            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer || x.IsMulticastServer);

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
            var NoErrors = true;
            foreach (var com in comServers)
            {
                if (!new APICall().ClientComServerApi.CancelAllImagingTasks(com.Url, "", decryptedKey))
                    NoErrors = false;
            }

            return NoErrors;

        }

        public bool Execute()
        {
            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            _thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                Logger.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }

            if (string.IsNullOrEmpty(_thisComServer.TftpPath))
            {
                Logger.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
                return false;
            }

            if (_thisComServer.IsTftpServer)
            {

                var tftpPath = _thisComServer.TftpPath;
                var pxePaths = new List<string>
                {
                tftpPath + "pxelinux.cfg" + Path.DirectorySeparatorChar,
                tftpPath + "proxy" + Path.DirectorySeparatorChar + "bios" +
                Path.DirectorySeparatorChar + "pxelinux.cfg" + Path.DirectorySeparatorChar,
                tftpPath + "proxy" + Path.DirectorySeparatorChar + "efi32" +
                Path.DirectorySeparatorChar + "pxelinux.cfg" + Path.DirectorySeparatorChar,
                tftpPath + "proxy" + Path.DirectorySeparatorChar + "efi64" +
                Path.DirectorySeparatorChar + "pxelinux.cfg" + Path.DirectorySeparatorChar
                };

                foreach (var pxePath in pxePaths)
                {
                    var pxeFiles = Directory.GetFiles(pxePath, "01*");
                    try
                    {
                        foreach (var pxeFile in pxeFiles)
                        {
                            File.Delete(pxeFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                        return false;
                    }
                }
            }

            if(_thisComServer.IsMulticastServer)
            {
                if (Environment.OSVersion.ToString().Contains("Unix"))
                {
                    for (var x = 1; x <= 10; x++)
                    {
                        try
                        {
                            var killProcInfo = new ProcessStartInfo
                            {
                                FileName = "killall",
                                Arguments = " -s SIGKILL udp-sender"
                            };
                            Process.Start(killProcInfo);
                        }
                        catch
                        {
                            // ignored
                        }
                        try
                        {
                            var killProcInfo = new ProcessStartInfo
                            {
                                FileName = "killall",
                                Arguments = " -s SIGKILL udp-receiver"
                            };
                            Process.Start(killProcInfo);
                        }
                        catch
                        {
                            // ignored
                        }

                        Thread.Sleep(200);
                    }
                }

                else
                {
                    for (var x = 1; x <= 10; x++)
                    {
                        foreach (var p in Process.GetProcessesByName("udp-sender"))
                        {
                            try
                            {
                                p.Kill();
                                p.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.ToString());
                            }
                        }
                        foreach (var p in Process.GetProcessesByName("udp-receiver"))
                        {
                            try
                            {
                                p.Kill();
                                p.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.ToString());
                            }
                        }
                        Thread.Sleep(200);
                    }
                }
            }

            new ServiceActiveImagingTask().DeleteAll();
            new ServiceActiveMulticastSession().DeleteAll();
            return true;
        }
    }
}
