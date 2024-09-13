using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.GameContext.Pipelines;

public class AddGame
{
    public record Request(Game Game) : IRequest<Response>;

    public record Response(bool Success, Game CreatedGame);

    public class Handler(ImageGameContext db) : IRequestHandler<Request, Response>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            _db.Games.Add(request.Game);
            await _db.SaveChangesAsync(cancellationToken);

            return new Response(true, request.Game);
        }
    }
}