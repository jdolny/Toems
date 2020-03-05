using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class DefaultBootMenu
    {
        private const string NewLineChar = "\n";
        private readonly Regex _alphaNumericNoSpace = new Regex("[^a-zA-Z0-9]");
        private readonly Regex _alphaNumericSpace = new Regex("[^a-zA-Z0-9 ]");
        private ServiceCustomBootMenu _bootEntryServices;
        private DtoBootMenuGenOptions _defaultBoot;
        private EntityClientComServer _thisComServer;
        private string _globalComputerArgs;
        private string _webPath;
        private string _userToken { get; set; }
        private readonly ILog log = LogManager.GetLogger(typeof(DefaultBootMenu));

        public bool RunAllServers(DtoBootMenuGenOptions defaultBootMenu)
        {
            _defaultBoot = defaultBootMenu;


            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
            var NoErrors = true;
            foreach (var com in comServers)
            {
                if (!new APICall().ClientComServerApi.CreateDefaultBootMenu(com.Url, "", decryptedKey, defaultBootMenu))
                    NoErrors = false;
            }

            return NoErrors;

        }

        public bool Create(DtoBootMenuGenOptions bootOptions)
        {
            _defaultBoot = bootOptions;
            var mode = ServiceSetting.GetSettingValue(SettingStrings.PxeBootloader);

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

            _bootEntryServices = new ServiceCustomBootMenu();
            _globalComputerArgs = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingArguments);

            var defaultCluster = new UnitOfWork().ComServerClusterRepository.Get(x => x.IsDefault).FirstOrDefault();
            var defaultImagingServers = new UnitOfWork().ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id);

            _webPath = "\"";
            foreach (var imageServer in defaultImagingServers)
            {
                var url = new ServiceClientComServer().GetServer(imageServer.ComServerId).Url;
                _webPath += url + "clientimaging/ "; //adds a space delimiter
            }
            _webPath = _webPath.Trim(' ');
            _webPath += "\"";

            var webRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.WebTasksRequireLogin);
            var consoleRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);
            var globalToken = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingToken);
            if (webRequiresLogin.Equals("False") || consoleRequiresLogin.Equals("False"))
                _userToken = globalToken;
            else
                _userToken = "";

            if (_defaultBoot.Type == "standard")
            {
                if (mode.Contains("ipxe"))
                    CreateIpxeMenu(defaultCluster.Id);
                else if (mode.Contains("grub"))
                    CreateGrubMenu();
                else
                    CreateSyslinuxMenu();
            }
            else
            {
                foreach (var proxyMode in new[] { "bios", "efi32", "efi64" })
                {
                    bootOptions.Type = proxyMode;
                    if(proxyMode.Equals("bios"))
                    {
                        bootOptions.Kernel = bootOptions.BiosKernel;
                        bootOptions.BootImage = bootOptions.BiosBootImage;
                    }
                    else if(proxyMode.Equals("efi32"))
                    {
                        bootOptions.Kernel = bootOptions.Efi32Kernel;
                        bootOptions.BootImage = bootOptions.Efi32BootImage;
                    }
                    else
                    {
                        bootOptions.Kernel = bootOptions.Efi64Kernel;
                        bootOptions.BootImage = bootOptions.Efi64BootImage;
                    }
                    CreateIpxeMenu(defaultCluster.Id);
                    CreateSyslinuxMenu();
                    CreateGrubMenu();
                }
            }

            return true;
        }


        private void CreateGrubMenu()
        {
            var customMenuEntries =
                _bootEntryServices.GetAll().Where(x => x.Type == "grub" && x.IsActive).OrderBy(x => x.Order).ThenBy(x => x.Name);
            var defaultCustomEntry = customMenuEntries.FirstOrDefault(x => x.IsDefault);

            var grubMenu = new StringBuilder();

            grubMenu.Append("insmod password_pbkdf2" + NewLineChar);
            grubMenu.Append("insmod regexp" + NewLineChar);
            grubMenu.Append("set default=0" + NewLineChar);
            grubMenu.Append("set timeout=10" + NewLineChar);
            grubMenu.Append("set pager=1" + NewLineChar);
            /*if (!string.IsNullOrEmpty(_defaultBoot.GrubUserName) && !string.IsNullOrEmpty(_defaultBoot.GrubPassword))
            {
                grubMenu.Append("set superusers=\"" + _defaultBoot.GrubUserName + "\"" + NewLineChar);
                string sha = null;
                try
                {
                    sha =
                        new WebClient().DownloadString(
                            "http://docs.clonedeploy.org/grub-pass-gen/encrypt.php?password=" +
                            _defaultBoot.GrubPassword);
                    sha = sha.Replace("\n \n\n\n", "");
                }
                catch
                {
                    log.Error("Could not generate sha for grub password.  Could not contact http://clonedeploy.org");
                }
                grubMenu.Append("password_pbkdf2 " + _defaultBoot.GrubUserName + " " + sha + "" + NewLineChar);
                grubMenu.Append("export superusers" + NewLineChar);
                grubMenu.Append("" + NewLineChar);
            }*/
            grubMenu.Append(@"regexp -s 1:b1 '(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3})' $net_default_mac" +
                            NewLineChar);
            grubMenu.Append(@"regexp -s 2:b2 '(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3})' $net_default_mac" +
                            NewLineChar);
            grubMenu.Append(@"regexp -s 3:b3 '(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3})' $net_default_mac" +
                            NewLineChar);
            grubMenu.Append(@"regexp -s 4:b4 '(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3})' $net_default_mac" +
                            NewLineChar);
            grubMenu.Append(@"regexp -s 5:b5 '(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3})' $net_default_mac" +
                            NewLineChar);
            grubMenu.Append(@"regexp -s 6:b6 '(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3}):(.{1,3})' $net_default_mac" +
                            NewLineChar);
            grubMenu.Append(@"mac=01-$b1-$b2-$b3-$b4-$b5-$b6" + NewLineChar);
            grubMenu.Append("" + NewLineChar);

            if (_defaultBoot.Type == "standard")
            {
                grubMenu.Append("if [ -s /pxelinux.cfg/$mac.cfg ]; then" + NewLineChar);
                grubMenu.Append("configfile /pxelinux.cfg/$mac.cfg" + NewLineChar);
                grubMenu.Append("fi" + NewLineChar);
            }
            else
            {
                grubMenu.Append("if [ -s /proxy/efi64/pxelinux.cfg/$mac.cfg ]; then" + NewLineChar);
                grubMenu.Append("configfile /proxy/efi64/pxelinux.cfg/$mac.cfg" + NewLineChar);
                grubMenu.Append("fi" + NewLineChar);
            }

            if (defaultCustomEntry != null)
            {
                grubMenu.Append("" + NewLineChar);
                grubMenu.Append("menuentry \"" + _alphaNumericSpace.Replace(defaultCustomEntry.Name, "") +
                                "\" --unrestricted {" + NewLineChar);
                grubMenu.Append(defaultCustomEntry.Content + NewLineChar);
                grubMenu.Append("}" + NewLineChar);
            }

            grubMenu.Append("" + NewLineChar);
            grubMenu.Append("menuentry \"Boot To Local Machine\" --unrestricted {" + NewLineChar);
            grubMenu.Append("exit" + NewLineChar);
            grubMenu.Append("}" + NewLineChar);
            grubMenu.Append("" + NewLineChar);

            grubMenu.Append("menuentry \"Theopenem\" --user {" + NewLineChar);
            grubMenu.Append("echo Please Wait While The Boot Image Is Transferred.  This May Take A Few Minutes." +
                            NewLineChar);
            grubMenu.Append("linux /kernels/" + _defaultBoot.Kernel + " root=/dev/ram0 rw ramdisk_size=156000 " +
                            " web=" +
                            _webPath +
                            " USER_TOKEN=" + _userToken + " consoleblank=0 " + _globalComputerArgs + "" +
                            NewLineChar);
            grubMenu.Append("initrd /images/" + _defaultBoot.BootImage + "" + NewLineChar);
            grubMenu.Append("}" + NewLineChar);
            grubMenu.Append("" + NewLineChar);

            grubMenu.Append("menuentry \"Client Console\" --user {" + NewLineChar);
            grubMenu.Append("echo Please Wait While The Boot Image Is Transferred.  This May Take A Few Minutes." +
                            NewLineChar);
            grubMenu.Append("linux /kernels/" + _defaultBoot.Kernel + " root=/dev/ram0 rw ramdisk_size=156000 " +
                            " web=" +
                            _webPath +
                            " USER_TOKEN=" + _userToken + " task=debug consoleblank=0 " + _globalComputerArgs + "" +
                            NewLineChar);
            grubMenu.Append("initrd /images/" + _defaultBoot.BootImage + "" + NewLineChar);
            grubMenu.Append("}" + NewLineChar);
            grubMenu.Append("" + NewLineChar);

            foreach (var customEntry in customMenuEntries)
            {
                if (defaultCustomEntry != null && customEntry.Id == defaultCustomEntry.Id)
                    continue;

                grubMenu.Append("" + NewLineChar);
                grubMenu.Append("menuentry \"" + _alphaNumericSpace.Replace(customEntry.Name, "") +
                                "\" --user {" + NewLineChar);
                grubMenu.Append(customEntry.Content + NewLineChar);
                grubMenu.Append("}" + NewLineChar);

                grubMenu.Append("" + NewLineChar);
            }

            var path = _thisComServer.TftpPath + "grub" + Path.DirectorySeparatorChar +
                       "grub.cfg";


            new FilesystemServices().WritePath(path, grubMenu.ToString());
          
        }

        private void CreateIpxeMenu(int defaultClusterId)
        {
            var defaultTftpServers = new UnitOfWork().ComServerClusterServerRepository.GetTftpClusterServers(defaultClusterId);
            var iPxePath = defaultTftpServers.First().Url; //just use the first tftpserver for the ipxe kernel transfer
            if (iPxePath.Contains("https://")) 
            {
                if (ServiceSetting.GetSettingValue(SettingStrings.IpxeSSL).Equals("False"))
                {
                    iPxePath = iPxePath.ToLower().Replace("https://", "http://");
                    var currentPort = iPxePath.Split(':').Last();
                    iPxePath = iPxePath.Replace(currentPort, ServiceSetting.GetSettingValue(SettingStrings.IpxeHttpPort)) + "/clientimaging/";
                }
            }

         
            var customMenuEntries =
                _bootEntryServices.GetAll()
                    .Where(x => x.Type == "ipxe" && x.IsActive)
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Name);
            var defaultCustomEntry = customMenuEntries.FirstOrDefault(x => x.IsDefault);

            var ipxeMenu = new StringBuilder();

            ipxeMenu.Append("#!ipxe" + NewLineChar);
            ipxeMenu.Append("chain 01-${net0/mac:hexhyp}.ipxe || chain 01-${net1/mac:hexhyp}.ipxe || goto Menu" +
                            NewLineChar);
            ipxeMenu.Append("" + NewLineChar);
            ipxeMenu.Append(":Menu" + NewLineChar);
            ipxeMenu.Append("menu Boot Menu" + NewLineChar);
            ipxeMenu.Append("item bootLocal Boot To Local Machine" + NewLineChar);
            ipxeMenu.Append("item theopenem Theopenem" + NewLineChar);
            ipxeMenu.Append("item console Client Console" + NewLineChar);

            foreach (var customEntry in customMenuEntries)
            {
                ipxeMenu.Append("item " + _alphaNumericNoSpace.Replace(customEntry.Name, "") + " " +
                                _alphaNumericSpace.Replace(customEntry.Name, "") + NewLineChar);
            }
            if (defaultCustomEntry == null)
                ipxeMenu.Append("choose --default bootLocal --timeout 5000 target && goto ${target}" + NewLineChar);
            else
            {
                ipxeMenu.Append("choose --default " + _alphaNumericNoSpace.Replace(defaultCustomEntry.Name, "") +
                                " --timeout 5000 target && goto ${target}" + NewLineChar);
            }
            ipxeMenu.Append("" + NewLineChar);

            if (ServiceSetting.GetSettingValue(SettingStrings.IpxeRequiresLogin) == "True")
            {
                ipxeMenu.Append(":bootLocal" + NewLineChar);
                ipxeMenu.Append("exit" + NewLineChar);
                ipxeMenu.Append("" + NewLineChar);

                ipxeMenu.Append(":console" + NewLineChar);
                ipxeMenu.Append("set task debug" + NewLineChar);
                ipxeMenu.Append("goto login" + NewLineChar);
                ipxeMenu.Append("" + NewLineChar);



                ipxeMenu.Append(":theopenem" + NewLineChar);
                ipxeMenu.Append("set task ond" + NewLineChar);
                ipxeMenu.Append("goto login" + NewLineChar);
                ipxeMenu.Append("" + NewLineChar);


                ipxeMenu.Append(":login" + NewLineChar);
                ipxeMenu.Append("login" + NewLineChar);
                ipxeMenu.Append("params" + NewLineChar);
                ipxeMenu.Append("param uname ${username:uristring}" + NewLineChar);
                ipxeMenu.Append("param pwd ${password:uristring}" + NewLineChar);
                ipxeMenu.Append("param kernel " + _defaultBoot.Kernel + "" + NewLineChar);
                ipxeMenu.Append("param bootImage " + _defaultBoot.BootImage + "" + NewLineChar);
                ipxeMenu.Append("param task " + "${task}" + "" + NewLineChar);
                ipxeMenu.Append("echo Authenticating" + NewLineChar);
                ipxeMenu.Append("chain --timeout 15000 " + iPxePath +
                                "IpxeLogin##params || goto Menu" +
                                NewLineChar);
            }
            else
            {
                ipxeMenu.Append(":bootLocal" + NewLineChar);
                ipxeMenu.Append("exit" + NewLineChar);
                ipxeMenu.Append("" + NewLineChar);

                ipxeMenu.Append(":theopenem" + NewLineChar);
                ipxeMenu.Append("kernel " + iPxePath +
                                "IpxeBoot?filename=" + _defaultBoot.Kernel +
                                "&type=kernel" +
                                " initrd=" + _defaultBoot.BootImage + " root=/dev/ram0 rw ramdisk_size=156000 " +
                                " web=" +
                                _webPath + " USER_TOKEN=" + _userToken +
                                " consoleblank=0 " +
                                _globalComputerArgs + NewLineChar);
                ipxeMenu.Append("imgfetch --name " + _defaultBoot.BootImage + " " +
                                iPxePath +
                                "IpxeBoot?filename=" +
                                _defaultBoot.BootImage + "&type=bootimage" + NewLineChar);
                ipxeMenu.Append("boot" + NewLineChar);
                ipxeMenu.Append("" + NewLineChar);

                ipxeMenu.Append(":console" + NewLineChar);
                ipxeMenu.Append("kernel " + iPxePath +
                                "IpxeBoot?filename=" + _defaultBoot.Kernel +
                                "&type=kernel" +
                                " initrd=" + _defaultBoot.BootImage + " root=/dev/ram0 rw ramdisk_size=156000 " +
                                " web=" +
                                _webPath + " USER_TOKEN=" + _userToken +
                                " task=debug" + " consoleblank=0 " +
                                _globalComputerArgs + NewLineChar);
                ipxeMenu.Append("imgfetch --name " + _defaultBoot.BootImage + " " +
                                iPxePath +
                                "IpxeBoot?filename=" +
                                _defaultBoot.BootImage + "&type=bootimage" + NewLineChar);
                ipxeMenu.Append("boot" + NewLineChar);
                ipxeMenu.Append("" + NewLineChar);






            }

            //Set Custom Menu Entries
            foreach (var customEntry in customMenuEntries)
            {
                ipxeMenu.Append(":" + _alphaNumericNoSpace.Replace(customEntry.Name, "") + NewLineChar);
                ipxeMenu.Append(customEntry.Content + NewLineChar);
            }

            string path;
            if (_defaultBoot.Type == "standard")
                path = _thisComServer.TftpPath + "pxelinux.cfg" +
                       Path.DirectorySeparatorChar + "default.ipxe";
            else
                path = _thisComServer.TftpPath + "proxy" + Path.DirectorySeparatorChar +
                       _defaultBoot.Type +
                       Path.DirectorySeparatorChar + "pxelinux.cfg" + Path.DirectorySeparatorChar + "default.ipxe";


                new FilesystemServices().WritePath(path, ipxeMenu.ToString());
           
        }

        private void CreateSyslinuxMenu()
        {
            var customMenuEntries =
                _bootEntryServices.GetAll()
                    .Where(x => x.Type == "syslinux/pxelinux" && x.IsActive)
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Name);
            var defaultCustomEntry = customMenuEntries.FirstOrDefault(x => x.IsDefault);
            var sysLinuxMenu = new StringBuilder();

            sysLinuxMenu.Append("DEFAULT vesamenu.c32" + NewLineChar);
            sysLinuxMenu.Append("MENU TITLE Boot Menu" + NewLineChar);
            sysLinuxMenu.Append("MENU BACKGROUND bg.png" + NewLineChar);
            sysLinuxMenu.Append("menu tabmsgrow 22" + NewLineChar);
            sysLinuxMenu.Append("menu cmdlinerow 22" + NewLineChar);
            sysLinuxMenu.Append("menu endrow 24" + NewLineChar);
            sysLinuxMenu.Append("menu color title                1;34;49    #eea0a0ff #cc333355 std" + NewLineChar);
            sysLinuxMenu.Append("menu color sel                  7;37;40    #ff000000 #bb9999aa all" + NewLineChar);
            sysLinuxMenu.Append("menu color border               30;44      #ffffffff #00000000 std" + NewLineChar);
            sysLinuxMenu.Append("menu color pwdheader            31;47      #eeff1010 #20ffffff std" + NewLineChar);
            sysLinuxMenu.Append("menu color hotkey               35;40      #90ffff00 #00000000 std" + NewLineChar);
            sysLinuxMenu.Append("menu color hotsel               35;40      #90000000 #bb9999aa all" + NewLineChar);
            sysLinuxMenu.Append("menu color timeout_msg          35;40      #90ffffff #00000000 none" + NewLineChar);
            sysLinuxMenu.Append("menu color timeout              31;47      #eeff1010 #00000000 none" + NewLineChar);
            sysLinuxMenu.Append("NOESCAPE 0" + NewLineChar);
            sysLinuxMenu.Append("ALLOWOPTIONS 0" + NewLineChar);
            sysLinuxMenu.Append("" + NewLineChar);
            sysLinuxMenu.Append("LABEL local" + NewLineChar);
            sysLinuxMenu.Append("localboot 0" + NewLineChar);
            if (defaultCustomEntry == null)
                sysLinuxMenu.Append("MENU DEFAULT" + NewLineChar);
            sysLinuxMenu.Append("MENU LABEL Boot To Local Machine" + NewLineChar);
            sysLinuxMenu.Append("" + NewLineChar);

            sysLinuxMenu.Append("LABEL Theopenem" + NewLineChar);
            if (!string.IsNullOrEmpty(_defaultBoot.OndPwd) && _defaultBoot.OndPwd != "Error: Empty password")
                sysLinuxMenu.Append("MENU PASSWD " + _defaultBoot.OndPwd + "" + NewLineChar);
            sysLinuxMenu.Append("kernel kernels" + Path.DirectorySeparatorChar + _defaultBoot.Kernel + "" + NewLineChar);
            sysLinuxMenu.Append("append initrd=images" + Path.DirectorySeparatorChar + _defaultBoot.BootImage +
                                " root=/dev/ram0 rw ramdisk_size=156000 " + " web=" + _webPath + " USER_TOKEN=" +
                                _userToken +
                                " consoleblank=0 " + _globalComputerArgs + "" + NewLineChar);
            sysLinuxMenu.Append("MENU LABEL Theopenem" + NewLineChar);
            sysLinuxMenu.Append("" + NewLineChar);

            sysLinuxMenu.Append("LABEL Client Console" + NewLineChar);
            if (!string.IsNullOrEmpty(_defaultBoot.DebugPwd) && _defaultBoot.DebugPwd != "Error: Empty password")
                sysLinuxMenu.Append("MENU PASSWD " + _defaultBoot.DebugPwd + "" + NewLineChar);
            sysLinuxMenu.Append("kernel kernels" + Path.DirectorySeparatorChar + _defaultBoot.Kernel + "" + NewLineChar);
            sysLinuxMenu.Append("append initrd=images" + Path.DirectorySeparatorChar + _defaultBoot.BootImage +
                                " root=/dev/ram0 rw ramdisk_size=156000 " + " web=" + _webPath + " USER_TOKEN=" +
                                _userToken +
                                " task=debug consoleblank=0 " + _globalComputerArgs + "" + NewLineChar);
            sysLinuxMenu.Append("MENU LABEL Client Console" + NewLineChar);
            sysLinuxMenu.Append("" + NewLineChar);







            //Insert active custom boot menu entries
            foreach (var customEntry in customMenuEntries)
            {
                sysLinuxMenu.Append("LABEL " + _alphaNumericSpace.Replace(customEntry.Name, "") + NewLineChar);
                sysLinuxMenu.Append(customEntry.Content + NewLineChar);
                if (defaultCustomEntry != null && customEntry.Id == defaultCustomEntry.Id)
                    sysLinuxMenu.Append("MENU DEFAULT" + NewLineChar);
                sysLinuxMenu.Append("MENU LABEL " + _alphaNumericSpace.Replace(customEntry.Name, "") + NewLineChar);
                sysLinuxMenu.Append("" + NewLineChar);
            }

            sysLinuxMenu.Append("PROMPT 0" + NewLineChar);
            sysLinuxMenu.Append("TIMEOUT 50" + NewLineChar);

            string path;
            if (_defaultBoot.Type == "standard")
                path = _thisComServer.TftpPath + "pxelinux.cfg" +
                       Path.DirectorySeparatorChar + "default";
            else
                path = _thisComServer.TftpPath + "proxy" + Path.DirectorySeparatorChar +
                       _defaultBoot.Type +
                       Path.DirectorySeparatorChar + "pxelinux.cfg" + Path.DirectorySeparatorChar + "default";


            new FilesystemServices().WritePath(path, sysLinuxMenu.ToString());
           
        }
    }
}
