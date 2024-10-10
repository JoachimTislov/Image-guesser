using Image_guesser.Core.Domain.GameContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class PlayerGuessedIncorrectlyHandler(IGameService gameService) : INotificationHandler<PlayerGuessedIncorrectly>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));

    public async Task Handle(PlayerGuessedIncorrectly notification, CancellationToken cancellationToken)
    {
        var guesser = await _gameService.GetGuesserById(notification.GuesserId);

        if (guesser.ReachedMaxWrongGuesses())
        {
            // Oracles Turn, how should it be handled in multiplayer game ? 
            // await _mediator.Publish(new OraclesTurn(notification.GameId), cancellationToken);

            guesser.ResetWrongGuesses();
        }
        else
        {
            guesser.IncrementWrongGuessCounter();
        }

        guesser.IncrementGuesses();

        await _gameService.UpdateGuesser(guesser);
    }
}
