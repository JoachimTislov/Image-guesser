using Image_guesser.Core.Domain.SessionContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class SessionClosedHandler(ISessionService sessionService) : INotificationHandler<SessionClosed>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    public async Task Handle(SessionClosed notification, CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetSessionById(notification.SessionId);

        session.SessionUsers.Clear();
        session.CloseLobby();

        await _sessionService.UpdateSession(session);
    }
}