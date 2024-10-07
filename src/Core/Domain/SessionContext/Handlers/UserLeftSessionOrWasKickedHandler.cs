using MediatR;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SessionContext.Events;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class UserLeftSessionOrWasKickedHandler(ISessionService sessionService) : INotificationHandler<UserLeftSessionOrWasKicked>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    public async Task Handle(UserLeftSessionOrWasKicked notification, CancellationToken cancellationToken)
    {
        var sessionId = notification.SessionId;
        var userId = notification.UserId;

        await _sessionService.UpdateChosenOracleIfUserWasOracle(sessionId, Guid.Parse(userId));
        await _sessionService.RemoveUserFromSession(userId, sessionId.ToString());
    }
}