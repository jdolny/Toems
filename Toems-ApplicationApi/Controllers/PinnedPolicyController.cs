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
    public class PinnedPolicyController : ApiController
    {
        private readonly ServicePinnedPolicy _pinnedPolicyService;

        public PinnedPolicyController()
        {
            _pinnedPolicyService = new ServicePinnedPolicy();
          
        }

         [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public DtoActionResult Delete(int policyId, int userId)
        {
            var result = _pinnedPolicyService.Delete(policyId,userId);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public DtoActionResult Post(EntityPinnedPolicy pinnedPolicy)
        {
            return _pinnedPolicyService.Add(pinnedPolicy);
        }

       
    }
}