using log4net;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class BrowserTokenController : ApiController
    {
        private readonly int _userId;
        private readonly ServiceBrowserToken _browserToken;
        public BrowserTokenController()
        {
            _browserToken = new ServiceBrowserToken();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());
        }

        [Authorize]
        public EntityBrowserToken GetToken()
        {
            return _browserToken.Create(_userId);
        }
    }
}