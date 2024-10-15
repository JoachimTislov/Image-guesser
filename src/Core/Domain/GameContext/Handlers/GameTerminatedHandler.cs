using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class GameTerminatedHandler(IGameService gameService, IMediator mediator) : INotificationHandler<GameTerminated>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(GameTerminated notification, CancellationToken cancellationToken)
    {
        BaseGame game = await _gameService.GetBaseGameById(notification.GameId);

        game.Terminated();

        await _gameService.UpdateGameOrGuesser(game);

        await _mediator.Publish(new ReturnToLobby(notification.SessionId), cancellationToken);
    }
}