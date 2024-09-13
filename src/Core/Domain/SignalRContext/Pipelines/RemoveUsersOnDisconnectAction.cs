using Image_guesser.Core.Domain.SessionContext.Pipelines;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.SignalRContext.Services;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SignalRContext.Pipelines;

public class RemoveUsersOnDisconnectAction
{
    public record Request(Guid SessionId, User User) : IRequest;

    public class Handler(
        IMediator mediator,
        IConnectionMappingService connectionMappingService,
        IHubContext<GameHub, IGameClient> hubContext) : IRequestHandler<Request>
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));
        private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var sessionId = _connectionMappingService.GetGroups(request.User.Id.ToString());

            if (sessionId != string.Empty)
            {
                var session = await _mediator.Send(new GetSessionById.Request(Guid.Parse(sessionId)), cancellationToken);

                if (request.User != null && session != null)
                {
                    await _connectionMappingService.RemoveFromGroup(request.User.Id, Guid.Parse(sessionId));

                    if (session.SessionHostId == request.User.Id)
                    {
                        // This makes sure that the session is closed and deleted if the host leaves the session via the home button
                        var users = session.SessionUsers.ToList();
                        foreach (var user in users)
                        {
                            Console.WriteLine($"Disconnected user: {user.UserName} from the session before closing it");
                            await _mediator.Send(new ForceLeaveUserFromSession.Request(user, session, true), cancellationToken);

                            var userConnection = _connectionMappingService.GetConnections(user.Id.ToString());
                            if (userConnection != null) await _hubContext.Clients.Client(userConnection).RedirectToLink("/");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Force leave user: {request.User.UserName} from the session before closing it");
                        await _mediator.Send(new ForceLeaveUserFromSession.Request(request.User, session, false), cancellationToken);
                    }
                }
            }
            return Unit.Value;
        }
    }
}