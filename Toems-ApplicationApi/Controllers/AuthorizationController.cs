using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Toems_Common.Dto;
using Toems_Service;

namespace Toems_ApplicationApi.Controllers
{
    public class AuthorizationController : ApiController
    {
        private readonly int _userId;

        public AuthorizationController()
        {
            _userId = Convert.ToInt32(((ClaimsIdentity) User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

      


     

        [Authorize]
        [HttpGet]
        public DtoApiBoolResponse IsAuthorized(string requiredRight)
        {
           
            return new DtoApiBoolResponse
            {
                Value = new AuthorizationServices(Convert.ToInt32(_userId), requiredRight).IsAuthorized()
            };
        }
    }
}