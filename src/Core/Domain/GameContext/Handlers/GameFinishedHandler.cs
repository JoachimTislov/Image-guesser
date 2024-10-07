using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Infrastructure.GenericRepository;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class GameFinishedHandler(IGameService gameService, IRepository repository) : INotificationHandler<GameFinished>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task Handle(GameFinished notification, CancellationToken cancellationToken)
    {
        BaseGame game = await _gameService.GetBaseGameById(notification.GameId);

        game.GameOver();

        await _repository.Update(game);
    }
}