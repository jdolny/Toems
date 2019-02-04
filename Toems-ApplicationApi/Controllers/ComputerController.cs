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
    public class ComputerController : ApiController
    {
        private readonly ServiceComputer _computerServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ComputerController()
        {
            _computerServices = new ServiceComputer();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerArchive)]
        [HttpGet]
        public DtoActionResult Restore(int id)
        {
            var result = _computerServices.RestoreComputer(id);
            var computer = _computerServices.GetComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Restore;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerArchive)]
        [HttpGet]
        public DtoActionResult Archive(int id)
        {
            var computer = _computerServices.GetComputer(id);
            var result = _computerServices.ArchiveComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Archive;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerDelete)]
        public DtoActionResult Delete(int id)
        {
            var computer = _computerServices.GetComputer(id);
            var result = _computerServices.DeleteComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public EntityComputer Get(int id)
        {
            var result = _computerServices.GetComputer(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityComputer> Get()
        {
            return _computerServices.SearchComputers(new DtoSearchFilterCategories());
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityGroup> GetComputerGroups(int id)
        {
            return _computerServices.GetComputerGroups(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<DtoComputerPolicyHistory> GetPolicyHistory(int id)
        {
            return _computerServices.GetPolicyHistory(id);
        }

         [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetEffectivePolicy(int id, EnumPolicy.Trigger trigger, string comServerUrl)
        {
            var result = _computerServices.GetEffectivePolicy(id, trigger, comServerUrl);
            return new DtoApiStringResponse() {Value = result};
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityPolicy> GetComputerPolicies(int id)
        {
            return _computerServices.GetComputerPolicies(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<DtoModule> GetComputerModules(int id)
        {
            return _computerServices.GetComputerModules(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<DtoCustomComputerInventory> GetCustomInventory(int id)
        {
            return _computerServices.GetCustomInventory(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public IEnumerable<EntityCustomComputerAttribute> GetCustomAttributes(int id)
        {
            return _computerServices.GetCustomAttributes(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> Search(DtoSearchFilterCategories filter)
        {
            return _computerServices.SearchComputers(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> SearchAllComputers(DtoSearchFilterAllComputers filter)
        {
            return _computerServices.SearchAllComputers(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> GetArchived(DtoSearchFilterCategories filter)
        {
            return _computerServices.GetArchived(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> SearchPreProvision(DtoSearchFilter filter)
        {
            return _computerServices.SearchPreProvision(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> SearchForGroup(DtoSearchFilter filter)
        {
            return _computerServices.SearchForGroup(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetActiveCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.TotalActiveCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetAllCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.AllCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.TotalCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetArchivedCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.ArchivedCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetPreProvisionCount()
        {
            return new DtoApiStringResponse { Value = _computerServices.TotalPreProvisionCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerUpdate)]
        public DtoActionResult Post(EntityComputer computer)
        {
            return _computerServices.AddComputer(computer);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerUpdate)]
        public DtoActionResult Put(int id, EntityComputer computer)
        {
            computer.Id = id;
            var result = _computerServices.UpdateComputer(computer);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerSendMessage)]
        [HttpPost]
        public DtoApiBoolResponse SendMessage(int id, DtoMessage message)
        {
            var response = new DtoApiBoolResponse() {Value = _computerServices.SendMessage(id, message)};
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(message);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Message;
                _auditLogService.AddAuditLog(auditLog);
            }
            return response;
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerForceCheckin)]
        [HttpGet]
        public DtoApiBoolResponse ForceCheckin(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.ForceCheckin(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerForceCheckin)]
        [HttpGet]
        public DtoApiBoolResponse CollectInventory(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.CollectInventory(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        public DtoApiStringResponse GetLoggedInUsers(int id)
        {
            return new DtoApiStringResponse() { Value = _computerServices.GetLoggedInUsers(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        public DtoApiBoolResponse GetStatus(int id)
        {
            return new DtoApiBoolResponse() { Value = _computerServices.GetStatus(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerReboot)]
        [HttpGet]
        public DtoApiBoolResponse Reboot(int id)
        {
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Reboot;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _computerServices.Reboot(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerShutdown)]
        [HttpGet]
        public DtoApiBoolResponse Shutdown(int id)
        {
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Shutdown;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _computerServices.Shutdown(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerWakeup)]
        [HttpGet]
        public DtoApiBoolResponse Wakeup(int id)
        {
            var computer = _computerServices.GetComputer(id);
            if (computer != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Computer";
                auditLog.ObjectId = id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Wakeup;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _computerServices.Wakeup(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        public DtoInventoryCollection GetSystemInfo(int id)
        {
            return _computerServices.GetSystemInfo(id);
        }


        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        public IEnumerable<EntitySoftwareInventory> GetSoftware(int id, string searchstring = "")
        {
            return string.IsNullOrEmpty(searchstring)
                ? _computerServices.GetComputerSoftware(id)
                : _computerServices.GetComputerSoftware(id, searchstring);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        public IEnumerable<DtoComputerUpdates> GetUpdates(int id, string searchstring = "")
        {
            return string.IsNullOrEmpty(searchstring)
                ? _computerServices.GetUpdates(id)
                : _computerServices.GetUpdates(id, searchstring);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpGet]
        public IEnumerable<EntityUserLogin> GetUserLogins(int id, string searchstring = "")
        {
            return string.IsNullOrEmpty(searchstring)
                ? _computerServices.GetUserLogins(id)
                : _computerServices.GetUserLogins(id, searchstring);
        }

        [CustomAuth(Permission = AuthorizationStrings.CommentAdd)]
        [HttpPost]
        public DtoActionResult AddComment(DtoComputerComment comment)
        {
            return _computerServices.AddComment(comment, _userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.CommentRead)]
        public List<DtoComputerComment> GetComments(int id)
        {
            return _computerServices.GetComments(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.AttachmentRead)]
        public IEnumerable<EntityAttachment> GetAttachments(int id)
        {
            return _computerServices.GetAttachments(id);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithTime> ComputerProcessTimes(DateTime dateCutoff, int limit, int computerId)
        {
            return _computerServices.GetComputerProcessTimes(dateCutoff, limit, computerId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithCount> ComputerProcessCounts(DateTime dateCutoff, int limit, int computerId)
        {
            return _computerServices.GetComputerProcessCounts(dateCutoff, limit, computerId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithUser> ComputerProcess(DateTime dateCutoff, int limit, int computerId)
        {
            return _computerServices.GetAllProcessForComputer(dateCutoff, limit, computerId);
        }
    }
}