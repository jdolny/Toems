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
    public class PolicyCategoryController : ApiController
    {
        private readonly ServicePolicyCategory _policyCategoryServices;

        public PolicyCategoryController()
        {
            _policyCategoryServices = new ServicePolicyCategory();
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Post(List<EntityPolicyCategory> policyCategories)
        {
            return _policyCategoryServices.AddOrUpdate(policyCategories);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _policyCategoryServices.DeleteAllForPolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


    }
}