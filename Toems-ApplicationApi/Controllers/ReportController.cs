using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class ReportController : ApiController
    {
        private readonly ServiceReport _reportService;

        public ReportController()
        {
            _reportService = new ServiceReport();
        }
      

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public IEnumerable<DtoComputerUserLogins> GetUserLogins(string searchString)
        {
            return _reportService.GetUserLogins(searchString);
        }

        [HttpPost]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public DtoApiStringResponse GetCustomComputer(List<DtoCustomComputerQuery> queries)
        {
            var result = _reportService.GetInventory(queries);
            return new DtoApiStringResponse() { Value = JsonConvert.SerializeObject(result) };
        }
        

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithTime> TopProcessTimes(DateTime dateCutoff, int limit)
        {
            return _reportService.GetTopProcessTimes(dateCutoff,limit);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public List<DtoProcessWithCount> TopProcessCounts(DateTime dateCutoff, int limit)
        {
            return _reportService.GetTopProcessCounts(dateCutoff, limit);
        }

        [Authorize]
        public DtoApiStringResponse GetCheckinCounts()
        {
            return _reportService.GetCheckinCounts();
        }


    }
}