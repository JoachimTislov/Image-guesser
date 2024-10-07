using MediatR;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SessionContext.Events;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class UserClickedJoinSessionHandler(ISessionService sessionService) : INotificationHandler<UserClickedJoinSession>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    public async Task Handle(UserClickedJoinSession notification, CancellationToken cancellationToken)
    {
        await _sessionService.AddUserToSession(notification.UserId, notification.SessionId);
    }
}

