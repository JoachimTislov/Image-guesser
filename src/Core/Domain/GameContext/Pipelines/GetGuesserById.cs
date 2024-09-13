using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.GameContext.Pipelines;

public class GetGuesserById
{
    public record Request(Guid GuesserId) : IRequest<Guesser>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, Guesser?>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));
        public async Task<Guesser?> Handle(Request request, CancellationToken cancellationToken)
        {
            var guesser = await _db.Guessers.Where(s => s.Id == request.GuesserId)
                                            .SingleOrDefaultAsync(cancellationToken);
            return guesser;
        }
    }
}