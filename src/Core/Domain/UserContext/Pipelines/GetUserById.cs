using MediatR;
using Image_guesser.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.UserContext.Pipelines;

public class GetUserById
{
    public record Request(Guid UserId) : IRequest<User>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, User?>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<User?> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.Where(u => u.Id == request.UserId).SingleOrDefaultAsync(cancellationToken);

            return user;
        }
    }

}
