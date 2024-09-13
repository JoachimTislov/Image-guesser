using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext.Pipelines;

public class GetAllSessions
{
    public record Request() : IRequest<List<Session>>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, List<Session>>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<List<Session>> Handle(Request request, CancellationToken cancellationToken)
        {
            var sessions = await _db.Sessions.OrderByDescending(s => s.CreationTime).ToListAsync(cancellationToken: cancellationToken);
            return sessions;
        }
    }
}