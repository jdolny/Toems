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
    public class SysprepModuleController : ApiController
    {
        private readonly ServiceSysprepModule _sysprepModuleServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public SysprepModuleController()
        {
            _sysprepModuleServices = new ServiceSysprepModule();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleDelete)]
        public DtoActionResult Delete(int id)
        {
            var module = _sysprepModuleServices.GetModule(id);
            var result = _sysprepModuleServices.DeleteModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "SysprepModule";
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
        public EntitySysprepModule Get(int id)
        {
            var result = _sysprepModuleServices.GetModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public IEnumerable<EntitySysprepModule> Get()
        {
            return _sysprepModuleServices.SearchModules(new DtoSearchFilterCategories());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntitySysprepModule> Search(DtoSearchFilterCategories filter)
        {
            return  _sysprepModuleServices.SearchModules(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntitySysprepModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return  _sysprepModuleServices.GetArchived(filter);
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _sysprepModuleServices.TotalCount() };
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
       public DtoApiStringResponse GetArchivedCount()
       {
           return new DtoApiStringResponse { Value = _sysprepModuleServices.ArchivedCount() };
       }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Post(EntitySysprepModule module)
        {
            var result = _sysprepModuleServices.AddModule(module);
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "SysprepModule";
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
        public DtoActionResult Put(int id, EntitySysprepModule module)
        {
            module.Id = id;
            var result = _sysprepModuleServices.UpdateModule(module);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "SysprepModule";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = module.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(module);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }
    }
}