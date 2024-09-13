using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.SessionContext.Pipelines;

public class DeleteSession
{
    public record Request(Guid SessionId) : IRequest;

    public class Handler(ImageGameContext db) : IRequestHandler<Request>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var users = await _db.Users.Where(u => u.SessionId == request.SessionId).ToListAsync(cancellationToken: cancellationToken);
            var session = await _db.Sessions.SingleOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken) ?? throw new EntityNotFoundException("Session with ID " + request.SessionId + " could not be found");

            // Done to make sure that all the foreign keys are removed before deleting the session
            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    user.SessionId = null;
                }
                await _db.SaveChangesAsync(cancellationToken);
            }

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}