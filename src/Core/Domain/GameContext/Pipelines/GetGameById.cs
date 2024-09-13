using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.GameContext.Pipelines;

public class GetGameById
{
    public record Request(Guid GameId) : IRequest<Game>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, Game>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Game> Handle(Request request, CancellationToken cancellationToken)
        {
            var game = await _db.Games
                .Where(s => s.Id == request.GameId)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("Game not found");

            return game;
        }
    }
}