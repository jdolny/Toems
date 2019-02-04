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
    public class PrinterModuleController : ApiController
    {
        private readonly ServicePrinterModule _printerModuleServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public PrinterModuleController()
        {
            _printerModuleServices = new ServicePrinterModule();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleDelete)]
        public DtoActionResult Delete(int id)
        {
            var module = _printerModuleServices.GetModule(id);
            var result = _printerModuleServices.DeleteModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "PrinterModule";
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
        public EntityPrinterModule Get(int id)
        {
            var result = _printerModuleServices.GetModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public IEnumerable<EntityPrinterModule> Get()
        {
            return _printerModuleServices.SearchModules(new DtoSearchFilterCategories());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityPrinterModule> Search(DtoSearchFilterCategories filter)
        {
            return _printerModuleServices.SearchModules(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityPrinterModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return _printerModuleServices.GetArchived(filter);
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _printerModuleServices.TotalCount() };
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
       public DtoApiStringResponse GetArchivedCount()
       {
           return new DtoApiStringResponse { Value = _printerModuleServices.ArchivedCount() };
       }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Post(EntityPrinterModule module)
        {
            var result = _printerModuleServices.AddModule(module);
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "PrinterModule";
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
        public DtoActionResult Put(int id, EntityPrinterModule module)
        {
            module.Id = id;
            var result = _printerModuleServices.UpdateModule(module);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "PrinterModule";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = module.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(module);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Create;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }
    }
}