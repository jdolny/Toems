using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class UserRightController : ApiController
    {
        private readonly ServiceUserRight _userRightServices;

        public UserRightController()
        {
            _userRightServices = new ServiceUserRight();
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(List<EntityUserRight> listOfRights)
        {
            return _userRightServices.AddUserRights(listOfRights);
        }
    }
}