using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using MediatR;

namespace Image_guesser.Core.Domain.SignalRContext.Handlers;

public class SessionClosedHandler(IHubService hubService, IConnectionMappingService connectionMappingService) : INotificationHandler<SessionClosed>
{
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));

    public async Task Handle(SessionClosed notification, CancellationToken cancellationToken)
    {
        var sessionId = notification.SessionId;

        var groupConnections = _connectionMappingService.GetGroupConnections(sessionId.ToString());
        if (groupConnections != null)
        {
            await _hubService.RemoveConnectionsFromGroup(groupConnections, sessionId.ToString());
        }

        await _connectionMappingService.DeleteGroup(sessionId.ToString());
    }
}