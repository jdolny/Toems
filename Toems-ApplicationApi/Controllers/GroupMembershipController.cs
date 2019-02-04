using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class GroupMembershipController : ApiController
    {
        private readonly ServiceGroupMembership _groupMembershipServices;

        public GroupMembershipController()
        {
            _groupMembershipServices = new ServiceGroupMembership();
        }

        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        public DtoActionResult Post(List<EntityGroupMembership> groupMemberships)
        {
            return _groupMembershipServices.AddMembership(groupMemberships);
        }

      
    }
}