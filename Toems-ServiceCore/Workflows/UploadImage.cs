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
using Toems_Common;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class UploadImage(InfrastructureContext ictx, ServiceClientComServer serviceClientComServer, ServiceActiveImagingTask serviceActiveImagingTask, 
        ServiceImageProfile serviceImageProfile, ServicePort servicePort, ServiceActiveMulticastSession serviceActiveMulticastSession)
    {
        private EntityClientComServer _thisComServer;
        public string Upload(int taskId, string fileName, int profileId, int userId, string hdNumber)
        {
            //no need to find and call com server, client should already be directly communicating with the correct imaging server
            var guid = ictx.Config["ComServerUniqueId"];
            _thisComServer = serviceClientComServer.GetServerByGuid(guid);

            if (_thisComServer == null)
            {
                ictx.Log.Error($"Com Server With Guid {guid} Not Found");
                return "0";
            }

            var appPath = Path.Combine(ictx.Environment.ContentRootPath,"private","apps"); 

            var task = serviceActiveImagingTask.GetTask(taskId);
            if (task == null)
                return "0";

            var imageProfile = serviceImageProfile.ReadProfile(profileId);

            var uploadPort = servicePort.GetNextPort(task.ComServerId);
      
            var path = _thisComServer.LocalStoragePath;


            try
            {
                var dir = Path.Combine(path, "images", imageProfile.Image.Name, $"hd{ hdNumber}");
                if(!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch(Exception ex)
            {
                ictx.Log.Error("Could Not Create Directory");
                ictx.Log.Error(ex.Message);
                return "0";
            }

            path = Path.Combine(path, "images", imageProfile.Image.Name, $"hd{hdNumber}", fileName);
            string arguments = " /c \"";
            var receiverPath = Path.Combine(appPath, "udp-receiver.exe");
            arguments += $"{receiverPath}\" --portbase {uploadPort}";
            arguments += $" --interface {_thisComServer.ImagingIp} --file {path}";

            var pid = StartReceiver(arguments, imageProfile.Image.Name);
            //use multicast session even though it's not a multicast, uploads still use udpcast
            var activeMulticast = new EntityActiveMulticastSession();
            
            if(pid != 0)
            {
                activeMulticast.ImageProfileId = imageProfile.Id;
                activeMulticast.Name = imageProfile.Image.Name;
                activeMulticast.Pid = pid;
                activeMulticast.Port = uploadPort;
                activeMulticast.ComServerId = _thisComServer.Id;
                activeMulticast.UserId = userId;
                activeMulticast.UploadTaskId = task.Id;

                var result = serviceActiveMulticastSession.AddActiveMulticastSession(activeMulticast);
                if (result) return uploadPort.ToString();
            }
            return "0";
           
        }

        private int StartReceiver(string processArguments, string imageName)
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


            var senderInfo = new ProcessStartInfo { FileName = shell, Arguments = processArguments, UseShellExecute = false};

            var logPath = ictx.Environment.ContentRootPath + Path.DirectorySeparatorChar + "private" +
                          Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "multicast.log";

            var logText = Environment.NewLine + DateTime.Now.ToString("MM-dd-yy hh:mm") +
                          " Starting Upload Session " +
                          imageName +
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
                ictx.Log.Error(ex.ToString());
                File.AppendAllText(logPath,
                    "Could Not Start Session " + imageName + " Try Pasting The Command Into A Command Prompt");
                return 0;
            }

            Thread.Sleep(2000);

            if (sender == null) return 0;

            if (sender.HasExited)
            {
                File.AppendAllText(logPath,
                    "Session " + imageName + " Started And Was Forced To Quit, Try Running The Command Manually");
                return 0;
            }

            return sender.Id;
        }
    }
}
