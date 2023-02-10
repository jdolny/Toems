using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class ImageController : ApiController
    {
        private readonly ServiceImage _imageService;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ImageController()
        {
            _imageService = new ServiceImage();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());

        }

        [CustomAuth(Permission = AuthorizationStrings.ImageDelete)]
        [ImageAuth]
        public DtoActionResult Delete(int id)
        {
            var image = _imageService.GetImage(id);
            var result = _imageService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            else
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Image";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = image.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(image);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);
                return result;
            }
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageRead)]
        [ImageAuth]
        public EntityImage Get(int id)
        {
            var result = _imageService.GetImage(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [Authorize]
        public IEnumerable<EntityImage> Get()
        {
            return _imageService.GetAll(_userId);
        }


        [Authorize]
        public IEnumerable<DtoServerImageRepStatus> GetReplicationStatus(int id)
        {
            return _imageService.GetReplicationStatus(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageRead)]
        [HttpPost]
        public IEnumerable<ImageWithDate> Search(DtoSearchFilterCategories filter)
        {
            return _imageService.Search(filter,_userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _imageService.TotalCount()};
        }

        [Authorize]
        public IEnumerable<EntityImageProfile> GetImageProfiles(int id)
        {
            return _imageService.SearchProfiles(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageRead)]
        public DtoApiStringResponse GetImageSizeOnServer(string imageName, string hdNumber)
        {
            return new DtoApiStringResponse { Value = _imageService.ImageSizeOnServerForGridView(imageName, hdNumber) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageRead)]
        public IEnumerable<DtoImageFileInfo> GetPartitionFileInfo(int id, string selectedHd, string selectedPartition)
        {
            return _imageService.GetPartitionImageFileInfoForGridView(id, selectedHd, selectedPartition);
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageUpdate)]
        public DtoActionResult Post(EntityImage image)
        {
            var result = _imageService.Add(image);
            if(result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Image";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = image.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(image);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Create;
                _auditLogService.AddAuditLog(auditLog);


                new ServiceUser().UpdateUsersImagesList(new EntityToemsUsersImages() { ImageId = result.Id, UserId = Convert.ToInt32(_userId) });
            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageUpdate)]
        [ImageAuth]
        public DtoActionResult Put(int id, EntityImage image)
        {
            image.Id = id;
            var result = _imageService.Update(image);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if(result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Image";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = image.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(image);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);
            }
            return result;
        }

        [HttpGet]
        [Authorize]
        public EntityImageProfile SeedDefaultProfile(int id)
        {
            return _imageService.SeedDefaultImageProfile(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageRead)]
        public IEnumerable<EntityImageCategory> GetImageCategories(int id)
        {
            return _imageService.GetImageCategories(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageRead)]
        public IEnumerable<EntityAuditLog> GetImageAuditLogs(int id, int limit)
        {
            return _imageService.GetImageAuditLogs(id, limit);
        }
    }
}