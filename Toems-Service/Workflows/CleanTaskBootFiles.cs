using System;
using System.Configuration;
using System.IO;
using log4net;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class CleanTaskBootFiles
    {
        private const string ConfigFolder = "pxelinux.cfg";
        private  string _bootFile;
        private  EntityComputer _computer;
        private  ServiceComputer _computerServices;
        private EntityClientComServer _thisComServer;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool RunAllServers(EntityComputer computer)
        {
         
            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);
           
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
            var NoErrors = true;
            foreach (var com in comServers)
            {
                if (!new APICall().ClientComServerApi.CleanTaskBootFiles(com.Url, "", decryptedKey,computer))
                    NoErrors = false;
            }

            return NoErrors;

        }


        public bool Execute(EntityComputer computer)
        {
            _computer = computer;
            _bootFile = StringManipulationServices.MacToPxeMac(_computer.ImagingMac);
            _computerServices = new ServiceComputer();

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

            if (ServiceSetting.GetSettingValue(SettingStrings.ProxyDhcpEnabled) == "Yes")
            {
                DeleteProxyFile("bios");
                DeleteProxyFile("bios", ".ipxe");
                DeleteProxyFile("efi32");
                DeleteProxyFile("efi32", ".ipxe");
                DeleteProxyFile("efi64");
                DeleteProxyFile("efi64", ".ipxe");
                DeleteProxyFile("efi64", ".cfg");
            }
            else
            {
                var mode = ServiceSetting.GetSettingValue(SettingStrings.PxeBootloader);
                if (mode.Contains("ipxe"))
                    DeleteStandardFile(".ipxe");
                else if (mode.Contains("grub"))
                    DeleteStandardFile(".cfg");
                else
                    DeleteStandardFile();
            }
            return true;

        }

        private void DeleteProxyFile(string architecture, string extension = "")
        {

            try
            {
                File.Delete(_thisComServer.TftpPath + "proxy" +
                            Path.DirectorySeparatorChar + architecture +
                            Path.DirectorySeparatorChar + ConfigFolder + Path.DirectorySeparatorChar + _bootFile +
                            extension);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private void DeleteStandardFile(string extension = "")
        {
            try
            {
                File.Delete(_thisComServer.TftpPath + ConfigFolder +
                            Path.DirectorySeparatorChar +
                            _bootFile + extension);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }


        }
    }
}
