using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using log4net;
using Toems_Common.Dto;
using Toems_Service;

namespace Toems_ApplicationApi.Controllers.Authorization
{
    public class CustomAuthAttribute : AuthorizeAttribute
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(CustomAuthAttribute));
        public string Permission { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authorized = false;
            _log.Debug(actionContext.Request.RequestUri);
            base.OnAuthorization(actionContext);
            var identity = (ClaimsPrincipal) Thread.CurrentPrincipal;
            var userId = identity.Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault();

            var mfaSetupRequired = identity.Claims.Where(c => c.Type == "mfa_setup_required")
                .Select(c => c.Value).SingleOrDefault();
            if (mfaSetupRequired == null)
                mfaSetupRequired = "false";

            if (userId == null || mfaSetupRequired.Equals("true"))
                authorized = false;
            else
                authorized = new AuthorizationServices(Convert.ToInt32(userId), Permission).IsAuthorized();
           

            if (!authorized)
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                    new DtoValidationResult {Success = false, ErrorMessage = "Not Authorized"});
                throw new HttpResponseException(response);
            }
        }

  
    }
}