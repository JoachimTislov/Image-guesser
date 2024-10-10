using Image_guesser.Core.Domain.GameContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class PlayerGuessedCorrectlyHandler(IGameService gameService) : INotificationHandler<PlayerGuessedCorrectly>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    public async Task Handle(PlayerGuessedCorrectly notification, CancellationToken cancellationToken)
    {
        BaseGame game = await _gameService.GetBaseGameById(notification.GameId);
        Guesser guesser = await _gameService.GetGuesserById(notification.GuesserId);

        guesser.TimeSpan = DateTime.Now - game.TimeOfCreation;
        guesser.Points = notification.Points;

        await _gameService.UpdateGuesser(guesser);
    }
}
