using MediatR;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class CreateGameHandler(IGameService gameService) : INotificationHandler<CreateGame>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        await _gameService.CreateGame(notification.Session);
    }
}