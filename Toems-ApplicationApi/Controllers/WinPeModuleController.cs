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
    public class WinPeModuleController : ApiController
    {
        private readonly ServiceWinPeModule _winPeModuleServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public WinPeModuleController()
        {
            _winPeModuleServices = new ServiceWinPeModule();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleDelete)]
        public DtoActionResult Delete(int id)
        {
            var module = _winPeModuleServices.GetModule(id);
            var result = _winPeModuleServices.DeleteModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "WinPeModule";
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
        public EntityWinPeModule Get(int id)
        {
            var result = _winPeModuleServices.GetModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public IEnumerable<EntityWinPeModule> Get()
        {
            return _winPeModuleServices.SearchModules(new DtoSearchFilterCategories());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityWinPeModule> Search(DtoSearchFilterCategories filter)
        {
            return _winPeModuleServices.SearchModules(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityWinPeModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return _winPeModuleServices.GetArchived(filter);
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _winPeModuleServices.TotalCount() };
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
       public DtoApiStringResponse GetArchivedCount()
       {
           return new DtoApiStringResponse { Value = _winPeModuleServices.ArchivedCount() };
       }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Post(EntityWinPeModule module)
        {
            var result = _winPeModuleServices.AddModule(module);
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "WinPeModule";
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
        public DtoActionResult Put(int id, EntityWinPeModule module)
        {
            module.Id = id;
            var result = _winPeModuleServices.UpdateModule(module);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "WinPeModule";
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