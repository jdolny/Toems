using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Workflows;
using Toems_Common.Dto.formdata;
using System.Text;
using System;
using Toems_Service;
using System.Web;
using System.Configuration;
using Toems_Service.Entity;

namespace Toems_ClientApi.Controllers
{
    public class ClientImagingController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly HttpResponseMessage _response = new HttpResponseMessage(HttpStatusCode.OK);

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage AddComputer(AddComputerDTO addComputerDto)
        {
            _response.Content =
                new StringContent(
                    new ClientImagingServices().AddComputer(addComputerDto.name, addComputerDto.mac,
                        addComputerDto.clientIdentifier),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage AddImage(NameDTO nameDto)
        {
            _response.Content = new StringContent(new ClientImagingServices().AddImage(nameDto.name), Encoding.UTF8,
                "text/plain");
            return _response;
        }



        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage AddImageWinPEEnv(NameDTO nameDto)
        {
            _response.Content = new StringContent(new ClientImagingServices().AddImageWinPEEnv(nameDto.name),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckForCancelledTask(ActiveTaskDTO activeTaskDto)
        {
            _response.Content =
                new StringContent(
                    new ClientImagingServices().CheckForCancelledTask(Convert.ToInt32(activeTaskDto.taskId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckHdRequirements(HdReqs hdReqs)
        {
            _response.Content =
                new StringContent(
                    new ClientImagingServices().CheckHdRequirements(Convert.ToInt32(hdReqs.profileId),
                        Convert.ToInt32(hdReqs.clientHdNumber), hdReqs.newHdSize, hdReqs.schemaHds,
                        Convert.ToInt32(hdReqs.clientLbs)), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckIn(ActiveTaskDTO activeTaskDto)
        {
            _response.Content = new StringContent(new ClientImagingServices().CheckIn(activeTaskDto.taskId),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void CheckOut(ActiveTaskDTO activeTaskDto)
        {
            new ClientImagingServices().CheckOut(Convert.ToInt32(activeTaskDto.taskId),
                Convert.ToInt32(activeTaskDto.profileId));
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckQueue(ActiveTaskDTO activeTaskDto)
        {
            _response.Content =
                new StringContent(new ClientImagingServices().CheckQueue(Convert.ToInt32(activeTaskDto.taskId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckTaskAuth(TaskDTO taskDto)
        {
            var userToken = StringManipulationServices.Decode(HttpContext.Current.Request.Headers["Authorization"],
                "Authorization");
            _response.Content = new StringContent(new ClientImagingServices().CheckTaskAuth(taskDto.task, userToken),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage ConsoleLogin()
        {
            var ip = StringManipulationServices.Decode(HttpContext.Current.Request.Form["clientIP"], "clientIP");
            var username = StringManipulationServices.Decode(HttpContext.Current.Request.Form["username"], "username");
            var password = StringManipulationServices.Decode(HttpContext.Current.Request.Form["password"], "password");
            var task = StringManipulationServices.Decode(HttpContext.Current.Request.Form["task"], "task");

            _response.Content =
                new StringContent(new AuthenticationServices().ConsoleLogin(username, password, task, ip), Encoding.UTF8,
                    "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void DeleteImage(ProfileDTO profileDto)
        {
            new ClientImagingServices().DeleteImage(Convert.ToInt32(profileDto.profileId));
        }

        [HttpPost]
        public HttpResponseMessage DetermineTask(IdTypeDTO idTypeDto)
        {
            _response.Content =
                new StringContent(new ClientImagingServices().DetermineTask(idTypeDto.idType, idTypeDto.id),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage ModelMatch(ModelMatchDTO modelMatch)
        {
            _response.Content =
                new StringContent(new ClientImagingServices().CheckModelMatch(modelMatch.environment, modelMatch.systemModel),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

   
        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage DownloadCoreScripts(ScriptNameDTO scriptDto)
        {
            var scriptPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                             Path.DirectorySeparatorChar + "clientscripts" + Path.DirectorySeparatorChar;

            if (File.Exists(scriptPath + scriptDto.scriptName))
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(scriptPath + scriptDto.scriptName, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = scriptDto.scriptName;
                return result;
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [ClientImagingAuth]
        public void ErrorEmail(ErrorEmailDTO errorEmailDto)
        {
            new ClientImagingServices().ErrorEmail(Convert.ToInt32(errorEmailDto.taskId), errorEmailDto.error);
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetAllClusterDps(ComputerIdDTO computerIdDto)
        {
            _response.Content = new StringContent(
                new ClientImagingServices().GetAllClusterComServers(computerIdDto.computerId), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetCustomPartitionScript(ProfileDTO profileDto)
        {
            _response.Content =
                new StringContent(
                    new ClientImagingServices().GetCustomPartitionScript(Convert.ToInt32(profileDto.profileId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetCustomScript(ScriptIdDTO scriptIdDto)
        {
            _response.Content = new StringContent(new ClientImagingServices().GetCustomScript(scriptIdDto.scriptId),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetFileCopySchema(ProfileDTO profileDto)
        {
            _response.Content = new StringContent(new ClientImagingServices().GetFileCopySchema(profileDto.profileId),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpGet]
        public HttpResponseMessage GetLocalDateTime()
        {
            _response.Content = new StringContent(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Encoding.UTF8,
                "text/plain");
            return _response;
        }



        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetOriginalLvm(OriginalLVM originalLvm)
        {
            _response.Content =
                new StringContent(
                    new ClientImagingServices().GetOriginalLvm(Convert.ToInt32(originalLvm.profileId),
                        originalLvm.clientHd, originalLvm.hdToGet, originalLvm.partitionPrefix), Encoding.UTF8,
                    "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetPartLayout(PartitionDTO partitionDto)
        {
            var partLayout = new ClientPartitionScript
            {
                profileId = Convert.ToInt32(partitionDto.imageProfileId),
                HdNumberToGet = Convert.ToInt32(partitionDto.hdToGet),
                NewHdSize = partitionDto.newHdSize,
                ClientHd = partitionDto.clientHd,
                TaskType = partitionDto.taskType,
                partitionPrefix = partitionDto.partitionPrefix,
                clientBlockSize = Convert.ToInt32(partitionDto.lbs)
            };

            _response.Content = new StringContent(partLayout.GeneratePartitionScript(), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetSysprepTag(SysprepDTO sysprepDto)
        {
            _response.Content =
                new StringContent(
                    new ClientImagingServices().GetSysprepTag(sysprepDto.tagId, sysprepDto.imageEnvironment),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpGet]
        public HttpResponseMessage GetUtcDateTime()
        {
            _response.Content = new StringContent(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), Encoding.UTF8,
                "text/plain");
            return _response;
        }

        [HttpGet]
        public void IpxeBoot(string filename, string type)
        {
            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (thisComServer == null)
            {
                Logger.Error($"Com Server With Guid {guid} Not Found");
            }

            if (string.IsNullOrEmpty(thisComServer.TftpPath))
            {
                Logger.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");

            }
            if (type == "kernel")
            {
                var path = thisComServer.TftpPath + "kernels" +
                           Path.DirectorySeparatorChar;
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
                HttpContext.Current.Response.TransmitFile(path + filename);
                HttpContext.Current.Response.End();
            }
            else
            {
                var path = thisComServer.TftpPath + "images" +
                           Path.DirectorySeparatorChar;
                HttpContext.Current.Response.ContentType = "application/x-gzip";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
                HttpContext.Current.Response.TransmitFile(path + filename);
                HttpContext.Current.Response.End();
            }
        }

        [HttpPost]
        public HttpResponseMessage IpxeLogin()
        {
            var username = HttpContext.Current.Request.Form["uname"];
            var password = HttpContext.Current.Request.Form["pwd"];
            var kernel = HttpContext.Current.Request.Form["kernel"];
            var bootImage = HttpContext.Current.Request.Form["bootImage"];
            var task = HttpContext.Current.Request.Form["task"];

            _response.Content =
                new StringContent(new AuthenticationServices().IpxeLogin(username, password, kernel, bootImage, task),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage IsLoginRequired(TaskDTO taskDto)
        {
            _response.Content = new StringContent(new ClientImagingServices().IsLoginRequired(taskDto.task),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage ListImageProfiles(ImageIdDTO imageIdDto)
        {
            _response.Content =
                new StringContent(new ClientImagingServices().ImageProfileList(Convert.ToInt32(imageIdDto.imageId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage ListImages(ImageListDTO imageListDto)
        {
            if (string.IsNullOrEmpty(imageListDto.userId))
                imageListDto.userId = "0";
            _response.Content =
                new StringContent(
                    new ClientImagingServices().ImageList(imageListDto.environment,
                        imageListDto.computerid, imageListDto.task, Convert.ToInt32(imageListDto.userId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage ListMulticasts(EnvironmentDTO environmentDto)
        {
            _response.Content =
                new StringContent(new ClientImagingServices().MulicastSessionList(environmentDto.environment),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage MulticastCheckOut(PortDTO portDto)
        {
            _response.Content = new StringContent(new ClientImagingServices().MulticastCheckout(portDto.portBase),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage OnDemandCheckin(OnDemandDTO onDemandDto)
        {
            _response.Content =
                new StringContent(
                    new ClientImagingServices().OnDemandCheckIn(onDemandDto.mac,
                        Convert.ToInt32(onDemandDto.objectId), onDemandDto.task, onDemandDto.userId,
                        onDemandDto.computerId), Encoding.UTF8, "text/plain");
            return _response;
        }

   

        [HttpGet]
        public HttpResponseMessage Test()
        {
            _response.Content = new StringContent("true", Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage UpdateLegacyBcd()
        {
            var bcd = StringManipulationServices.Decode(HttpContext.Current.Request.Form["bcd"], "bcd");
            var offsetBytes = StringManipulationServices.Decode(HttpContext.Current.Request.Form["offsetBytes"],
                "offsetBytes");
            var diskSignature = StringManipulationServices.Decode(HttpContext.Current.Request.Form["diskSignature"],
               "diskSignature");
            _response.Content = new StringContent(new BcdServices().UpdateLegacy(bcd, diskSignature, Convert.ToInt64(offsetBytes)),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetStandardLegacyBcd()
        {
            var diskSignature = StringManipulationServices.Decode(HttpContext.Current.Request.Form["diskSignature"],
               "diskSignature");
            _response.Content = new StringContent(new BcdServices().GetStandardLegacy(diskSignature),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetStandardEfiBcd()
        {
            var diskGuid = StringManipulationServices.Decode(HttpContext.Current.Request.Form["diskGuid"], "diskGuid");
            var windowsGuid = StringManipulationServices.Decode(HttpContext.Current.Request.Form["windowsGuid"],
                "windowsGuid");
            var recoveryGuid = StringManipulationServices.Decode(HttpContext.Current.Request.Form["recoveryGuid"],
               "recoveryGuid");
            var efiGuid = StringManipulationServices.Decode(HttpContext.Current.Request.Form["efiGuid"],
              "recoveryGuid");
            _response.Content = new StringContent(new BcdServices().GetStandardEfi(diskGuid, recoveryGuid, windowsGuid, efiGuid),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage UpdateEfiBcd()
        {
            var bcd = StringManipulationServices.Decode(HttpContext.Current.Request.Form["bcd"], "bcd");
            var diskGuid = StringManipulationServices.Decode(HttpContext.Current.Request.Form["diskGuid"], "diskGuid");
            var windowsGuid = StringManipulationServices.Decode(HttpContext.Current.Request.Form["windowsGuid"],
                "windowsGuid");
            var recoveryGuid = StringManipulationServices.Decode(HttpContext.Current.Request.Form["recoveryGuid"],
               "recoveryGuid");
            _response.Content = new StringContent(new BcdServices().UpdateEfi(bcd, diskGuid, recoveryGuid, windowsGuid),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage UpdateGuid(ProfileDTO profileDto)
        {
            _response.Content = new StringContent(
                new ClientImagingServices().UpdateGuid(profileDto.profileId), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UpdateProgress(ProgressDTO progressDto)
        {
            new ClientImagingServices().UpdateProgress(Convert.ToInt32(progressDto.taskId), progressDto.progress,
                progressDto.progressType);
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UpdateProgressPartition(ProgressPartitionDTO progressPartitionDto)
        {
            new ClientImagingServices().UpdateProgressPartition(Convert.ToInt32(progressPartitionDto.taskId),
                progressPartitionDto.partition);
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UpdateStatusInProgress(ActiveTaskDTO activeTaskDto)
        {
            new ClientImagingServices().ChangeStatusInProgress(Convert.ToInt32(activeTaskDto.taskId));
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UploadLog()
        {
            var computerId = StringManipulationServices.Decode(HttpContext.Current.Request.Form["computerId"],
                "computerId");
            var logContents = StringManipulationServices.Decode(HttpContext.Current.Request.Form["logContents"],
                "logContents");
            var subType = StringManipulationServices.Decode(HttpContext.Current.Request.Form["subType"], "subType");
            var computerMac = StringManipulationServices.Decode(HttpContext.Current.Request.Form["mac"], "mac");
            new ClientImagingServices().UploadLog(computerId, logContents, subType, computerMac);
        }
    }
}