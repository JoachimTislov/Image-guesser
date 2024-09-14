using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Pipelines;
using Image_guesser.Core.Domain.ImageContext.Pipelines;
using Image_guesser.Core.Domain.OracleContext.Pipelines;
using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class PlayerGuessedHandler(IMediator mediator, ImageGameContext db) : INotificationHandler<PlayerGuessed>
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

    public async Task Handle(PlayerGuessed notification, CancellationToken cancellationToken)
    {
        var game = await _mediator.Send(new GetGameById.Request(Guid.Parse(notification.GameId)), cancellationToken);
        var Oracle = await _mediator.Send(new GetBaseOracleById.Request(game.OracleId), cancellationToken);

        Oracle.TotalGuesses++;
        _db.Oracles.Update(Oracle);

        var Image = await _mediator.Send(new GetImageDataByIdentifier.Request(Oracle.ImageIdentifier), cancellationToken);
        if (Image.Name == notification.Guess)
        {
            //Calculating amount of unrevealed tiles
            var points = Image.PieceCount - Oracle.NumberOfTilesRevealed;

            game.Events.Add(new PlayerGuessedCorrectly(points, game.Id, Guid.Parse(notification.GuesserId), game.GameMode));
        }
        else
        {
            game.Events.Add(new PlayerGuessedIncorrectly(Guid.Parse(notification.GuesserId), game.Id));
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}