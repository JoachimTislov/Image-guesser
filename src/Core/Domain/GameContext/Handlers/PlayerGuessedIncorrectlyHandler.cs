using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Infrastructure.GenericRepository;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class PlayerGuessedIncorrectlyHandler(IGameService gameService, IRepository repository) : INotificationHandler<PlayerGuessedIncorrectly>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

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

        await _repository.Update(guesser);
    }
}
