using log4net;
using Toems_Common;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class CleanTaskBootFiles(ServiceContext ctx)
    {
        private const string ConfigFolder = "pxelinux.cfg";
        private EntityComputer _computer;

        private EntityClientComServer _thisComServer;
        private List<string> _listOfMacs;

        public bool RunAllServers(EntityComputer computer)
        {
            
            var comServers = ctx.Uow.ClientComServerRepository.Get(x => x.IsTftpServer);

            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
            var NoErrors = true;
            foreach (var com in comServers)
            {
                //todo - fix
                //if (!new APICall().ClientComServerApi.CleanTaskBootFiles(com.Url, "", decryptedKey, computer))
                    NoErrors = false;
            }

            return NoErrors;

        }


        public bool Execute(EntityComputer computer)
        {
            _computer = computer;
            _listOfMacs = new List<string>();
            if (!string.IsNullOrEmpty(_computer.ImagingMac))
                _listOfMacs.Add(StringManipulationServices.MacToPxeMac(_computer.ImagingMac));

            var computerMacs = ctx.Uow.NicInventoryRepository.Get(x => x.ComputerId == computer.Id && x.Type.Equals("Ethernet")).Select(x => x.Mac).ToList();
            foreach (var mac in computerMacs)
            {
                _listOfMacs.Add(StringManipulationServices.MacToPxeMac(mac));
            }


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

            if (ctx.Setting.GetSettingValue(SettingStrings.ProxyDhcpEnabled) == "Yes")
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
                var mode = ctx.Setting.GetSettingValue(SettingStrings.PxeBootloader);
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
            foreach (var mac in _listOfMacs)
            {
                try
                {
                    File.Delete(_thisComServer.TftpPath + "proxy" +
                                Path.DirectorySeparatorChar + architecture +
                                Path.DirectorySeparatorChar + ConfigFolder + Path.DirectorySeparatorChar + mac +
                                extension);
                }
                catch (Exception ex)
                {
                    ctx.Log.Error(ex.Message);
                }
            }
        }

        private void DeleteStandardFile(string extension = "")
        {
            foreach (var mac in _listOfMacs)
            {
                try
                {
                    File.Delete(_thisComServer.TftpPath + ConfigFolder +
                                Path.DirectorySeparatorChar +
                                mac + extension);
                }
                catch (Exception ex)
                {
                    ctx.Log.Error(ex.Message);
                }
            }


        }
    }
    
}
