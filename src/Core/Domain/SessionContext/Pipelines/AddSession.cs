using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Pipelines;

public class AddSession
{
    public record Request(Session Session) : IRequest<Session>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, Session>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Session> Handle(Request request, CancellationToken cancellationToken)
        {
            _db.Sessions.Add(request.Session);
            await _db.SaveChangesAsync(cancellationToken);

            return request.Session;
        }
    }
}