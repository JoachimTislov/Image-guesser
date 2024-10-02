using Image_guesser.Core.Domain.GameContext.Events;
using MediatR;
using Image_guesser.Core.Domain.SessionContext.Services;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class CreateGameHandler(ISessionService sessionService) : INotificationHandler<CreateGame>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        notification.Session.InGame();
        notification.Session.Options.DecrementNumberOfRounds();

        await _sessionService.UpdateSession(notification.Session);
    }
}