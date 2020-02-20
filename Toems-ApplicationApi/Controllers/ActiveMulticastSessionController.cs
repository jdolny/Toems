using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class ActiveMulticastSessionController : ApiController
    {
        private readonly ServiceActiveMulticastSession _activeMulticastSessionService;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ActiveMulticastSessionController()
        {
            _activeMulticastSessionService = new ServiceActiveMulticastSession();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());

        }

        [Authorize]
        public DtoActionResult Delete(int id)
        {
            var result = _activeMulticastSessionService.Delete(id);
            if (result.Id == 0)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, result));
            return result;
        }

        [Authorize]
        public IEnumerable<EntityActiveMulticastSession> Get()
        {
            return _activeMulticastSessionService.GetAllMulticastSessions(Convert.ToInt32(_userId));
        }

        [Authorize]
        public IEnumerable<EntityComputer> GetComputers(int id)
        {
            return new ServiceActiveImagingTask().GetMulticastComputers(id);
        }

        [Authorize]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse
            {
                Value = _activeMulticastSessionService.ActiveCount(Convert.ToInt32(_userId))
            };
        }

        [Authorize]
        public IEnumerable<TaskWithComputer> GetMemberStatus(int id)
        {
            return new ServiceActiveImagingTask().MulticastMemberStatus(id);
        }

        [Authorize]
        public IEnumerable<EntityActiveImagingTask> GetProgress(int id)
        {
            return new ServiceActiveImagingTask().MulticastProgress(id);
        }

        [Authorize]
        [HttpGet]
        public DtoApiStringResponse StartOnDemandMulticast(int profileId, string clientCount, string sessionName, int comServerId)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var userId = identity.Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault();

            return new DtoApiStringResponse
            {
                Value =
                    new Multicast(profileId, clientCount, sessionName, Convert.ToInt32(userId), comServerId).Create()
            };
        }
    }
}