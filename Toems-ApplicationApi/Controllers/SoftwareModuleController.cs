using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Newtonsoft.Json;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class SoftwareModuleController : ApiController
    {
        private readonly ServiceSoftwareModule _softwareModuleServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public SoftwareModuleController()
        {
            _softwareModuleServices = new ServiceSoftwareModule();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleDelete)]
        public DtoActionResult Delete(int id)
        {
            var module = _softwareModuleServices.GetModule(id);
            var result = _softwareModuleServices.DeleteModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "SoftwareModule";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = module.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(module);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public EntitySoftwareModule Get(int id)
        {
            var result = _softwareModuleServices.GetModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public IEnumerable<EntitySoftwareModule> Get()
        {
            return _softwareModuleServices.SearchModules(new DtoSearchFilterCategories());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntitySoftwareModule> Search(DtoSearchFilterCategories filter)
        {
            return _softwareModuleServices.SearchModules(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntitySoftwareModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return _softwareModuleServices.GetArchived(filter);
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _softwareModuleServices.TotalCount() };
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
       public DtoApiStringResponse GetArchivedCount()
       {
           return new DtoApiStringResponse { Value = _softwareModuleServices.ArchivedCount() };
       }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Post(EntitySoftwareModule module)
        {
            var result = _softwareModuleServices.AddModule(module);
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "SoftwareModule";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = module.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(module);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Create;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Put(int id, EntitySoftwareModule module)
        {
            module.Id = id;
            var result = _softwareModuleServices.UpdateModule(module);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "SoftwareModule";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = module.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(module);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
       [HttpGet]
       public DtoActionResult GenerateArguments(int moduleId)
       {
           var result = _softwareModuleServices.GenerateArguments(moduleId);
           if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
           return result;
       }
    }
}