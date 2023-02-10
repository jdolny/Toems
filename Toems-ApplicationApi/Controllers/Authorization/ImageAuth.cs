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
    public class ImageAuthAttribute : AuthorizeAttribute
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ImageAuthAttribute));

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authorized = false;
            base.OnAuthorization(actionContext);
            var identity = (ClaimsPrincipal) Thread.CurrentPrincipal;
            var userId = identity.Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault();
            var imageId = actionContext.RequestContext.RouteData.Values["id"];

            var imageAcls = new ServiceUser().GetAllowedImages(Convert.ToInt32(userId));
            if(!imageAcls.ImageManagementEnforced)
                authorized = true;
            else if (imageAcls.AllowedImageIds.Contains(Convert.ToInt32(imageId)))
                authorized = true;
            else
                authorized = false;
           

            if (!authorized)
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                    new DtoValidationResult {Success = false, ErrorMessage = "Not Authorized To Access This Image"});
                throw new HttpResponseException(response);
            }
        }

  
    }
}