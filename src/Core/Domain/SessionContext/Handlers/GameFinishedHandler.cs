using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class GameFinishedHandler(ISessionService sessionService) : INotificationHandler<GameFinished>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    public async Task Handle(GameFinished notification, CancellationToken cancellationToken)
    {
        Session session = await _sessionService.GetSessionById(notification.SessionId);

        session.Options.IncrementAmountOfGamesPlayed();

        await _sessionService.UpdateSession(session);
    }
}