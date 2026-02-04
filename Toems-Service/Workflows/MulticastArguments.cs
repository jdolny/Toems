using log4net;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Toems_Service;
using Toems_Common;
using System.Net;
using Toems_ApiCalls;

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
            return 0;

        var storageRoot = Path.GetFullPath(_thisComServer.LocalStoragePath);
        var appPath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "private", "apps");

        if (!Directory.Exists(storageRoot) || !Directory.Exists(appPath))
            return 0;

        PathUtils.ValidateFileName(mArgs.ImageName);

        if (string.IsNullOrWhiteSpace(_thisComServer.MulticastInterfaceIp) || !IPAddress.TryParse(_thisComServer.MulticastInterfaceIp, out _))
        {
            log.Error($"Invalid MulticastInterfaceIp: '{_thisComServer.MulticastInterfaceIp}' - must be a valid IP address");
            return 0;
        }


        var processArguments = string.Empty;
        var schemaCounter = -1;
        var multicastHdCounter = 0;

        foreach (var hd in mArgs.schema.HardDrives)
        {
            schemaCounter++;
            if (!hd.Active) continue;
            multicastHdCounter++;

            var x = 0;
            foreach (var part in hd.Partitions)
            {
                if (!part.Active) continue;

                string imageFile = null;

                foreach (var ext in new[] { "ntfs", "fat", "extfs", "hfsp", "imager", "winpe", "xfs" })
                {
                    imageFile = new FilesystemServices()
                        .GetMulticastFileNameWithFullPath(mArgs.ImageName, schemaCounter.ToString(), part.Number, ext, storageRoot);

                    if (!string.IsNullOrEmpty(imageFile))
                        break;

                    if (part.VolumeGroup?.LogicalVolumes == null) continue;

                    foreach (var lv in part.VolumeGroup.LogicalVolumes)
                    {
                        if (!lv.Active) continue;

                        imageFile = new FilesystemServices()
                            .GetMulticastLVMFileNameWithFullPath(mArgs.ImageName, schemaCounter.ToString(), lv.VolumeGroup, lv.Name, ext, storageRoot);

                        if (!string.IsNullOrEmpty(imageFile))
                            break;
                    }
                }

                if (string.IsNullOrEmpty(imageFile))
                    continue;

                imageFile = Path.GetFullPath(imageFile);

                PathUtils.ValidatePathIsUnderRoot(storageRoot, imageFile);
                PathUtils.EnsureNoReparsePoints(storageRoot);

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

                string compAlg;
                string stdout;

                switch (Path.GetExtension(imageFile).ToLowerInvariant())
                {
                    case ".lz4":
                        compAlg = "lz4.exe\" -d ";
                        stdout = " - ";
                        break;
                    case ".gz":
                        compAlg = "7za.exe\" x ";
                        stdout = " -so ";
                        break;
                    case ".uncp":
                    case ".wim":
                        compAlg = "none";
                        stdout = "";
                        break;
                    default:
                        return 0;
                }

                var minReceivers = string.IsNullOrEmpty(mArgs.clientCount)
                    ? ""
                    : " --min-receivers " + int.Parse(mArgs.clientCount);

                string prefix;
                if (multicastHdCounter == 1)
                    prefix = x == 1 ? " /c \"" : " & ";
                else
                    prefix = " & ";

                var senderExe = PathUtils.Quote(Path.Combine(appPath, "udp-sender.exe"));

                if (compAlg == "none" || _thisComServer.DecompressImageOn == "client")
                {
                    processArguments += prefix +
                        $"{senderExe} --file {PathUtils.Quote(imageFile)} " +
                        $"--portbase {int.Parse(mArgs.Port)}{minReceivers} " +
                        $"--ttl 32 --interface {_thisComServer.MulticastInterfaceIp} ";
                }
                else
                {
                    processArguments += prefix +
                        $"\"{Path.Combine(appPath, compAlg)}{PathUtils.Quote(imageFile)}{stdout}\" | " +
                        $"{senderExe} --portbase {int.Parse(mArgs.Port)}{minReceivers} " +
                        $"--ttl 32 --interface {_thisComServer.MulticastInterfaceIp} ";
                }
            }
        }

        processArguments += "\"";
        return StartMulticastSender(processArguments, mArgs.groupName);
    }

    private int StartMulticastSender(string processArguments, string groupName)
    {
        var logPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                         Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "multicast.log";

        var logText = Environment.NewLine + DateTime.Now.ToString("MM-dd-yy hh:mm") +
                      " Starting Multicast Session " +
                      groupName +
                      " With The Following Command:" + Environment.NewLine + "cmd.exe " + processArguments + Environment.NewLine;

        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = processArguments,
            UseShellExecute = false
        };

        Process sender;
        try
        {
            sender = Process.Start(psi);
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
