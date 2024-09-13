using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Pipelines;
using MediatR;

namespace Core.Domain.GameManagementContext.Handlers;

public class PlayerGuessedCorrectlyHandler(IMediator mediator) : INotificationHandler<PlayerGuessedCorrectly>
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(PlayerGuessedCorrectly notification, CancellationToken cancellationToken)
    {
        var game = await _mediator.Send(new GetGameById.Request(notification.GameId), cancellationToken);

        game.Events.Add(new GameFinished(notification.GameId));

        var speed = DateTime.Now - game.Timer;
        var guesser = await _mediator.Send(new GetGuesserById.Request(notification.GuesserId), cancellationToken);

        await _mediator.Send(new UpdateGuesserStats.Request(game.Id, guesser, speed), cancellationToken);
    }
}

