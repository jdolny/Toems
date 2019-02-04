using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class ScheduleController : ApiController
    {
        private readonly ServiceSchedule _scheduleService;

        public ScheduleController()
        {
            _scheduleService = new ServiceSchedule();
          
        }

         [CustomAuth(Permission = AuthorizationStrings.ScheduleDelete)]
        public DtoActionResult Delete(int id)
        {
            var result = _scheduleService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ScheduleRead)]
        public EntitySchedule Get(int id)
        {
            var result = _scheduleService.GetSchedule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [Authorize]
        public IEnumerable<EntitySchedule> Get()
        {
            return _scheduleService.GetAll();
        }

         [CustomAuth(Permission = AuthorizationStrings.ScheduleRead)]
        [HttpPost]
        public IEnumerable<EntitySchedule> Search(DtoSearchFilter filter)
        {
            return _scheduleService.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ScheduleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _scheduleService.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.ScheduleRead)]
        public IEnumerable<EntityPolicy> GetSchedulePolicies(int id, string type)
        {
            return _scheduleService.GetSchedulePolicies(id, type);
        }

        [CustomAuth(Permission = AuthorizationStrings.ScheduleRead)]
        public IEnumerable<EntityGroup> GetScheduleGroups(int id, string type)
        {
            return _scheduleService.GetScheduleGroups(id, type);
        }

        [CustomAuth(Permission = AuthorizationStrings.ScheduleRead)]
        public IEnumerable<EntityComputer> GetScheduleComputers(int id, string type)
        {
            return _scheduleService.GetScheduleComputers(id, type);
        }

        [CustomAuth(Permission = AuthorizationStrings.ScheduleUpdate)]
        public DtoActionResult Post(EntitySchedule schedule)
        {
            return _scheduleService.Add(schedule);
        }

        [CustomAuth(Permission = AuthorizationStrings.ScheduleUpdate)]
        public DtoActionResult Put(int id, EntitySchedule schedule)
        {
            schedule.Id = id;
            var result = _scheduleService.Update(schedule);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}