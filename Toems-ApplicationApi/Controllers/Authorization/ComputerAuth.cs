using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.UI;
using log4net;
using Toems_Common.Dto;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers.Authorization
{
    public class ComputerAuthAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authorized = false;
            base.OnAuthorization(actionContext);
            var identity = (ClaimsPrincipal) Thread.CurrentPrincipal;
            var userId = identity.Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault();
            var computerId = actionContext.RequestContext.RouteData.Values["id"];

            var computerAcls = new ServiceUser().GetAllowedComputers(Convert.ToInt32(userId));
            if(!computerAcls.ComputerManagementEnforced)
                authorized = true;
            else if (computerAcls.AllowedComputerIds.Contains(Convert.ToInt32(computerId)))
                authorized = true;
            else
                authorized = false;
           

            if (!authorized)
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                    new DtoValidationResult {Success = false, ErrorMessage = "Not Authorized To Access This Computer"});
                throw new HttpResponseException(response);
            }
        }

  
    }
}