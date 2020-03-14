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
    public class ComputerLogController : ApiController
    {
        private readonly ServiceComputerLog _computerLogService;

        public ComputerLogController()
        {
            _computerLogService = new ServiceComputerLog();
          
        }

        [Authorize]
        public DtoActionResult Delete(int id)
        {
            var result = _computerLogService.DeleteComputerLog(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [Authorize]
        public EntityComputerLog Get(int id)
        {
            var result = _computerLogService.GetComputerLog(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [Authorize]
        public DtoActionResult Post(EntityComputerLog computerLog)
        {
            return _computerLogService.AddComputerLog(computerLog);
        }

        [Authorize]
        public IEnumerable<EntityComputerLog> GetUnregLogs(int limit = 0)
        {
            return _computerLogService.SearchUnreg(limit);
        }
    }
}