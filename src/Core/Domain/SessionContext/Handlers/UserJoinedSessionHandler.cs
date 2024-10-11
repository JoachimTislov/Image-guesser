using Image_guesser.Core.Domain.SessionContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class UserJoinedSessionHandler(ISessionService sessionService) : INotificationHandler<UserJoinedSession>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    public async Task Handle(UserJoinedSession notification, CancellationToken cancellationToken)
    {
        var (sessionId, userId) = notification;

        await _sessionService.AddUserToSession(userId, sessionId);
    }
}
