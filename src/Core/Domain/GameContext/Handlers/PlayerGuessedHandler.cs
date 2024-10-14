using Image_guesser.Core.Domain.GameContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class PlayerGuessedHandler(IGameService gameService) : INotificationHandler<PlayerGuessed>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));

    public async Task Handle(PlayerGuessed notification, CancellationToken cancellationToken)
    {
        await _gameService.CreateAndAddGuess(notification.Guess, notification.Username, notification.TimeOfGuess, notification.GameId);

        var guesser = await _gameService.GetGuesserById(notification.GuesserId);

        guesser.IncrementGuesses();

        await _gameService.UpdateGuesser(guesser);
    }
}
