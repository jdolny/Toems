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
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class GroupController : ApiController
    {
        private readonly ServiceGroup _groupServices;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public GroupController()
        {
            _groupServices = new ServiceGroup();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupDelete)]
        public DtoActionResult Delete(int id)
        {
            var group = _groupServices.GetGroup(id);
            var result = _groupServices.DeleteGroup(id);
            if (result == null) return new DtoActionResult() { ErrorMessage = "Result Was Null" };
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Group";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = group.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(group);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Delete;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public EntityGroup Get(int id)
        {
            var result = _groupServices.GetGroup(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public IEnumerable<EntityGroupCategory> GetGroupCategories(int id)
        {
            return _groupServices.GetGroupCategories(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public IEnumerable<DtoGroupWithCount> Get()
        {
            return _groupServices.SearchGroups(new DtoSearchFilterCategories());
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        [HttpGet]
        public DtoActionResult ClearImagingIds(int id)
        {
            var result = _groupServices.ClearImagingIds(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public IEnumerable<EntityGroup> GetAdGroups()
        {
            return _groupServices.GetAllAdGroups();
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public IEnumerable<EntityGroup> GetOuGroups()
        {
            return _groupServices.GetAllOuGroups();
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public IEnumerable<EntityGroup> GetAdSecurityGroups()
        {
            return _groupServices.GetAllAdSecurityGroups();
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        [HttpPost]
        public IEnumerable<DtoGroupWithCount> Search(DtoSearchFilterCategories filter)
        {
            return _groupServices.SearchGroups(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _groupServices.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerSendMessage)]
        [HttpPost]
        public DtoApiBoolResponse SendMessage(int id, DtoMessage message)
        {
            var response = new DtoApiBoolResponse() { Value = _groupServices.SendMessage(id, message) };
            var group = _groupServices.GetGroup(id);
            if (group != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Group";
                auditLog.ObjectId = id;
                auditLog.ObjectName = group.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(message);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Message;
                _auditLogService.AddAuditLog(auditLog);
            }
            return response;

        }

        [CustomAuth(Permission = AuthorizationStrings.GroupReboot)]
        [HttpGet]
        public DtoApiBoolResponse Reboot(int id)
        {
            var group = _groupServices.GetGroup(id);
            if (group != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Group";
                auditLog.ObjectId = id;
                auditLog.ObjectName = group.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(group);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Reboot;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _groupServices.Reboot(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupShutdown)]
        [HttpGet]
        public DtoApiBoolResponse Shutdown(int id)
        {
            var group = _groupServices.GetGroup(id);
            if (group != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Group";
                auditLog.ObjectId = id;
                auditLog.ObjectName = group.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(group);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Shutdown;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _groupServices.Shutdown(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupWakeup)]
        [HttpGet]
        public DtoApiBoolResponse Wakeup(int id)
        {
            var group = _groupServices.GetGroup(id);
            if (group != null)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Group";
                auditLog.ObjectId = id;
                auditLog.ObjectName = group.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(group);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Wakeup;
                _auditLogService.AddAuditLog(auditLog);
            }
            return new DtoApiBoolResponse() { Value = _groupServices.Wakeup(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerForceCheckin)]
        [HttpGet]
        public DtoApiBoolResponse ForceCheckin(int id)
        {
            return new DtoApiBoolResponse() { Value = _groupServices.ForceCheckin(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerForceCheckin)]
        [HttpGet]
        public DtoApiBoolResponse CollectInventory(int id)
        {
            return new DtoApiBoolResponse() { Value = _groupServices.CollectInventory(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        public DtoActionResult Post(EntityGroup group)
        {
            var result = _groupServices.AddGroup(group);
            if (result == null) return new DtoActionResult() {ErrorMessage = "Result Was Null"};

            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Group";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = group.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(group);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Create;
                _auditLogService.AddAuditLog(auditLog);
                
            }

            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        public DtoActionResult Put(int id, EntityGroup group)
        {
            group.Id = id;
            var result = _groupServices.UpdateGroup(group);
            if (result == null) return new DtoActionResult() { ErrorMessage = "Result Was Null" };
            if (result.Success)
            {
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "Group";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = group.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(group);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.Update;
                _auditLogService.AddAuditLog(auditLog);

            }
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        [HttpPost]
        public IEnumerable<GroupPolicyDetailed> GetAssignedPolicies(int id, DtoSearchFilter filter)
        {
            return _groupServices.GetAssignedPolicies(id, filter);
        }


        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public DtoApiStringResponse GetDynamic(List<EntitySmartGroupQuery> queries)
        {
            var result = _groupServices.GetDynamicMembers(queries);
            return new DtoApiStringResponse() {Value = JsonConvert.SerializeObject(result)};
        }

        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        public DtoApiBoolResponse UpdateDynamicQuery(List<EntitySmartGroupQuery> queries)
        {
            var result = _groupServices.UpdateDynamicQuery(queries);
            return new DtoApiBoolResponse() { Value = result };
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public IEnumerable<EntitySmartGroupQuery> GetDynamicQuery(int id)
        {
            return _groupServices.GetDynamicQuery(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        [HttpPost]
        public IEnumerable<EntityComputer> GetGroupMembers(int id, DtoSearchFilter filter)
        {
            return _groupServices.SearchGroupMembers(id, filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupRead)]
        public DtoApiStringResponse GetMemberCount(int id)
        {
            return new DtoApiStringResponse {Value = _groupServices.GetGroupMemberCount(id)};
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        public DtoApiBoolResponse RemoveGroupMember(int id, int computerId)
        {
            return new DtoApiBoolResponse {Value = _groupServices.DeleteMembership(computerId, id)};
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithTime> GroupProcessTimes(DateTime dateCutoff, int limit, int groupId)
        {
            return _groupServices.GetGroupProcessTimes(dateCutoff, limit, groupId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithCount> GroupProcessCounts(DateTime dateCutoff, int limit, int groupId)
        {
            return _groupServices.GetGroupProcessCounts(dateCutoff, limit,groupId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ImageDeployTask)]
        public DtoApiIntResponse StartGroupUnicast(int id)
        {
            return new DtoApiIntResponse
            {
                Value = _groupServices.StartGroupUnicast(id, Convert.ToInt32(_userId))
            };
        }

        [CustomAuth(Permission = AuthorizationStrings.ImageMulticastTask)]
        [HttpGet]
        public DtoApiStringResponse StartMulticast(int id)
        {
            return new DtoApiStringResponse
            {
                Value = new Multicast(id, Convert.ToInt32(_userId)).Create()
            };
        }


    }
}