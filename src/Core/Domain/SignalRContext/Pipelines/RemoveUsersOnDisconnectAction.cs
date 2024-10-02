using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SignalRContext.Pipelines;

public class RemoveUsersOnDisconnectAction
{
    public record Request(Guid SessionId, User User) : IRequest;

    public class Handler(
        IMediator mediator,
        ISessionService sessionService,
        IConnectionMappingService connectionMappingService,
        IHubContext<GameHub, IGameClient> hubContext) : IRequestHandler<Request>
    {
        private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));
        private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            string groupId = _connectionMappingService.GetGroupId(request.User.Id.ToString());

            if (groupId == string.Empty)
            {
                return Unit.Value;
            }
            else
            {
                var session = await _sessionService.GetSessionById(Guid.Parse(groupId));

                await _connectionMappingService.RemoveFromGroup(request.User.Id.ToString(), groupId);

                if (session.SessionHostId == request.User.Id)
                {
                    // This makes sure that the session is closed and deleted if the host leaves the session via the home button
                    var users = session.SessionUsers.ToList();
                    foreach (var user in users)
                    {
                        Console.WriteLine($"Disconnected user: {user.UserName} from the session before closing it");
                        await _mediator.Send(new ForceLeaveUserFromSession.Request(user, session, true), cancellationToken);

                        var userConnection = _connectionMappingService.GetConnection(user.Id.ToString());
                        if (userConnection != null)
                        {
                            await _hubContext.Clients.Client(userConnection).RedirectToLink("/");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Force leave user: {request.User.UserName} from the session before closing it");
                    await _mediator.Send(new ForceLeaveUserFromSession.Request(request.User, session, false), cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}