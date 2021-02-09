using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class IsoGenerator
    {
        private const string NewLineChar = "\n";
        private string _basePath;
        private string _buildPath;
        private string _configOutPath;
        private DtoIsoGenOptions _isoOptions;
        private EntityClientComServer _thisComServer;
        private string _globalComputerArgs;
        private string _rootfsPath;
        private string _outputPath;
        private string _grubPath;
        private string _webPath;
        private string _userToken { get; set; }
        private readonly ILog Logger = LogManager.GetLogger(typeof(IsoGenerator));

        public byte [] RunAllServers(DtoIsoGenOptions isoOptions)
        {
            var uow = new UnitOfWork();
            var tftpComServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);
            EntityClientComServer tftpInfoServer;
            if (tftpComServers.Count == 0)
            {
                Logger.Error("No Tftp Servers Are Currently Enabled To Generate ISO");
                return null;
            }
            if (tftpComServers.Count > 1)
            {
                tftpInfoServer = tftpComServers.Where(x => x.IsTftpInfoServer).FirstOrDefault();
                if (tftpInfoServer == null)
                {
                    Logger.Error("No Tftp Servers Are Currently Set As The Information Server.  Unable To Generate ISO");
                    return null;
                }
            }
            else
                tftpInfoServer = tftpComServers.First();

            //Connect To Client Com Server

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

            var result = new APICall().ClientComServerApi.GenerateISO(tftpInfoServer.Url, "", decryptedKey,isoOptions);

            return result;

        }

        public byte[] Create(DtoIsoGenOptions isoOptions)
        {
            var uow = new UnitOfWork();
            _isoOptions = isoOptions;
            var mode = ServiceSetting.GetSettingValue(SettingStrings.PxeBootloader);
            var imageServers = new List<DtoClientComServers>();
            var defaultCluster = uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
            if(isoOptions.clusterId == -1)
            {
               imageServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id);
            }
            else
            {
                imageServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(isoOptions.clusterId);
            }

            if(imageServers == null)
            {
                Logger.Error($"No Image Servers Found For This Cluster");
                return null;
            }

            if (imageServers.Count == 0)
            {
                Logger.Error($"No Image Servers Found For This Cluster");
                return null;
            }

            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            _thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                Logger.Error($"Com Server With Guid {guid} Not Found");
                return null;
            }

            var webRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.WebTasksRequireLogin);
            var consoleRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);
            var globalToken = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingToken);
            if (webRequiresLogin.Equals("False") || consoleRequiresLogin.Equals("False"))
                _userToken = globalToken;
            else
                _userToken = "";

            _globalComputerArgs = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingArguments);

            _webPath = "\"";
            foreach (var imageServer in imageServers)
            {
                var url = new ServiceClientComServer().GetServer(imageServer.ComServerId).Url;
                _webPath += url + "clientimaging/ "; //adds a space delimiter
            }
            _webPath = _webPath.Trim(' ');
            _webPath += "\"";

            _basePath = HttpContext.Current.Server.MapPath("~") + "private" +
                      Path.DirectorySeparatorChar;
            _rootfsPath = _basePath + "client_iso" + Path.DirectorySeparatorChar + "rootfs" +
                          Path.DirectorySeparatorChar;
            if(isoOptions.useSecureBoot)
                _grubPath = _basePath + "client_iso" + Path.DirectorySeparatorChar + "grub_binaries" +
                          Path.DirectorySeparatorChar + "signed" + Path.DirectorySeparatorChar;
            else
                _grubPath = _basePath + "client_iso" + Path.DirectorySeparatorChar + "grub_binaries" +
                         Path.DirectorySeparatorChar + "unsigned" + Path.DirectorySeparatorChar;
            _buildPath = _basePath + "client_iso" + Path.DirectorySeparatorChar + "build-tmp";
            _outputPath = _basePath + "client_iso" + Path.DirectorySeparatorChar;
            _configOutPath = _basePath + "client_iso" + Path.DirectorySeparatorChar + "config" +
                             Path.DirectorySeparatorChar;

            Generate();

            var file = File.ReadAllBytes(_basePath + "client_iso" + Path.DirectorySeparatorChar + "clientboot.iso");
           
            return file;
        }

        private void CleanBuildPath()
        {
            if (Directory.Exists(_buildPath))
            {
                Directory.Delete(_buildPath, true);
            }
        }


        private void CreateGrubMenu()
        {
            var grubMenu = new StringBuilder();

            grubMenu.Append("# Global options" + NewLineChar);
            grubMenu.Append("set timeout=-1" + NewLineChar);
            grubMenu.Append("set default=0" + NewLineChar);
            grubMenu.Append("set fallback=1" + NewLineChar);
            grubMenu.Append("set pager=1" + NewLineChar);

            grubMenu.Append("if loadfont /EFI/boot/unicode.pf2; then" + NewLineChar);
            grubMenu.Append("insmod efi_gop" + NewLineChar);
            grubMenu.Append("insmod efi_uga" + NewLineChar);
            grubMenu.Append("insmod video_bochs" + NewLineChar);
            grubMenu.Append("insmod video_cirrus" + NewLineChar);
            grubMenu.Append("insmod all_video" + NewLineChar);
            grubMenu.Append("fi" + NewLineChar);

            grubMenu.Append("" + NewLineChar);
            grubMenu.Append("menuentry \"Boot To Local Machine\" {" + NewLineChar);
            grubMenu.Append("exit" + NewLineChar);
            grubMenu.Append("}" + NewLineChar);
            grubMenu.Append("" + NewLineChar);

            grubMenu.Append("menuentry \"Theopenem\" {" + NewLineChar);
            grubMenu.Append("set gfxpayload=keep" + NewLineChar);
            grubMenu.Append("linux	/theopenem/" + _isoOptions.kernel + " ramdisk_size=156000 root=/dev/ram0 rw web=" +
                            _webPath +
                            " USER_TOKEN=" + _userToken + " consoleblank=0 " + _isoOptions.arguments +
                            NewLineChar);
            grubMenu.Append("initrd	/theopenem/" + _isoOptions.bootImage + NewLineChar);
            grubMenu.Append("}" + NewLineChar);
            grubMenu.Append("" + NewLineChar);

            grubMenu.Append("menuentry \"Client Console\" {" + NewLineChar);
            grubMenu.Append("set gfxpayload=keep" + NewLineChar);
            grubMenu.Append("linux	/theopenem/" + _isoOptions.kernel + " ramdisk_size=156000 root=/dev/ram0 rw web=" +
                            _webPath +
                            " USER_TOKEN=" + _userToken + " task=debug consoleblank=0 " + _isoOptions.arguments +
                            NewLineChar);
            grubMenu.Append("initrd	/theopenem/" + _isoOptions.bootImage + NewLineChar);
            grubMenu.Append("}" + NewLineChar);
            grubMenu.Append("" + NewLineChar);



            var outFile = _configOutPath + "EFI" + Path.DirectorySeparatorChar + "boot" + Path.DirectorySeparatorChar +
                          "grub.cfg";

            new FilesystemServices().WritePath(outFile, grubMenu.ToString());
        }

        private void CreateSyslinuxMenu()
        {
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
            sysLinuxMenu.Append("MENU DEFAULT" + NewLineChar);
            sysLinuxMenu.Append("MENU LABEL Boot To Local Machine" + NewLineChar);
            sysLinuxMenu.Append("" + NewLineChar);


            sysLinuxMenu.Append("LABEL Theopenem" + NewLineChar);
            sysLinuxMenu.Append("kernel /theopenem/" + _isoOptions.kernel + "" + NewLineChar);
            sysLinuxMenu.Append("append initrd=/theopenem/" + _isoOptions.bootImage +
                                " root=/dev/ram0 rw ramdisk_size=156000 " + " web=" + _webPath + " USER_TOKEN=" +
                                _userToken +
                                " consoleblank=0 " + _isoOptions.arguments + "" + NewLineChar);
            sysLinuxMenu.Append("MENU LABEL Theopenem" + NewLineChar);
            sysLinuxMenu.Append("" + NewLineChar);


            sysLinuxMenu.Append("LABEL Client Console" + NewLineChar);
            sysLinuxMenu.Append("kernel /theopenem/" + _isoOptions.kernel + "" + NewLineChar);
            sysLinuxMenu.Append("append initrd=/theopenem/" + _isoOptions.bootImage +
                                " root=/dev/ram0 rw ramdisk_size=156000 " + " web=" + _webPath + " USER_TOKEN=" +
                                _userToken +
                                " task=debug consoleblank=0 " + _isoOptions.arguments + "" + NewLineChar);
            sysLinuxMenu.Append("MENU LABEL Client Console" + NewLineChar);
            sysLinuxMenu.Append("" + NewLineChar);

            sysLinuxMenu.Append("PROMPT 0" + NewLineChar);
            sysLinuxMenu.Append("TIMEOUT 0" + NewLineChar);

            string outFile;
                outFile = _configOutPath + "syslinux" + Path.DirectorySeparatorChar + "isolinux.cfg";


            new FilesystemServices().WritePath(outFile, sysLinuxMenu.ToString());
        }

      

        public bool Generate()
        {
            try
            {
                if (Directory.Exists(_configOutPath))
                    Directory.Delete(_configOutPath, true);
                Directory.CreateDirectory(_configOutPath);
                Directory.CreateDirectory(_configOutPath + "theopenem");
                Directory.CreateDirectory(_configOutPath + "EFI");
                Directory.CreateDirectory(_configOutPath + "EFI" + Path.DirectorySeparatorChar + "boot");
                Directory.CreateDirectory(_configOutPath + "syslinux");
                File.Copy(
                    _thisComServer.TftpPath + "images" + Path.DirectorySeparatorChar +
                    _isoOptions.bootImage,
                    _configOutPath + "theopenem" + Path.DirectorySeparatorChar + _isoOptions.bootImage, true);
                File.Copy(
                    _thisComServer.TftpPath + "kernels" + Path.DirectorySeparatorChar +
                    _isoOptions.kernel,
                    _configOutPath + "theopenem" + Path.DirectorySeparatorChar + _isoOptions.kernel, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }

          
                CreateSyslinuxMenu();
                CreateGrubMenu();
                StartMkIsofs();
           

            return true;
        }

        private string GenerateArgs()
        {
            var logPath = _basePath + "logs" + Path.DirectorySeparatorChar + "mkisofs.log";

            string arguments;
            var isUnix = Environment.OSVersion.ToString().Contains("Unix");
            if (isUnix)
            {
                arguments = "-c \"mkisofs -o " + "\"" + _outputPath + "\"" +
                            " -log-file \"" + logPath + "\"" +
                            " -graft-points -joliet -R -full-iso9660-filenames -allow-lowercase" +
                            " -b syslinux/isolinux.bin -c syslinux/boot.cat -no-emul-boot -boot-load-size 4 -boot-info-table" +
                            " -eltorito-alt-boot -e EFI/boot/efiboot.img -no-emul-boot " + _buildPath + "\"";
            }
            else
            {
                var appPath = _basePath + "apps" + Path.DirectorySeparatorChar + "mkisofs.exe";
                arguments = "/c \"cd /d " + _buildPath + " & " + " \"" + appPath + "\"" + " -o " + "\"" + _outputPath +
                            "\"" +
                            " -log-file \"" + logPath + "\"" +
                            " -graft-points -joliet -R -full-iso9660-filenames -allow-lowercase" +
                            " -b syslinux/isolinux.bin -c syslinux/boot.cat -no-emul-boot -boot-load-size 4 -boot-info-table" +
                            " -eltorito-alt-boot -eltorito-platform 0xEF -eltorito-boot EFI/boot/efiboot.img -no-emul-boot . " +
                            "\"";
            }
            return arguments;
        }

        private bool StartMkIsofs()
        {
            _outputPath += "clientboot.iso";
            try
            {
                //delete existing file
                File.Delete(_outputPath);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
            }
            //copy base root path to temporary location
            new FilesystemServices().Copy(_rootfsPath, _buildPath);
            //copy correct grub binaries to build path
            new FilesystemServices().Copy(_grubPath, _buildPath + Path.DirectorySeparatorChar + "EFI" + Path.DirectorySeparatorChar + "boot");
            //copy newly generated config files on top of temporary location
            new FilesystemServices().Copy(_configOutPath, _buildPath);

            var shell = "";
            if (Environment.OSVersion.ToString().Contains("Unix"))
            {
                string dist = null;
                var distInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = "uname",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(distInfo))
                    if (process != null) dist = process.StandardOutput.ReadToEnd();

                var unixDist = dist != null && dist.ToLower().Contains("bsd") ? "bsd" : "linux";
                shell = unixDist == "bsd" ? "/bin/tcsh" : "/bin/bash";
            }
            else
            {
                shell = "cmd.exe";
            }


            var processArguments = GenerateArgs();
            if (processArguments == null) return false;
            var pInfo = new ProcessStartInfo { FileName = shell, Arguments = processArguments };

            Process makeIso;
            try
            {
                makeIso = Process.Start(pInfo);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return false;
            }

            if (makeIso == null)
            {
                CleanBuildPath();
                return false;
            }

            makeIso.WaitForExit(15000);

            CleanBuildPath();
            return true;
        }


    }
}
