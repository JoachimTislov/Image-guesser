using Image_guesser.Core.Domain.GameContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class PlayerGuessedCorrectlyHandler(IGameService gameService, IMediator mediator) : INotificationHandler<PlayerGuessedCorrectly>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    public async Task Handle(PlayerGuessedCorrectly notification, CancellationToken cancellationToken)
    {
        Guesser guesser = await _gameService.GetGuesserById(notification.GuesserId);

        guesser.Points = notification.Points;

        await _gameService.UpdateGameOrGuesser(guesser);

        await _mediator.Publish(new GameFinished(notification.GameId, notification.SessionId), cancellationToken);
    }
}
