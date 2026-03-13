using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ClientApi.Controllers.Authorization
{
    public class ClientImagingAuthFilter(ServiceContext ctx) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            var authHeader = request.Headers["Authorization"].FirstOrDefault();

            var userToken = StringManipulationServices.Decode(authHeader, "Authorization");

            var authResult = ctx.ClientImaging.AuthorizeApiCall(userToken);

            if (!authResult.IsAuthorized)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            if (authResult.UserType.Equals("user"))
            {
                context.HttpContext.Response.Headers["client_user_id"] = authResult.Id.ToString();
            }
        }
    }
}