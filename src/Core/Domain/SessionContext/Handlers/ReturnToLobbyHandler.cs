using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.SessionContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class ReturnToLobbyHandler(ISessionService sessionService) : INotificationHandler<ReturnToLobby>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    public async Task Handle(ReturnToLobby notification, CancellationToken cancellationToken)
    {
        await _sessionService.BackToLobbyEvent(notification.SessionId);
    }
}