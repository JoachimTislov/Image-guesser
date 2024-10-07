using MediatR;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class CreateGameHandler(IGameService gameService, ISessionService sessionService) : INotificationHandler<CreateGame>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetSessionById(notification.SessionId);
        await _gameService.CreateGame(session);
    }
}