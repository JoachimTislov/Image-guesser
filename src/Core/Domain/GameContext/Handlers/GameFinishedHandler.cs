using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Infrastructure;
using Image_guesser.Infrastructure.GenericRepository;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class GameFinishedHandler(IGameService gameService, IRepository repository) : INotificationHandler<GameFinished>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task Handle(GameFinished notification, CancellationToken cancellationToken)
    {
        Guesser guesser = await _gameService.GetGuesserById(notification.GuesserId);
        TimeSpan speed = DateTime.Now - notification.Game.TimeOfCreation;

        guesser.TimeSpan = speed;
        guesser.Points = notification.Points;
        await _repository.Update(guesser);

        notification.Game.GameOver();

        await _repository.Update(notification.Game);
    }
}