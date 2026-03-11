using System.Diagnostics;
using log4net;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class CancelAllImagingTasks(ServiceContext ctx)
    {

        private EntityClientComServer _thisComServer;

        public bool RunAllServers()
        {

            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer || x.IsMulticastServer);

            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
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
            var guid = ctx.Config["ComServerUniqueId"];
            _thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }

            if (string.IsNullOrEmpty(_thisComServer.TftpPath))
            {
                ctx.Log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
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
                        ctx.Log.Error(ex.ToString());
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
                                ctx.Log.Error(ex.ToString());
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
                                ctx.Log.Error(ex.ToString());
                            }
                        }
                        Thread.Sleep(200);
                    }
                }
            }

            ctx.ActiveImagingTask.DeleteAll();
            ctx.ActiveMulticastSession.DeleteAll();
            return true;
        }
    }
}
