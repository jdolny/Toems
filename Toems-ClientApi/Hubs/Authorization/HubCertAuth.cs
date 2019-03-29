using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Toems_ClientApi.Hubs.Authorization
{
    public class HubCertAuth : AuthorizeAttribute
    {

        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            object cert = null;
            if (request.Environment.TryGetValue("ssl.ClientCertificate", out cert) ||
                !(cert is X509Certificate))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override bool UserAuthorized(System.Security.Principal.IPrincipal user)
        {

            return true;
        }

    }
}