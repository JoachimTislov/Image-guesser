using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.SessionContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class GameTerminatedHandler(ISessionService sessionService) : INotificationHandler<GameTerminated>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    public async Task Handle(GameTerminated notification, CancellationToken cancellationToken)
    {
        await _sessionService.BackToLobbyEvent(notification.SessionId);
    }
}