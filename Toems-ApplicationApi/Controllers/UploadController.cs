using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class UploadController : ApiController
    {
        protected HttpContext Context;
        protected HttpResponse Response;
        protected new HttpRequest Request;
        private readonly string _basePath;

        public UploadController()
        {
            Context = HttpContext.Current;
            Request = HttpContext.Current.Request;
            Response = HttpContext.Current.Response;
            _basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
        }

        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.ModuleUploadFiles)]
        [EnableCors("*", "*", "*")]
        public void ChunkingComplete()
        {
            var fileName = Request["qqfilename"];
            if (string.IsNullOrEmpty(fileName) || fileName == Path.DirectorySeparatorChar.ToString())
            {
                throw new HttpException();
            }
            var moduleGuid = Request["moduleGuid"];
            if (string.IsNullOrEmpty(moduleGuid))
            {
                throw new HttpException();
            }
            var fullPath = Path.Combine(_basePath, "software_uploads", moduleGuid, fileName);

            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    var uploadedFile = new EntityUploadedFile();
                    uploadedFile.Name = fileName;
                    uploadedFile.Guid = moduleGuid;
                    uploadedFile.Hash = Utility.GetFileHash(fullPath);

                    var result = new ServiceUploadedFile().AddFile(uploadedFile);
                    if (!result.Success)
                    {
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch
                        {
                            //ignored                  
                        }
                        throw new HttpException();
                    }
                }
                else
                {
                    throw new HttpException("Could Not Reach Storage Path");
                }
            }
        }

        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.AttachmentAdd)]
        [EnableCors("*", "*", "*")]
        public void AttachmentChunkingComplete()
        {
            var fileName = Request["qqfilename"];
            if (string.IsNullOrEmpty(fileName) || fileName == Path.DirectorySeparatorChar.ToString())
            {
                throw new HttpException();
            }
            var attachmentGuid = Request["attachmentGuid"];
            if (string.IsNullOrEmpty(attachmentGuid))
            {
                throw new HttpException();
            }

            var computerId = Request["computerId"];
            var assetId = Request["assetId"];
            if (assetId == null && computerId == null)
            {
                throw new HttpException();
            }

            var userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
            var userName = new ServiceUser().GetUserName(userId);

            var attachment = new EntityAttachment();
            attachment.AttachmentTime = DateTime.Now;
            attachment.DirectoryGuid = attachmentGuid;
            attachment.Name = fileName;
            attachment.UserName = userName;
            var result = new ServiceAttachment().Add(attachment);
            if(!result.Success) throw new HttpException();

            if (assetId != null)
            {
                var asset = new EntityAssetAttachment();
                asset.AssetId = Convert.ToInt32(assetId);
                asset.AttachmentId = attachment.Id;
                result = new ServiceAssetAttachment().Add(asset);
                if(!result.Success) throw new HttpException();
            }

            if (computerId != null)
            {
                var computer = new EntityComputerAttachment();
                computer.ComputerId = Convert.ToInt32(computerId);
                computer.AttachmentId = attachment.Id;
                result = new ServiceComputerAttachment().Add(computer);
                if (!result.Success) throw new HttpException();
            }
        }

        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.ModuleUploadFiles)]
        [EnableCors("*", "*", "*")]
        public void UploadFile()
        {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            //Some basic validation
            var moduleId = Convert.ToInt32(Request["moduleId"]);
            var intType = Convert.ToInt16(Request["moduleType"]);
            var moduleType = (EnumModule.ModuleType) intType;
            if (moduleId == 0 || moduleType == 0)
            {
                Response.Write(
                    new FineUploaderResult(false, error: "Invalid Module Properties", preventRetry: true).BuildResponse());
                Response.End();
                return;
            }

            var isActiveModule = new ServiceModule().IsModuleActive(moduleId, moduleType);
            if (!string.IsNullOrEmpty(isActiveModule))
            {
                Response.Write(
                   new FineUploaderResult(false, error: isActiveModule.Trim(','), preventRetry: true).BuildResponse());
                Response.End();
                return;
            }

        
            //Now on to the upload

            // find filename
            var formUpload = Request.Files.Count > 0;
            var xFileName = Request.Headers["X-File-Name"];
            var qqFile = Request["qqfile"];
            var formFilename = formUpload ? Request.Files[0].FileName : null;
            var fileName = xFileName ?? qqFile ?? formFilename;
            var upload = new DtoFileUpload
            {
                Filename = fileName,
                InputStream = formUpload ? Request.Files[0].InputStream : Request.InputStream,
                UploadMethod = Request["uploadMethod"],
                DestinationDirectory = Path.Combine(_basePath, "software_uploads", Request["moduleGuid"]),
                ModuleGuid = Request["moduleGuid"]
            };

            int someval;
            if (int.TryParse(Request["qqpartindex"], out someval))
            {
                upload.PartIndex = someval;
                upload.OriginalFilename = Request["qqfilename"];
                upload.TotalParts = int.Parse(Request["qqtotalparts"]);
                upload.PartUuid = Request["qquuid"];
                upload.FileSize = ulong.Parse(Request["qqtotalfilesize"]);
            }

            var result = new FileUploadServices(upload).Upload("module");

            Response.Write(result == null
                ? new FineUploaderResult(true, new {extraInformation = 12345}).BuildResponse()
                : new FineUploaderResult(false, error: result).BuildResponse());

            Response.End();
        }


        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [EnableCors("*", "*", "*")]
        public void UploadClientMsi()
        {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            // find filename
            var formUpload = Request.Files.Count > 0;
            var xFileName = Request.Headers["X-File-Name"];
            var qqFile = Request["qqfile"];
            var formFilename = formUpload ? Request.Files[0].FileName : null;
            var fileName = xFileName ?? qqFile ?? formFilename;
            var upload = new DtoFileUpload
            {
                Filename = fileName,
                InputStream = formUpload ? Request.Files[0].InputStream : Request.InputStream,
                UploadMethod = Request["uploadMethod"],
                DestinationDirectory = Path.Combine(_basePath,"client_versions"),
            };

            int someval;
            if (int.TryParse(Request["qqpartindex"], out someval))
            {
                upload.PartIndex = someval;
                upload.OriginalFilename = Request["qqfilename"];
                upload.TotalParts = int.Parse(Request["qqtotalparts"]);
                upload.PartUuid = Request["qquuid"];
                upload.FileSize = ulong.Parse(Request["qqtotalfilesize"]);
            }

            var result = new FileUploadServices(upload).Upload("msi");

            Response.Write(result == null
                ? new FineUploaderResult(true, new { extraInformation = 12345 }).BuildResponse()
                : new FineUploaderResult(false, error: result).BuildResponse());

            Response.End();
        }

        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.AttachmentAdd)]
        [EnableCors("*", "*", "*")]
        public void UploadAttachment()
        {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            // find filename
            var formUpload = Request.Files.Count > 0;
            var xFileName = Request.Headers["X-File-Name"];
            var qqFile = Request["qqfile"];
            var formFilename = formUpload ? Request.Files[0].FileName : null;
            var fileName = xFileName ?? qqFile ?? formFilename;
            var userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
            var userName = new ServiceUser().GetUserName(userId);
            var upload = new DtoFileUpload
            {
                Filename = fileName,
                InputStream = formUpload ? Request.Files[0].InputStream : Request.InputStream,
                UploadMethod = Request["uploadMethod"],
                DestinationDirectory = Path.Combine(_basePath, "attachments", Request["attachmentGuid"]),
                AssetId = Request["assetId"],
                ComputerId = Request["computerId"],
                AttachmentGuid = Request["attachmentGuid"],
                Username = userName
                
            };

            int someval;
            if (int.TryParse(Request["qqpartindex"], out someval))
            {
                upload.PartIndex = someval;
                upload.OriginalFilename = Request["qqfilename"];
                upload.TotalParts = int.Parse(Request["qqtotalparts"]);
                upload.PartUuid = Request["qquuid"];
                upload.FileSize = ulong.Parse(Request["qqtotalfilesize"]);
            }

            var result = new FileUploadServices(upload).Upload("attachment");

            Response.Write(result == null
                ? new FineUploaderResult(true, new { extraInformation = 12345 }).BuildResponse()
                : new FineUploaderResult(false, error: result).BuildResponse());

            Response.End();
        }
    }
}