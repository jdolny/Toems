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
    public class ModuleController : ApiController
    {
        private readonly ServiceModule _moduleServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ModuleController()
        {
            _moduleServices = new ServiceModule();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleArchive)]
        [HttpGet]
        public DtoActionResult Restore(int moduleId, EnumModule.ModuleType moduleType)
        {
            var result = _moduleServices.RestoreModule(moduleId,moduleType);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                switch (moduleType)
                {
                    case EnumModule.ModuleType.Command:
                        var cModule = new ServiceCommandModule().GetModule(moduleId);
                        auditLog.ObjectName = cModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(cModule);
                        break;
                    case EnumModule.ModuleType.FileCopy:
                        var fModule = new ServiceFileCopyModule().GetModule(moduleId);
                        auditLog.ObjectName = fModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(fModule);
                        break;
                    case EnumModule.ModuleType.Printer:
                        var pModule = new ServicePrinterModule().GetModule(moduleId);
                        auditLog.ObjectName = pModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(pModule);
                        break;
                    case EnumModule.ModuleType.Script:
                        var scModule = new ServiceScriptModule().GetModule(moduleId);
                        auditLog.ObjectName = scModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(scModule);
                        break;
                    case EnumModule.ModuleType.Software:
                        var sModule = new ServiceSoftwareModule().GetModule(moduleId);
                        auditLog.ObjectName = sModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(sModule);
                        break;
                    case EnumModule.ModuleType.Wupdate:
                        var uModule = new ServiceWuModule().GetModule(moduleId);
                        auditLog.ObjectName = uModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(uModule);
                        break;
                    case EnumModule.ModuleType.Message:
                        var messageModule = new ServiceMessageModule().GetModule(moduleId);
                        auditLog.ObjectName = messageModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(messageModule);
                        break;
                    case EnumModule.ModuleType.WinPE:
                        var winPeModule = new ServiceWinPeModule().GetModule(moduleId);
                        auditLog.ObjectName = winPeModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(winPeModule);
                        break;
                }


                auditLog.ObjectType = moduleType.ToString();
                auditLog.ObjectId = result.Id;


                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Restore;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleArchive)]
        [HttpGet]
        public DtoActionResult Archive(int moduleId, EnumModule.ModuleType moduleType)
        {
            var result = _moduleServices.ArchiveModule(moduleId, moduleType);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                switch (moduleType)
                {
                    case EnumModule.ModuleType.Command:
                        var cModule = new ServiceCommandModule().GetModule(moduleId);
                        auditLog.ObjectName = cModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(cModule);
                        break;
                    case EnumModule.ModuleType.FileCopy:
                        var fModule = new ServiceFileCopyModule().GetModule(moduleId);
                        auditLog.ObjectName = fModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(fModule);
                        break;
                    case EnumModule.ModuleType.Printer:
                        var pModule = new ServicePrinterModule().GetModule(moduleId);
                        auditLog.ObjectName = pModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(pModule);
                        break;
                    case EnumModule.ModuleType.Script:
                        var scModule = new ServiceScriptModule().GetModule(moduleId);
                        auditLog.ObjectName = scModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(scModule);
                        break;
                    case EnumModule.ModuleType.Software:
                        var sModule = new ServiceSoftwareModule().GetModule(moduleId);
                        auditLog.ObjectName = sModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(sModule);
                        break;
                    case EnumModule.ModuleType.Wupdate:
                        var uModule = new ServiceWuModule().GetModule(moduleId);
                        auditLog.ObjectName = uModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(uModule);
                        break;
                    case EnumModule.ModuleType.Message:
                        var messageModule = new ServiceMessageModule().GetModule(moduleId);
                        auditLog.ObjectName = messageModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(messageModule);
                        break;
                    case EnumModule.ModuleType.WinPE:
                        var winPeModule = new ServiceWinPeModule().GetModule(moduleId);
                        auditLog.ObjectName = winPeModule.Name;
                        auditLog.ObjectJson = JsonConvert.SerializeObject(winPeModule);
                        break;
                }


                auditLog.ObjectType = moduleType.ToString();
                auditLog.ObjectId = result.Id;


                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Archive;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public List<EntityPolicy> GetModulePolicies(string moduleGuid)
        {
            return _moduleServices.GetModulePolicies(moduleGuid);
        }

         [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public List<EntityGroup> GetModuleGroups(string moduleGuid)
        {
            return _moduleServices.GetModuleGroups(moduleGuid);
        }

         [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public List<EntityComputer> GetModuleComputers(string moduleGuid)
        {
            return _moduleServices.GetModuleComputers(moduleGuid);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public List<EntityImage> GetModuleImages(string moduleGuid)
        {
            return _moduleServices.GetModuleImages(moduleGuid);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public List<EntityModuleCategory> GetModuleCategories(string moduleGuid)
        {
            return _moduleServices.GetModuleCategories(moduleGuid);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpGet]
        public DtoApiStringResponse IsModuleActive(string moduleGuid)
        {
            var f = new ServiceModule().GetModuleIdFromGuid(moduleGuid);
            var result = _moduleServices.IsModuleActive(f.moduleId, f.moduleType);
            return new DtoApiStringResponse() {Value = result};
        }


        

    }
}