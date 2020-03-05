using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class TaskBootMenu
    {

        private EntityClientComServer _thisComServer;
        private string _userToken { get; set; }
        private EntityComputer _computer;
        private EntityImageProfile _imageProfile;
        private UnitOfWork _uow;
        private readonly ILog log = LogManager.GetLogger(typeof(TaskBootMenu));

        public bool RunAllServers(EntityComputer computer, EntityImageProfile imageProfile)
        {

            _uow = new UnitOfWork();
            var comServers = new Workflows.GetCompTftpServers().Run(computer.Id);
            if(comServers == null)
            {
                log.Error("Could Not Determine Tftp Com Servers For Computer: " + computer.Name);
                return false;
            }
            if (comServers.Count == 0)
            {
                log.Error("Could Not Determine Tftp Com Servers For Computer: " + computer.Name);
                return false;
            }

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
            var NoErrors = true;

            var dtoTaskBootFile = new DtoTaskBootFile();
            dtoTaskBootFile.Computer = computer;
            dtoTaskBootFile.ImageProfile = imageProfile;
            foreach (var com in comServers)
            {
                if (!new APICall().ClientComServerApi.CreateTaskBootFiles(com.Url, "", decryptedKey, dtoTaskBootFile))
                    NoErrors = false;
            }

            return NoErrors;

        }

        public bool CreatePxeBootFiles(EntityComputer computer, EntityImageProfile imageProfile)
        {
            _uow = new UnitOfWork();
            const string newLineChar = "\n";
            _computer = computer;
            _imageProfile = imageProfile;

            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            _thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }

            if (string.IsNullOrEmpty(_thisComServer.TftpPath))
            {
                log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
                return false;
            }

            var webRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.WebTasksRequireLogin);
            var globalToken = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingToken);
            if (webRequiresLogin.Equals("False"))
                _userToken = globalToken;
            else
                _userToken = "";

            var pxeComputerMac = StringManipulationServices.MacToPxeMac(_computer.ImagingMac);

            var imageComServers = new Workflows.GetCompImagingServers().Run(computer.Id);

            if (imageComServers == null)
            {
                log.Error("Could Not Determine Imaging Com Servers For Computer: " + computer.Name);
                return false;
            }
            if (imageComServers.Count == 0)
            {
                log.Error("Could Not Determine Imaging Com Servers For Computer: " + computer.Name);
                return false;
            }

            var webPath = "\"";
            foreach (var imageServer in imageComServers)
            {
                webPath += imageServer.Url + "clientimaging/ "; //adds a space delimiter
            }

            webPath = webPath.Trim(' ');
            webPath += "\"";

            var globalComputerArgs = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingArguments);

            var compTftpServers = new Workflows.GetCompTftpServers().Run(computer.Id);

            if (compTftpServers == null)
            {
                log.Error("Could Not Determine Tftp Com Servers For Computer: " + computer.Name);
                return false;
            }
            if (compTftpServers.Count == 0)
            {
                log.Error("Could Not Determine Tftp Com Servers For Computer: " + computer.Name);
                return false;
            }

            var iPxePath = compTftpServers.First().Url; //no way for fail over or load balance, just use first one
            if (iPxePath.Contains("https://"))
            {
                if (ServiceSetting.GetSettingValue(SettingStrings.IpxeSSL).Equals("False"))
                {
                    iPxePath = iPxePath.ToLower().Replace("https://", "http://");
                    var currentPort = iPxePath.Split(':').Last();
                    iPxePath = iPxePath.Replace(currentPort, ServiceSetting.GetSettingValue(SettingStrings.IpxeHttpPort)) + "/clientimaging/";
                }
            }


            var ipxe = new StringBuilder();
            ipxe.Append("#!ipxe" + newLineChar);
            ipxe.Append("kernel " + iPxePath + "IpxeBoot?filename=" + _imageProfile.Kernel +
                        "&type=kernel" + " initrd=" + _imageProfile.BootImage +
                        " root=/dev/ram0 rw ramdisk_size=156000" +
                        " consoleblank=0" + " web=" + webPath + " USER_TOKEN=" + _userToken + " " + globalComputerArgs +
                        " " + _imageProfile.KernelArguments + newLineChar);
            ipxe.Append("imgfetch --name " + _imageProfile.BootImage + " " + iPxePath +
                        "IpxeBoot?filename=" + _imageProfile.BootImage + "&type=bootimage" + newLineChar);
            ipxe.Append("boot" + newLineChar);

            var sysLinux = new StringBuilder();
            sysLinux.Append("DEFAULT theopenem" + newLineChar);
            sysLinux.Append("LABEL theopenem" + newLineChar);
            sysLinux.Append("KERNEL kernels" + Path.DirectorySeparatorChar + _imageProfile.Kernel + newLineChar);
            sysLinux.Append("APPEND initrd=images" + Path.DirectorySeparatorChar + _imageProfile.BootImage +
                            " root=/dev/ram0 rw ramdisk_size=156000" +
                            " consoleblank=0" + " web=" + webPath + " USER_TOKEN=" + _userToken + " " +
                            globalComputerArgs +
                            " " + _imageProfile.KernelArguments + newLineChar);

            var grub = new StringBuilder();
            grub.Append("set default=0" + newLineChar);
            grub.Append("set timeout=0" + newLineChar);
            grub.Append("menuentry Theopenem --unrestricted {" + newLineChar);
            grub.Append("echo Please Wait While The Boot Image Is Transferred.  This May Take A Few Minutes." +
                        newLineChar);
            grub.Append("linux /kernels/" + _imageProfile.Kernel +
                        " root=/dev/ram0 rw ramdisk_size=156000" + " consoleblank=0" + " web=" + webPath + " USER_TOKEN=" + _userToken +
                         " " +
                        globalComputerArgs + " " + _imageProfile.KernelArguments + newLineChar);
            grub.Append("initrd /images/" + _imageProfile.BootImage + newLineChar);
            grub.Append("}" + newLineChar);

            var list = new List<Tuple<string, string, string>>
            {
                Tuple.Create("bios", "", sysLinux.ToString()),
                Tuple.Create("bios", ".ipxe", ipxe.ToString()),
                Tuple.Create("efi32", "", sysLinux.ToString()),
                Tuple.Create("efi32", ".ipxe", ipxe.ToString()),
                Tuple.Create("efi64", "", sysLinux.ToString()),
                Tuple.Create("efi64", ".ipxe", ipxe.ToString()),
                Tuple.Create("efi64", ".cfg", grub.ToString())
            };

            //In proxy mode all boot files are created regardless of the pxe mode, this way computers can be customized
            //to use a specific boot file without affecting all others, using the proxydhcp reservations file.
            if (ServiceSetting.GetSettingValue(SettingStrings.ProxyDhcpEnabled) == "Yes")
            {

                foreach (var bootMenu in list)
                {
                    var path = _thisComServer.TftpPath + "proxy" +
                               Path.DirectorySeparatorChar + bootMenu.Item1 +
                               Path.DirectorySeparatorChar + "pxelinux.cfg" + Path.DirectorySeparatorChar +
                               pxeComputerMac +
                               bootMenu.Item2;

                    if (!new FilesystemServices().WritePath(path, bootMenu.Item3))
                        return false;
                }


            }
            //When not using proxy dhcp, only one boot file is created
            else
            {
                var mode = ServiceSetting.GetSettingValue(SettingStrings.PxeBootloader);
                var path = "";

                    path = _thisComServer.TftpPath + "pxelinux.cfg" +
                           Path.DirectorySeparatorChar + pxeComputerMac;

                    string fileContents = null;
                    if (mode == "pxelinux" || mode == "syslinux_32_efi" || mode == "syslinux_64_efi")
                    {
                        fileContents = sysLinux.ToString();
                    }

                    else if (mode.Contains("ipxe"))
                    {
                        path += ".ipxe";
                        fileContents = ipxe.ToString();
                    }
                    else if (mode.Contains("grub"))
                    {
                        path += ".cfg";
                        fileContents = grub.ToString();
                    }

                    if (!new FilesystemServices().WritePath(path, fileContents))
                        return false;
  
            }

            return true;
        }
    }
}
