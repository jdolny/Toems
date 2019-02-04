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
    public class ResetRequestController : ApiController
    {
        private readonly ServiceResetRequest _resetRequestService;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ResetRequestController()
        {
           _resetRequestService = new ServiceResetRequest();
           _auditLogService = new ServiceAuditLog();
           _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
               .Select(c => c.Value).SingleOrDefault());
          
        }

         [CustomAuth(Permission = AuthorizationStrings.ApproveReset)]
        public DtoActionResult Delete(int id)
        {
            var result = _resetRequestService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

         [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        [HttpPost]
        public IEnumerable<EntityResetRequest> Search(DtoSearchFilter filter)
        {
            return _resetRequestService.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _resetRequestService.TotalCount()};
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ApproveReset)]
        public DtoActionResult Approve(int id)
        {
            var result = _resetRequestService.ApproveRequest(id);
            if (result.Success)
            {
                var computer = new ServiceComputer().GetComputer(id);
                if (computer == null) return result;
                var auditLog = new EntityAuditLog();
                auditLog.ObjectType = "ResetRequest";
                auditLog.ObjectId = result.Id;
                auditLog.ObjectName = computer.Name;
                auditLog.ObjectJson = JsonConvert.SerializeObject(computer);
                auditLog.UserId = _userId;
                auditLog.AuditType = EnumAuditEntry.AuditType.ApproveReset;
                _auditLogService.AddAuditLog(auditLog);
            }

            return result;
        }
    }
}