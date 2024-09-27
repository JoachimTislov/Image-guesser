using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Pipelines;

public class AddPlayerToSession
{
    public record Request(Guid SessionId, User User) : IRequest;

    public class Handler(ImageGameContext db, ISessionService sessionService, IMediator mediator) : IRequestHandler<Request>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var session = await _mediator.Send(new GetSessionById.Request(request.SessionId), cancellationToken);

            if (session != null)
            {
                await _sessionService.JoinSession(request.User, session);
            }

            return Unit.Value;
        }
    }
}