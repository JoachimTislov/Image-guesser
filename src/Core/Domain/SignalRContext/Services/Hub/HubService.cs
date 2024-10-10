using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SignalRContext.Services.Hub;

public class HubService(IHubContext<GameHub, IGameClient> hubContext) : IHubService
{
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public async Task RedirectGroupToPage(string sessionId, string link)
    {
        await _hubContext.Clients.Groups(sessionId).RedirectToLink(link);
    }

    public async Task RedirectClientToPage(string userId, string link)
    {
        await _hubContext.Clients.User(userId).RedirectToLink(link);
    }

    public async Task ReloadGroupPage(string sessionId)
    {
        await _hubContext.Clients.Groups(sessionId).ReloadPage();
    }

    public async Task ReloadClientPage(string userId)
    {
        await _hubContext.Clients.User(userId).ReloadPage();
    }
}