using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class RemoteAccessController : ApiController
    {
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse VerifyRemoteAccessInstalled(int id)
        {
            var response = new ServiceRemoteAccess().VerifyRemoteAccessInstalled(id);
            return new DtoApiBoolResponse() { Value = response };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoActionResult InitializeRemotelyServer(int id)
        {
            return new ServiceRemoteAccess().InitializeRemotelyServer(id);
        }
    }
}