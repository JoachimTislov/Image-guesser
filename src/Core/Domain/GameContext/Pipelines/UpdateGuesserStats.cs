using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Pipelines;

public class UpdateGuesserStats
{
    public record Request(Guid GameId, Guesser Guesser, TimeSpan Speed) : IRequest<Unit>;

    public class Handler(IMediator mediator, ImageGameContext db) : IRequestHandler<Request, Unit>
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var guesser = await _mediator.Send(new GetGuesserById.Request(request.Guesser.Id), cancellationToken);
            var game = await _mediator.Send(new GetGameById.Request(request.GameId), cancellationToken);

            if (game.IsGameOver())
            {
                guesser.Points = request.Guesser.Points;
                guesser.TimeSpan = request.Speed;

                //Last guess is not counted, only wrong guesses
                guesser.Guesses += 1;
            }
            else // Game is still running
            {    // This is to update the guessers ongoing stats
                guesser.Guesses = request.Guesser.Guesses;
                guesser.WrongGuessCounter = request.Guesser.WrongGuessCounter;
            }

            _db.Games.Update(game);
            await _db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}