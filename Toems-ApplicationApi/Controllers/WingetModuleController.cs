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
    public class WingetModuleController : ApiController
    {
        private readonly ServiceWingetModule _wingetModuleServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public WingetModuleController()
        {
            _wingetModuleServices = new ServiceWingetModule();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleDelete)]
        public DtoActionResult Delete(int id)
        {
            var module = _wingetModuleServices.GetModule(id);
            var result = _wingetModuleServices.DeleteModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "WingetModule";
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
        public EntityWingetModule Get(int id)
        {
            var result = _wingetModuleServices.GetModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public IEnumerable<EntityWingetModule> Get()
        {
            return _wingetModuleServices.SearchModules(new DtoSearchFilterCategories());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityWingetModule> Search(DtoSearchFilterCategories filter)
        {
            return  _wingetModuleServices.SearchModules(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityWingetModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return  _wingetModuleServices.GetArchived(filter);
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _wingetModuleServices.TotalCount() };
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
       public DtoApiStringResponse GetArchivedCount()
       {
           return new DtoApiStringResponse { Value = _wingetModuleServices.ArchivedCount() };
       }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Post(EntityWingetModule module)
        {
            var result = _wingetModuleServices.AddModule(module);
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "WingetModule";
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
        public DtoActionResult Put(int id, EntityWingetModule module)
        {
            module.Id = id;
            var result = _wingetModuleServices.UpdateModule(module);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "WingetModule";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = module.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(module);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityWingetLocaleManifest> SearchManifests(DtoWingetSearchFilter filter)
        {
            return _wingetModuleServices.SearchManifests(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public EntityWingetLocaleManifest GetLocaleManifest(int id)
        {
            return _wingetModuleServices.GetLocaleManifest(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetLastWingetImportTime()
        {
            return new DtoApiStringResponse { Value = _wingetModuleServices.GetLastImportTime() };
        }
    }
}