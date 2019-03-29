using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Microsoft.AspNet.SignalR;
using Toems_ClientApi.Hubs.Authorization;
using Toems_Common.Dto;

namespace Toems_ClientApi.Hubs
{
    //[HubCertAuth]
    public class ActionHub : Hub
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;
            var compId = Context.QueryString["computerGuid"];
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                // We know that Stop() was called on the client,
                // and the connection shut down gracefully.
            }
            else
            {
                // This server hasn't heard from the client in the last ~35 seconds.
                // If SignalR is behind a load balancer with scaleout configured, 
                // the client may still be connected to another SignalR server.
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }

        public void SendAction()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ActionHub>();
            var hubAction = new DtoHubAction();
            hubAction.Action = "test";
            hubAction.Message = "message";
            context.Clients.All.ClientAction(hubAction);
            //Clients.Client(connectionId).doSomething();
        }
    }
}