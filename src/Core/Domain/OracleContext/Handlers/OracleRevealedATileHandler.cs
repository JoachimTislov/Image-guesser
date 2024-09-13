using Image_guesser.Core.Domain.OracleContext.Events;
using Image_guesser.Core.Domain.OracleContext.Pipelines;
using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class OracleRevealedATileHandler(IMediator mediator, ImageGameContext db) : INotificationHandler<OracleRevealedATile>
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

    public async Task Handle(OracleRevealedATile notification, CancellationToken cancellationToken)
    {
        var oracle = await _mediator.Send(new GetBaseOracleById.Request(notification.OracleId), cancellationToken);
        oracle.NumberOfTilesRevealed++;

        _db.Oracles.Update(oracle);
        await _db.SaveChangesAsync(cancellationToken);
    }
}