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
    public class PolicyComServerController : ApiController
    {
        private readonly ServicePolicyComServer _policyComServerServices;

        public PolicyComServerController()
        {
            _policyComServerServices = new ServicePolicyComServer();
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Post(List<EntityPolicyComServer> policyComServers)
        {
            return _policyComServerServices.AddOrUpdate(policyComServers);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _policyComServerServices.DeleteAllForPolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


    }
}