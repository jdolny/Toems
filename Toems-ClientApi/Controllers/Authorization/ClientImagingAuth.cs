using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Toems_Service;

namespace Toems_ClientApi.Controllers.Authorization
{
    public class ClientImagingAuth : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var userToken = StringManipulationServices.Decode(HttpContext.Current.Request.Headers["Authorization"],
               "Authorization");
            var authResult = new ClientImagingServices().AuthorizeApiCall(userToken);
            if (!authResult.IsAuthorized)
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                throw new HttpResponseException(response);
            }
            if (authResult.UserType.Equals("user"))
                HttpContext.Current.Response.AddHeader("client_user_id", authResult.Id.ToString());

        }
    }
}