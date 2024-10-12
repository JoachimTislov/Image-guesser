using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SignalRContext.Services.Hub;

public class HubService(IHubContext<GameHub, IGameClient> hubContext, IConnectionMappingService connectionMappingService) : IHubService
{
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));

    public async Task RedirectGroupToPage(string sessionId, string link)
    {
        await _hubContext.Clients.Groups(sessionId).RedirectToLink(link);
    }

    public async Task RedirectOthersInGroupToPage(string sessionId, string userId, string link)
    {
        var IgnoreThisConnection = _connectionMappingService.GetConnection(userId);
        await _hubContext.Clients.GroupExcept(sessionId, IgnoreThisConnection).RedirectToLink(link);
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

    public async Task ReloadConnectionPage(string connectionId)
    {
        await _hubContext.Clients.Client(connectionId).ReloadPage();
    }

    public async Task RemoveFromGroup(string connectionId, string sessionId)
    {
        await _hubContext.Groups.RemoveFromGroupAsync(connectionId, sessionId);
    }

    public async Task RemoveConnectionsFromGroup(HashSet<string> connectionIds, string sessionId)
    {
        foreach (var connection in connectionIds)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connection, sessionId);
        }
    }

    public async Task RemoveFromGroupAsync(string sessionId, string userId)
    {
        var connectionId = _connectionMappingService.GetConnection(userId);
        await _hubContext.Groups.RemoveFromGroupAsync(connectionId, sessionId);
        await _connectionMappingService.RemoveFromGroup(sessionId, connectionId);
    }

}