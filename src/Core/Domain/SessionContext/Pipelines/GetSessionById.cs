using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.SessionContext.Pipelines;

public class GetSessionById
{
    public record Request(Guid SessionId) : IRequest<Session>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, Session?>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Session?> Handle(Request request, CancellationToken cancellationToken)
        {
            var session = await _db.Sessions
                .Where(s => s.Id == request.SessionId)
                .Include(s => s.SessionUsers)
                .SingleOrDefaultAsync(cancellationToken);

            return session;
        }
    }
}