using Microsoft.AspNetCore.SignalR;
using Toems_Common.Dto;

namespace Toems_ClientApi.Hubs;

public class ActionHubService
{
    private readonly IHubContext<ActionHub> _hub;

    public ActionHubService(IHubContext<ActionHub> hub)
    {
        _hub = hub;
    }

    public async Task SendAction(DtoSocketRequest request)
    {
        var action = new DtoHubAction
        {
            Action = request.action,
            Message = request.message
        };

        await _hub.Clients.Clients(request.connectionIds).SendAsync("ClientAction", action);
    }
}