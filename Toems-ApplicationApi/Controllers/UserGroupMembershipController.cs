using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class UserGroupMembershipController : ApiController
    {
        private readonly ServiceUserGroupMembership _userGroupMembershipServices;

        public UserGroupMembershipController()
        {
            _userGroupMembershipServices = new ServiceUserGroupMembership();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(List<EntityUserGroupMembership> userGroupMemberships)
        {
            return _userGroupMembershipServices.AddMembership(userGroupMemberships);
        }

      
    }
}