using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Http;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class WieBuildController : ApiController
    {
        private readonly ServiceWieBuild _wieBuildServices;


        public WieBuildController()
        {
            _wieBuildServices = new ServiceWieBuild();

        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public EntityWieBuild GetLastBuild()
        {
            return _wieBuildServices.GetLastBuildOptions();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse UpdateStatus()
        {
            _wieBuildServices.UpdateProcessStatus();
            return new DtoApiBoolResponse() { Value = true };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public List<DtoReplicationProcess> GetProcess()
        {
            var result = _wieBuildServices.GetWieProcess();
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

       


    }
}