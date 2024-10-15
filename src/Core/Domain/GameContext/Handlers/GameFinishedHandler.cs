using Image_guesser.Core.Domain.GameContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class GameFinishedHandler(IGameService gameService) : INotificationHandler<GameFinished>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));

    public async Task Handle(GameFinished notification, CancellationToken cancellationToken)
    {
        BaseGame game = await _gameService.GetBaseGameById(notification.GameId);

        game.GameOver();

        await _gameService.UpdateGameOrGuesser(game);
    }
}