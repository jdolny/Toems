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
using System.Web;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class MulticastArguments
    {
        private readonly ILog log = LogManager.GetLogger(typeof(MulticastArguments));
        private EntityClientComServer _thisComServer;
        public int RunOnComServer(DtoMulticastArgs mArgs, EntityClientComServer comServer)
        {
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);


            var pid = new APICall().ClientComServerApi.StartUdpSender(comServer.Url, "", decryptedKey, mArgs);
            return pid;
        }

        public int GenerateProcessArguments(DtoMulticastArgs mArgs)
        {

            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            _thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                log.Error($"Com Server With Guid {guid} Not Found");
                return 0;
            }

            var schemaCounter = -1;
            var multicastHdCounter = 0;
            string processArguments = null;
            foreach (var hd in mArgs.schema.HardDrives)
            {
                schemaCounter++;
                if (!hd.Active) continue;
                multicastHdCounter++;

                var x = 0;
                foreach (var part in mArgs.schema.HardDrives[schemaCounter].Partitions)
                {
                    if (!part.Active) continue;
                    string imageFile = null;
                    foreach (var ext in new[] { "ntfs", "fat", "extfs", "hfsp", "imager", "winpe", "xfs" })
                    {
                        imageFile = new FilesystemServices().GetMulticastFileNameWithFullPath(mArgs.ImageName,
                            schemaCounter.ToString(), part.Number, ext,_thisComServer.LocalStoragePath);

                        if (!string.IsNullOrEmpty(imageFile)) break;

                        //Look for lvm
                        if (part.VolumeGroup == null) continue;
                        if (part.VolumeGroup.LogicalVolumes == null) continue;
                        foreach (var lv in part.VolumeGroup.LogicalVolumes.Where(lv => lv.Active))
                        {
                            imageFile = new FilesystemServices().GetMulticastLVMFileNameWithFullPath(mArgs.ImageName,
                                schemaCounter.ToString(), lv.VolumeGroup, lv.Name, ext,_thisComServer.LocalStoragePath);
                        }
                    }

                    if (string.IsNullOrEmpty(imageFile))
                        continue;
                    if (mArgs.Environment == "winpe" &&
                        mArgs.schema.HardDrives[schemaCounter].Table.ToLower() == "gpt")
                    {
                        if (part.Type.ToLower() == "system" || part.Type.ToLower() == "recovery" ||
                            part.Type.ToLower() == "reserved")
                            continue;
                    }
                    if (mArgs.Environment == "winpe" &&
                        mArgs.schema.HardDrives[schemaCounter].Table.ToLower() == "mbr")
                    {
                        if (part.Number == mArgs.schema.HardDrives[schemaCounter].Boot &&
                            mArgs.schema.HardDrives[schemaCounter].Partitions.Length > 1)
                            continue;
                    }
                    x++;

                    var minReceivers = "";

                    if (!string.IsNullOrEmpty(mArgs.clientCount))
                        minReceivers = " --min-receivers " + mArgs.clientCount;

                    var isUnix = Environment.OSVersion.ToString().Contains("Unix");

                    string compAlg;
                    var stdout = "";
                    switch (Path.GetExtension(imageFile))
                    {
                        case ".lz4":
                            compAlg = isUnix ? "lz4 -d " : "lz4.exe\" -d ";
                            stdout = " - ";
                            break;
                        case ".gz":
                            if (isUnix)
                            {
                                compAlg = "gzip -c -d ";
                                stdout = "";
                            }
                            else
                            {
                                compAlg = "7za.exe\" x ";
                                stdout = " -so ";
                            }
                            break;
                        case ".uncp":
                            compAlg = "none";
                            break;
                        case ".wim":
                            compAlg = "none";
                            break;
                        default:
                            return 0;
                    }

                    if (isUnix)
                    {
                        string prefix = null;
                        if (multicastHdCounter == 1)
                            prefix = x == 1 ? " -c \"" : " ; ";
                        else
                            prefix = " ; ";

                        if (compAlg == "none" ||
                            _thisComServer.DecompressImageOn == "client")
                        {
                            processArguments += prefix + "cat " + "\"" + imageFile + "\"" + " | udp-sender" +
                                                " --portbase " + mArgs.Port + minReceivers + " " +
                                                " --ttl 32 --interface " + _thisComServer.MulticastInterfaceIp + " " + mArgs.ExtraArgs;
                        }

                        else
                        {
                            processArguments += prefix + compAlg + "\"" + imageFile + "\"" + stdout + " | udp-sender" +
                                                " --portbase " + mArgs.Port + minReceivers + " " +
                                                " --ttl 32 --interface " + _thisComServer.MulticastInterfaceIp + " " + mArgs.ExtraArgs;
                        }
                    }
                    else
                    {
                        var appPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                                      Path.DirectorySeparatorChar + "apps" + Path.DirectorySeparatorChar;

                        string prefix = null;
                        if (multicastHdCounter == 1)
                        {
                                prefix = x == 1 ? " /c \"" : " & ";
                        }
                        else
                            prefix = " & ";

                        if (compAlg == "none" ||
                            _thisComServer.DecompressImageOn == "client")
                        {
                            processArguments += prefix + "\"" + appPath +
                                                "udp-sender.exe" + "\"" + " --file " + "\"" + imageFile + "\"" +
                                                " --portbase " + mArgs.Port + minReceivers + " " +
                                                " --ttl 32 --interface " + _thisComServer.MulticastInterfaceIp + " " + mArgs.ExtraArgs;
                        }
                        else
                        {
                            processArguments += prefix + "\"" + appPath + compAlg + "\"" + imageFile + "\"" + stdout +
                                                " | " + "\"" + appPath +
                                                "udp-sender.exe" + "\"" +
                                                " --portbase " + mArgs.Port + minReceivers + " " +
                                                " --ttl 32 --interface " + _thisComServer.MulticastInterfaceIp + " " + mArgs.ExtraArgs;
                        }
                    }
                }
            }

            processArguments += "\"";

            return StartMulticastSender(processArguments, mArgs.groupName);
        }

        private int StartMulticastSender(string processArguments, string groupName)
        {
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


            var senderInfo = new ProcessStartInfo { FileName = shell, Arguments = processArguments };

            //Fix
            var logPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                          Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "multicast.log";

            var logText = Environment.NewLine + DateTime.Now.ToString("MM-dd-yy hh:mm") +
                          " Starting Multicast Session " +
                          groupName +
                          " With The Following Command:" + Environment.NewLine + senderInfo.FileName +
                          senderInfo.Arguments
                          + Environment.NewLine;
            File.AppendAllText(logPath, logText);

            Process sender;
            try
            {
                sender = Process.Start(senderInfo);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                File.AppendAllText(logPath,
                    "Could Not Start Session " + groupName + " Try Pasting The Command Into A Command Prompt");
                return 0;
            }

            Thread.Sleep(2000);

            if (sender == null) return 0;

            if (sender.HasExited)
            {
                File.AppendAllText(logPath,
                    "Session " + groupName + " Started And Was Forced To Quit, Try Running The Command Manually");
                return 0;
            }

            return sender.Id;
        }
    }
}
