using log4net;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class UploadImage
    {
        private readonly ILog log = LogManager.GetLogger(typeof(UploadImage));
        private EntityClientComServer _thisComServer;

        public string Upload(int taskId, string fileName, int profileId, int userId, string hdNumber)
        {
            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            _thisComServer = new ServiceClientComServer().GetServerByGuid(guid);

            if (_thisComServer == null)
                return "0";

            PathUtils.ValidateFileName(fileName);
            ValidateHdNumber(hdNumber);

            var appPath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "private", "apps");
            var receiverExe = Path.Combine(appPath, "udp-receiver.exe");

            if (!File.Exists(receiverExe))
                throw new FileNotFoundException("udp-receiver.exe not found", receiverExe);

            var task = new ServiceActiveImagingTask().GetTask(taskId);
            if (task == null)
                return "0";

            var imageProfile = new ServiceImageProfile().ReadProfile(profileId);
            var uploadPort = new ServicePort().GetNextPort(task.ComServerId);

            var imageRoot = Path.Combine(_thisComServer.LocalStoragePath, "images", imageProfile.Image.Name, $"hd{hdNumber}");

            Directory.CreateDirectory(imageRoot);

            var fullFilePath = PathUtils.CombineSafe(imageRoot, fileName);

            var psi = new ProcessStartInfo
            {
                FileName = receiverExe,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments =
                    "--portbase " + uploadPort +
                    " --interface " + PathUtils.Quote(_thisComServer.ImagingIp) +
                    " --file " + PathUtils.Quote(fullFilePath)
            };

            LogCommand(psi, imageProfile.Image.Name);

            Process receiver;
            try
            {
                receiver = Process.Start(psi);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return "0";
            }

            Thread.Sleep(2000);

            if (receiver == null || receiver.HasExited)
                return "0";

            var session = new EntityActiveMulticastSession
            {
                ImageProfileId = imageProfile.Id,
                Name = imageProfile.Image.Name,
                Pid = receiver.Id,
                Port = uploadPort,
                ComServerId = _thisComServer.Id,
                UserId = userId,
                UploadTaskId = task.Id
            };

            if (new ServiceActiveMulticastSession().AddActiveMulticastSession(session))
                return uploadPort.ToString();

            return "0";
        }


        private static void ValidateHdNumber(string hd)
        {
            if (!int.TryParse(hd, out var n) || n < 0 || n > 64)
                throw new ArgumentException("Invalid HD number");
        }

        private void LogCommand(ProcessStartInfo psi, string imageName)
        {
            var logPath = Path.Combine(
                HttpContext.Current.Server.MapPath("~"),
                "private",
                "logs",
                "multicast.log");

            var text = $"{Environment.NewLine}{DateTime.Now:MM-dd-yy HH:mm} " +
                       $"Starting Upload Session {imageName}{Environment.NewLine}" +
                       $"{psi.FileName} {psi.Arguments}{Environment.NewLine}";

            File.AppendAllText(logPath, text);
        }
    }
}
