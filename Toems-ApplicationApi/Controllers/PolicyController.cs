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
using Toems_Common.Dto.exports;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class PolicyController : ApiController
    {
        private readonly ServicePolicy _policyServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public PolicyController()
        {
            _policyServices = new ServicePolicy();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }
        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        [HttpGet]
        public DtoActionResult Clone(int id)
        {
            var result = _policyServices.ClonePolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyArchive)]
        [HttpGet]
        public DtoActionResult Archive(int id)
        {
            var policy = _policyServices.GetPolicy(id);
            var result = _policyServices.ArchivePolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Policy";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = policy.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(policy);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Archive;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyArchive)]
        [HttpGet]
        public DtoActionResult Restore(int id)
        {
            var policy = _policyServices.GetPolicy(id);
            var result = _policyServices.RestorePolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Policy";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = policy.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(policy);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Restore;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyDelete)]
        public DtoActionResult Delete(int id)
        {
            var policy = _policyServices.GetPolicy(id);
            var result = _policyServices.DeletePolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Policy";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = policy.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(policy);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public EntityPolicy Get(int id)
        {
            var result = _policyServices.GetPolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public EntityActiveClientPolicy GetActiveStatus(int id)
        {
            return _policyServices.GetActivePolicy(id);
          
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public IEnumerable<EntityPolicy> Get()
        {
            var policyFilter = new DtoSearchFilterCategories();
            return _policyServices.SearchPolicies(policyFilter);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public IEnumerable<EntityGroup> GetPolicyGroups(int id)
        {
            return _policyServices.GetPolicyGroups(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyImport)]
        [HttpPost]
        public DtoImportResult ImportPolicy(DtoPolicyExport export)
        {
            return new ImportPolicy(export).Import();
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyExport)]
        [HttpPost]
        public DtoActionResult ValidatePolicyExport(DtoPolicyExportGeneral exportInfo)
        {
            return _policyServices.ValidatePolicyExport(exportInfo);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyExport)]
        [HttpPost]
        public DtoPolicyExport ExportPolicy(DtoPolicyExportGeneral exportInfo)
        {
            return new ExportPolicy().Export(exportInfo);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public IEnumerable<EntityPolicyComServer> GetPolicyComServers(int id)
        {
            return _policyServices.GetPolicyComServers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public IEnumerable<EntityPolicyCategory> GetPolicyCategories(int id)
        {
            return _policyServices.GetPolicyCategories(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public IEnumerable<EntityComputer> GetPolicyComputers(int id)
        {
            return _policyServices.GetPolicyComputers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        [HttpPost]
        public IEnumerable<EntityPolicyHistory> GetHistoryWithComputer(int id, DtoSearchFilter filter)
        {
            return _policyServices.GetHistoryWithComputer(id,filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public IEnumerable<EntityPolicyHashHistory> GetPolicyHashHistory(int id)
        {
            return _policyServices.GetHashHistory(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public DtoApiStringResponse GetHashDetail(int id, string hash)
        {
            var result = _policyServices.GetHashDetail(id, hash);
            return new DtoApiStringResponse() {Value = result};
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        [HttpPost]
        public IEnumerable<EntityPolicy> Search(DtoSearchFilterCategories filter)
        {
            return _policyServices.SearchPolicies(filter);
        }


        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        [HttpPost]
        public IEnumerable<EntityPolicy> GetArchived(DtoSearchFilterCategories filter)
        {
            return _policyServices.GetArchived(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        [HttpPost]
        public IEnumerable<DtoModule> GetAllModules(DtoModuleSearchFilter filter)
        {
            return _policyServices.AllAvailableModules(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        [HttpPost]
        public IEnumerable<EntityPolicyModules> GetAssignedModules(int id, DtoModuleSearchFilter filter)
        {
            return _policyServices.SearchAssignedPolicyModules(id,filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public DtoApiStringResponse GetAssignedModuleCount(int id)
        {
            return new DtoApiStringResponse { Value = _policyServices.AssignedModuleCount(id) };
        }

       [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _policyServices.TotalCount() };
        }

       [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
       public DtoApiStringResponse GetArchivedCount()
       {
           return new DtoApiStringResponse { Value = _policyServices.ArchivedCount() };
       }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Post(EntityPolicy policy)
        {
            var result = _policyServices.AddPolicy(policy);
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Policy";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = policy.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(policy);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Create;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Put(int id, EntityPolicy policy)
        {
            policy.Id = id;
            var result = _policyServices.UpdatePolicy(policy);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Policy";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = policy.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(policy);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyActivate)]
        [HttpGet]
        public DtoActionResult ActivatePolicy(int id, bool reRunExisting)
        {
            var policy = _policyServices.GetPolicy(id);
            var result = _policyServices.ActivatePolicy(id,reRunExisting);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Policy";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = policy.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(policy);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.ActivatePolicy;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyActivate)]
        [HttpGet]
        public DtoActionResult DeactivatePolicy(int id)
        {
            var policy = _policyServices.GetPolicy(id);
            var result = _policyServices.DeactivatePolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Policy";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = policy.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(policy);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.DeactivatePolicy;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        [HttpGet]
        public DtoApiStringResponse PolicyChangedSinceActivation(int id)
        {
            var result = _policyServices.PolicyChangedSinceLastActivation(id);
            return new DtoApiStringResponse() {Value = result};
        }

        [Authorize]
        public IEnumerable<DtoPinnedPolicy> GetAllActiveStatus()
        {
            return _policyServices.GetAllActiveStatus();
        }
    }
}