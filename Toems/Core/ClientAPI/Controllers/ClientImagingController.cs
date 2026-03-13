using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Dto.formdata;
using System.Text;
using System;
using System.Web;
using System.Configuration;
using Toems_Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.NoInjectTemp;
using Toems_ServiceCore.Workflows;

namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class ClientImagingController(ServiceContext ctx) : ControllerBase
    {

        private readonly HttpResponseMessage _response = new HttpResponseMessage(HttpStatusCode.OK);

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage AddComputer(AddComputerDTO addComputerDto)
        {
            _response.Content =
                new StringContent(
                    ctx.ClientImaging.AddComputer(addComputerDto.name, addComputerDto.mac,
                        addComputerDto.clientIdentifier),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage AddImage(NameDTO nameDto)
        {
            var userId = HttpContext.Response.Headers["client_user_id"];
             _response.Content = new StringContent(ctx.ClientImaging.AddImage(nameDto.name,userId), Encoding.UTF8,
                "text/plain");
            return _response;
        }



        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage AddImageWinPEEnv(NameDTO nameDto)
        {
            var userId = HttpContext.Response.Headers["client_user_id"];
            _response.Content = new StringContent(ctx.ClientImaging.AddImageWinPEEnv(nameDto.name,userId),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckForCancelledTask(ActiveTaskDTO activeTaskDto)
        {
            _response.Content =
                new StringContent(
                    ctx.ClientImaging.CheckForCancelledTask(Convert.ToInt32(activeTaskDto.taskId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpGet]
        [ClientImagingAuth]
        public HttpResponseMessage RegistrationSettings()
        {
            _response.Content = new StringContent(
                ctx.ClientImaging.GetRegistrationSettings(), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckHdRequirements(HdReqs hdReqs)
        {
            _response.Content =
                new StringContent(
                    ctx.ClientImaging.CheckHdRequirements(Convert.ToInt32(hdReqs.profileId),
                        Convert.ToInt32(hdReqs.clientHdNumber), hdReqs.newHdSize, hdReqs.schemaHds,
                        Convert.ToInt32(hdReqs.clientLbs)), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckHdRequirementsFfu(HdReqs hdReqs)
        {
            _response.Content =
                new StringContent(
                    ctx.ClientImaging.CheckHdRequirementsFfu(Convert.ToInt32(hdReqs.profileId),
                        Convert.ToInt32(hdReqs.clientHdNumber), hdReqs.newHdSize, hdReqs.schemaHds), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage PrepareServerUpload(DtoPrepareUpload upload)
        {
            _response.Content = new StringContent(ctx.UploadImage.Upload(upload.taskId,upload.fileName,upload.profileId,upload.userId,upload.hdNumber),
               Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpGet]
        [ClientImagingAuth]
        public HttpResponseMessage GetUploadServerIp()
        {
            _response.Content = new StringContent(ctx.ClientImaging.GetUploadServerIp(),
               Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void CloseServerUpload(DtoCloseUpload upload)
        {
            ctx.ClientImaging.CloseUpload(upload.taskId, upload.port);
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckIn(ActiveTaskDTO activeTaskDto)
        {
            _response.Content = new StringContent(ctx.ClientImaging.CheckIn(activeTaskDto.taskId,activeTaskDto.comServers),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void CheckOut(ActiveTaskDTO activeTaskDto)
        {
            ctx.ClientImaging.CheckOut(Convert.ToInt32(activeTaskDto.taskId));
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckQueue(ActiveTaskDTO activeTaskDto)
        {
            _response.Content =
                new StringContent(ctx.ClientImaging.CheckQueue(Convert.ToInt32(activeTaskDto.taskId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage CheckTaskAuth(TaskDTO taskDto)
        {
            var userToken = StringManipulationServices.Decode(HttpContext.Request.Headers["Authorization"],
                "Authorization");
            _response.Content = new StringContent(ctx.ClientImaging.CheckTaskAuth(taskDto.task, userToken),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage ConsoleLogin()
        {
            var ip = StringManipulationServices.Decode(HttpContext.Request.Form["clientIP"], "clientIP");
            var username = StringManipulationServices.Decode(HttpContext.Request.Form["username"], "username");
            var password = StringManipulationServices.Decode(HttpContext.Request.Form["password"], "password");
            var task = StringManipulationServices.Decode(HttpContext.Request.Form["task"], "task");

            _response.Content =
                new StringContent(ctx.Authentication.ConsoleLogin(username, password, task, ip), Encoding.UTF8,
                    "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void DeleteImage(ProfileDTO profileDto)
        {
            ctx.ClientImaging.DeleteImage(Convert.ToInt32(profileDto.profileId));
        }

        [HttpPost]
        public HttpResponseMessage GetComputerNameForPe(IdTypeDTO idTypeDto)
        {
            _response.Content =
                new StringContent(ctx.ClientImaging.GetComputerNameForPe(idTypeDto.id),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage DetermineTask(IdTypeDTO idTypeDto)
        {
            _response.Content =
                new StringContent(ctx.ClientImaging.DetermineTask(idTypeDto.id),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage GetWebTaskToken(IdTypeDTO idTypeDto)
        {
            _response.Content = new StringContent(ctx.ClientImaging.GetWebTaskToken(idTypeDto.id), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage ModelMatch(ModelMatchDTO modelMatch)
        {
            _response.Content =
                new StringContent(ctx.ClientImaging.CheckModelMatch(modelMatch.environment, modelMatch.systemModel),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

   
        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage DownloadCoreScripts(ScriptNameDTO scriptDto)
        {
            var scriptPath = ctx.Environment.ContentRootPath + Path.DirectorySeparatorChar + "private" +
                             Path.DirectorySeparatorChar + "clientscripts" + Path.DirectorySeparatorChar;


            if (System.IO.File.Exists(scriptPath + scriptDto.scriptName))
            {

                try
                {
                    var result = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = new FileStream(scriptPath + scriptDto.scriptName, FileMode.Open);
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = scriptDto.scriptName;
                    return result;
                }
                catch(Exception ex)
                {
                    ctx.Log.Debug("Error");
                    ctx.Log.Debug(ex.Message);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [ClientImagingAuth]
        public void ErrorEmail(ErrorEmailDTO errorEmailDto)
        {
            ctx.ClientImaging.ErrorEmail(Convert.ToInt32(errorEmailDto.taskId), errorEmailDto.error);
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetOtherImageServers(ComputerIdDTO computerIdDto)
        {
            _response.Content = new StringContent(
                ctx.ClientImaging.GetAllClusterComServers(computerIdDto.computerId), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetCustomPartitionScript(ProfileDTO profileDto)
        {
            _response.Content =
                new StringContent(
                    ctx.ClientImaging.GetCustomPartitionScript(Convert.ToInt32(profileDto.profileId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetCustomScript(ScriptIdDTO scriptIdDto)
        {
            _response.Content = new StringContent(ctx.ClientImaging.GetCustomScript(scriptIdDto.scriptId),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetFileCopySchema(ProfileDTO profileDto)
        {
            _response.Content = new StringContent(ctx.ClientImaging.GetFileCopySchema(profileDto.profileId),
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
                    ctx.ClientImaging.GetOriginalLvm(Convert.ToInt32(originalLvm.profileId),
                        originalLvm.clientHd, originalLvm.hdToGet, originalLvm.partitionPrefix), Encoding.UTF8,
                    "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetPartLayout(PartitionDTO partitionDto)
        {

            ctx.ClientPartitionScript.profileId = Convert.ToInt32(partitionDto.imageProfileId);
            ctx.ClientPartitionScript.HdNumberToGet = Convert.ToInt32(partitionDto.hdToGet);
            ctx.ClientPartitionScript.NewHdSize = partitionDto.newHdSize;
            ctx.ClientPartitionScript.ClientHd = partitionDto.clientHd;
            ctx.ClientPartitionScript.TaskType = partitionDto.taskType;
            ctx.ClientPartitionScript.partitionPrefix = partitionDto.partitionPrefix;
            ctx.ClientPartitionScript.clientBlockSize = Convert.ToInt32(partitionDto.lbs);
            

            _response.Content = new StringContent(ctx.ClientPartitionScript.GeneratePartitionScript(), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetSysprepTag(SysprepDTO sysprepDto)
        {
            _response.Content =
                new StringContent(
                    ctx.ClientImaging.GetSysprepTag(sysprepDto.tagId, sysprepDto.imageEnvironment),
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
        public ActionResult IpxeBoot(string filename, string type)
        {
            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
            }

            if (string.IsNullOrEmpty(thisComServer.TftpPath))
            {
                ctx.Log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");

            }
            if (type == "kernel")
            {
                return PhysicalFile(Path.Combine(thisComServer.TftpPath, "kernels", filename), "application/octet-stream", filename);
            }
            else
            {
                return PhysicalFile(Path.Combine(thisComServer.TftpPath, "images", filename), "application/x-gzip", filename);
            }
        }

        [HttpPost]
        public HttpResponseMessage IpxeLogin()
        {
            var username = HttpContext.Request.Form["uname"];
            var password = HttpContext.Request.Form["pwd"];
            var kernel = HttpContext.Request.Form["kernel"];
            var bootImage = HttpContext.Request.Form["bootImage"];
            var task = HttpContext.Request.Form["task"];

            _response.Content =
                new StringContent(ctx.Authentication.IpxeLogin(username, password, kernel, bootImage, task),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        public HttpResponseMessage IsLoginRequired(TaskDTO taskDto)
        {
            _response.Content = new StringContent(ctx.ClientImaging.IsLoginRequired(taskDto.task),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage ListImageProfiles(ImageIdDTO imageIdDto)
        {
            _response.Content =
                new StringContent(ctx.ClientImaging.ImageProfileList(Convert.ToInt32(imageIdDto.imageId)),
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
                    ctx.ClientImaging.ImageList(imageListDto.environment,
                        imageListDto.computerid, imageListDto.task, Convert.ToInt32(imageListDto.userId)),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage ListMulticasts(EnvironmentDTO environmentDto)
        {
            _response.Content =
                new StringContent(ctx.ClientImaging.MulicastSessionList(environmentDto.environment),
                    Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage MulticastCheckOut(PortDTO portDto)
        {
            _response.Content = new StringContent(ctx.ClientImaging.MulticastCheckout(portDto.portBase,portDto.comServerId),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage OnDemandCheckin(OnDemandDTO onDemandDto)
        {
            _response.Content =
                new StringContent(
                    ctx.ClientImaging.OnDemandCheckIn(onDemandDto.mac,
                        Convert.ToInt32(onDemandDto.objectId), onDemandDto.task, onDemandDto.userId,
                        onDemandDto.computerId,onDemandDto.comServers), Encoding.UTF8, "text/plain");
            return _response;
        }

   

        [HttpGet]
        public HttpResponseMessage Test()
        {
            _response.Content = new StringContent("true", Encoding.UTF8, "text/plain");
            return _response;
        }


        [HttpGet]
        [ClientImagingAuth]
        public HttpResponseMessage GetSmbShare()
        {
            _response.Content = new StringContent(ctx.ClientImaging.GetSmbShare(), Encoding.UTF8, "text/plain");
            return _response;
        }
        
        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage UpdateLegacyBcd()
        {
            var bcd = StringManipulationServices.Decode(HttpContext.Request.Form["bcd"], "bcd");
            var offsetBytes = StringManipulationServices.Decode(HttpContext.Request.Form["offsetBytes"],
                "offsetBytes");
            var diskSignature = StringManipulationServices.Decode(HttpContext.Request.Form["diskSignature"],
               "diskSignature");
            _response.Content = new StringContent(ctx.Bcd.UpdateLegacy(bcd, diskSignature, Convert.ToInt64(offsetBytes)),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetStandardLegacyBcd()
        {
            var diskSignature = StringManipulationServices.Decode(HttpContext.Request.Form["diskSignature"],
               "diskSignature");
            _response.Content = new StringContent(ctx.Bcd.GetStandardLegacy(diskSignature),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage GetStandardEfiBcd()
        {
            var diskGuid = StringManipulationServices.Decode(HttpContext.Request.Form["diskGuid"], "diskGuid");
            var windowsGuid = StringManipulationServices.Decode(HttpContext.Request.Form["windowsGuid"],
                "windowsGuid");
            var recoveryGuid = StringManipulationServices.Decode(HttpContext.Request.Form["recoveryGuid"],
               "recoveryGuid");
            var efiGuid = StringManipulationServices.Decode(HttpContext.Request.Form["efiGuid"],
              "recoveryGuid");
            _response.Content = new StringContent(ctx.Bcd.GetStandardEfi(diskGuid, recoveryGuid, windowsGuid, efiGuid),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage UpdateEfiBcd()
        {
            var bcd = StringManipulationServices.Decode(HttpContext.Request.Form["bcd"], "bcd");
            var diskGuid = StringManipulationServices.Decode(HttpContext.Request.Form["diskGuid"], "diskGuid");
            var windowsGuid = StringManipulationServices.Decode(HttpContext.Request.Form["windowsGuid"],
                "windowsGuid");
            var recoveryGuid = StringManipulationServices.Decode(HttpContext.Request.Form["recoveryGuid"],
               "recoveryGuid");
            _response.Content = new StringContent(ctx.Bcd.UpdateEfi(bcd, diskGuid, recoveryGuid, windowsGuid),
                Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage UpdateGuid(ProfileDTO profileDto)
        {
            _response.Content = new StringContent(
                ctx.ClientImaging.UpdateGuid(profileDto.profileId), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UpdateProgress(ProgressDTO progressDto)
        {
            ctx.ClientImaging.UpdateProgress(Convert.ToInt32(progressDto.taskId), progressDto.progress,
                progressDto.progressType);
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UpdateProgressPartition(ProgressPartitionDTO progressPartitionDto)
        {
            ctx.ClientImaging.UpdateProgressPartition(Convert.ToInt32(progressPartitionDto.taskId),
                progressPartitionDto.partition);
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UpdateStatusInProgress(ActiveTaskDTO activeTaskDto)
        {
            ctx.ClientImaging.ChangeStatusInProgress(Convert.ToInt32(activeTaskDto.taskId));
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage SaveImageSchemaPE(DtoUploadSchema schema)
        {
            byte[] data = Convert.FromBase64String(schema.schema);
            string decodedSchema = Encoding.UTF8.GetString(data);
            _response.Content = new StringContent(
                ctx.ClientImaging.SaveImageSchema(schema.profileId, decodedSchema), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage SaveImageSchema(DtoUploadSchema schema)
        {
            _response.Content = new StringContent(
                ctx.ClientImaging.SaveImageSchema(schema.profileId, schema.schema), Encoding.UTF8, "text/plain");
            return _response;
        }

        [HttpPost]
        [ClientImagingAuth]
        public void UploadLog()
        {
            var computerId = StringManipulationServices.Decode(HttpContext.Request.Form["computerId"],
                "computerId");
            var logContents = StringManipulationServices.Decode(HttpContext.Request.Form["logContents"],
                "logContents");
            var subType = StringManipulationServices.Decode(HttpContext.Request.Form["subType"], "subType");
            var computerMac = StringManipulationServices.Decode(HttpContext.Request.Form["mac"], "mac");
            ctx.ClientImaging.UploadLog(computerId, logContents, subType, computerMac);
        }

        [HttpPost]
        [ClientImagingAuth]
        public HttpResponseMessage UploadMbr()
        {
            var profileId = HttpContext.Request.Form["profileId"];
            var hdNumber = HttpContext.Request.Form["hdNumber"];
            var httpRequest = HttpContext.Request;
            _response.Content = new StringContent(
                ctx.ClientImaging.SaveMbr(httpRequest.Form.Files, Convert.ToInt32(profileId),hdNumber), Encoding.UTF8, "text/plain");
            return _response;
         
        }


        [HttpPost]
        [ClientImagingAuth]
        public ActionResult GetImagingFile(DtoImageFileRequest fileRequest)
        {
            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);

            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return NotFound();
            }

            var profile = ctx.ImageProfile.ReadProfile(fileRequest.profileId);

            if (profile == null)
            {
                ctx.Log.Error($"Image Profile Not Found: {fileRequest.profileId}");
                return NotFound();
            }

            var maxBitRate = thisComServer.ImagingMaxBps;
            var basePath = thisComServer.LocalStoragePath;

            var fullPath = Path.Combine(basePath, "images", profile.Image.Name, $"hd{fileRequest.hdNumber}", fileRequest.fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            try
            {
                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);

                Stream stream = fileStream;

                if (maxBitRate > 0)
                    stream = new ThrottledStream(fileStream, maxBitRate)
                    {
                        BlockSize = 1048576
                    };

                return File(stream, "application/octet-stream", fileRequest.fileName);
            }
            catch (Exception ex)
            {
                ctx.Log.Error(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ClientImagingAuth]
        public ActionResult GetFile(DtoFileRequest fileRequest)
        {
            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return NotFound();
            }
         
            var maxBitRate = thisComServer.ImagingMaxBps;
            var basePath = thisComServer.LocalStoragePath;

            var fullPath = Path.Combine(basePath, "software_uploads", fileRequest.guid, fileRequest.fileName);
            
            
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            try
            {
                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);

                Stream stream = fileStream;

                if (maxBitRate > 0)
                    stream = new ThrottledStream(fileStream, maxBitRate)
                    {
                        BlockSize = 1048576
                    };

                return File(stream, "application/octet-stream", fileRequest.fileName);
            }
            catch (Exception ex)
            {
                ctx.Log.Error(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public ActionResult<DtoTftpServer> GetAllTftpServers()
        {
            var allTftpServers = ctx.ProxyDhcp.GetAllTftpServers();
            if (allTftpServers == null)
                return NotFound();
            return allTftpServers;
        }



        [HttpGet]
        public ActionResult<DtoTftpServer> GetComputerTftpServers(string mac)
        {
            var computerTftpServers = ctx.ProxyDhcp.GetComputerTftpServers(mac);
            if (computerTftpServers == null)
                return NotFound();
            return computerTftpServers;
        }

        [HttpGet]
        public ActionResult<DtoProxyReservation> GetProxyReservation(string mac)
        {
            var proxyReservation = ctx.ProxyDhcp.GetProxyReservation(mac);
            if (proxyReservation == null)
                return NotFound();
            return proxyReservation;
        }
    }
}