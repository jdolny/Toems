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
    public class PolicyModulesController : ApiController
    {
        private readonly ServicePolicyModules _policyModulesServices;

        public PolicyModulesController()
        {
            _policyModulesServices = new ServicePolicyModules();
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _policyModulesServices.DeletePolicyModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyRead)]
        public EntityPolicyModules Get(int id)
        {
            var result = _policyModulesServices.GetPolicyModule(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Post(EntityPolicyModules policyModule)
        {
            return _policyModulesServices.AddPolicyModule(policyModule);
        }

        [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult PostList(List<EntityPolicyModules> policyModules)
        {
            return _policyModulesServices.AddPolicyModules(policyModules);
        }

       [CustomAuth(Permission = AuthorizationStrings.PolicyUpdate)]
        public DtoActionResult Put(int id, EntityPolicyModules policyModule)
        {
            policyModule.Id = id;
            var result = _policyModulesServices.UpdatePolicyModule(policyModule);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}