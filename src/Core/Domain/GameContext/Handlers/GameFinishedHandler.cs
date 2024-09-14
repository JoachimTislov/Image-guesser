using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Pipelines;
using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class GameFinishedHandler(IMediator mediator, ImageGameContext db) : INotificationHandler<GameFinished>
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

    public async Task Handle(GameFinished notification, CancellationToken cancellationToken)
    {
        var game = await _mediator.Send(new GetGameById.Request(notification.GameId), cancellationToken);

        game.GameOver();

        _db.Games.Update(game);
        await _db.SaveChangesAsync(cancellationToken);
    }
}