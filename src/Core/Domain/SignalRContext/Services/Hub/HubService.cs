
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SignalRContext.Services.Hub;

public class HubService(IHubContext<GameHub, IGameClient> hubContext, ISessionService sessionService, IConnectionMappingService connectionMappingService) : IHubService
{
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));

    private async Task RemoveFromGroups(string userConnection, string userId, string sessionId)
    {
        await _hubContext.Groups.RemoveFromGroupAsync(userConnection, sessionId);
        await _connectionMappingService.RemoveFromGroup(userId, sessionId);
    }

    public async Task LeaveGroup(string userId, string sessionId)
    {
        string userConnection = _connectionMappingService.GetConnection(userId);
        await RemoveFromGroups(userConnection, userId, sessionId);

        await RedirectClientToPage(userConnection, "/");
        // Allows us to bypass the need for extensive front-end logic that is already handled by the back-end
        await _hubContext.Clients.Groups(sessionId).ReloadPage();
    }

    public async Task CloseGroup(string sessionId)
    {
        var sessionUsers = await _sessionService.GetUsersInSessionById(Guid.Parse(sessionId));
        foreach (var user in sessionUsers)
        {
            // Remove User from group
            await LeaveGroup(user.Id.ToString(), sessionId);
        }
    }

    public async Task RedirectGroupToPage(string sessionId, string link)
    {
        await _hubContext.Clients.Group(sessionId).RedirectToLink(link);
    }

    public async Task RedirectClientToPage(string userConnection, string link)
    {
        await _hubContext.Clients.Client(userConnection).RedirectToLink(link);
    }
}