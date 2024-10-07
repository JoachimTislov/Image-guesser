using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Infrastructure.GenericRepository;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class GameTerminatedHandler(IGameService gameService, IRepository repository) : INotificationHandler<GameTerminated>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task Handle(GameTerminated notification, CancellationToken cancellationToken)
    {
        BaseGame game = await _gameService.GetBaseGameById(notification.GameId);

        game.Terminated();

        await _repository.Update(game);
    }
}