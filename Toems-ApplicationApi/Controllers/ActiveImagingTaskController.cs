using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class ActiveImagingTaskController : ApiController
    {
        private readonly ServiceActiveImagingTask _activeImagingTaskService;
        private readonly ServiceAuditLog _auditLogService;
        private readonly int _userId;

        public ActiveImagingTaskController()
        {
            _activeImagingTaskService = new ServiceActiveImagingTask();
            _auditLogService = new ServiceAuditLog();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());

        }


        [Authorize]
        public DtoActionResult Delete(int id)
        {
            var result = _activeImagingTaskService.DeleteActiveImagingTask(id);
            if (result.Id == 0)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, result));
            return result;
        }

        [Authorize]
        public DtoActionResult DeleteOnDemand(int id)
        {
            var result = _activeImagingTaskService.DeleteUnregisteredOndTask(id);
            if (result.Id == 0)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, result));
            return result;
        }

        [Authorize]
        public DtoApiStringResponse GetActiveNotOwned()
        {
            return new DtoApiStringResponse
            {
                Value = _activeImagingTaskService.ActiveCountNotOwnedByuser(Convert.ToInt32(_userId))
            };
        }

        [Authorize]
        [HttpGet]
        public DtoApiBoolResponse CancelAllImagingTasks()
        {
            return new DtoApiBoolResponse
            {
                Value = new CancelAllImagingTasks().RunAllServers()
            };
        }

        [Authorize]
        public IEnumerable<TaskWithComputer> GetActiveTasks()
        {
            return _activeImagingTaskService.ReadAll(Convert.ToInt32(_userId));
        }

        [Authorize]
        public DtoApiStringResponse GetActiveUnicastCount(string taskType)
        {
            return new DtoApiStringResponse
            {
                Value = _activeImagingTaskService.ActiveUnicastCount(Convert.ToInt32(_userId), taskType)
            };
        }

        [Authorize]
        public DtoApiStringResponse GetAllActiveCount()
        {
            return new DtoApiStringResponse
            {
                Value = _activeImagingTaskService.AllActiveCount(Convert.ToInt32(_userId))
            };
        }

        [Authorize]
        public IEnumerable<EntityActiveImagingTask> GetAllOnDemandUnregistered()
        {
            return _activeImagingTaskService.GetAllOnDemandUnregistered();
        }


        [Authorize]
        public IEnumerable<TaskWithComputer> GetUnicasts()
        {
            return _activeImagingTaskService.ReadUnicasts(Convert.ToInt32(_userId));
        }

        [Authorize]
        [HttpGet]
        public DtoApiIntResponse OnDemandCount()
        {
            return new DtoApiIntResponse { Value = _activeImagingTaskService.OnDemandCount() };
        }

    }
}