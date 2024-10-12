using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class SessionClosedHandler(ISessionService sessionService, IHubService hubService, IConnectionMappingService connectionMappingService) : INotificationHandler<SessionClosed>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));

    public async Task Handle(SessionClosed notification, CancellationToken cancellationToken)
    {
        var sessionId = notification.SessionId;

        var session = await _sessionService.GetSessionById(sessionId);

        if (session.Options.AmountOfGamesPlayed == 0)
        {
            await _sessionService.DeleteSessionById(sessionId);
        }
        else
        {
            session.SessionUsers.Clear();
            session.CloseLobby();

            await _sessionService.UpdateSession(session);
        }

        var groupConnections = _connectionMappingService.GetGroupConnections(sessionId.ToString());
        if (groupConnections != null)
        {
            await _hubService.RemoveConnectionsFromGroup(groupConnections, sessionId.ToString());
        }

        await _connectionMappingService.DeleteGroup(sessionId.ToString());
    }
}