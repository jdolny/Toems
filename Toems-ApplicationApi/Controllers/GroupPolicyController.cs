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
    public class GroupPolicyController : ApiController
    {
        private readonly ServiceGroupPolicy _groupPolicyServices;

        public GroupPolicyController()
        {
            _groupPolicyServices = new ServiceGroupPolicy();
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupPolicyUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _groupPolicyServices.DeleteGroupPolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupPolicyRead)]
        public EntityGroupPolicy Get(int id)
        {
            var result = _groupPolicyServices.GetGroupPolicy(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupPolicyUpdate)]
        public DtoActionResult Post(EntityGroupPolicy groupPolicy)
        {
            return _groupPolicyServices.AddGroupPolicy(groupPolicy);
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupPolicyUpdate)]
        public DtoActionResult PostList(List<EntityGroupPolicy> groupPolicies)
        {
            return _groupPolicyServices.AddGroupPolicies(groupPolicies);
        }

      [CustomAuth(Permission = AuthorizationStrings.GroupPolicyUpdate)]
        public DtoActionResult Put(int id, EntityGroupPolicy groupPolicy)
        {
            groupPolicy.Id = id;
            var result = _groupPolicyServices.UpdateGroupPolicy(groupPolicy);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}