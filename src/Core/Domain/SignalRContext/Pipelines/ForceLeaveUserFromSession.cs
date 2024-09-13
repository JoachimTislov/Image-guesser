using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Pipelines;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.SignalRContext.Services;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SignalRContext.Pipelines;

public class ForceLeaveUserFromSession
{
    public record Request(User User, Session Session, bool ClosedSession) : IRequest;

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
            var userConnection = _connectionMappingService.GetConnections(request.User.Id.ToString());

            if (userConnection != null) await _hubContext.Groups.RemoveFromGroupAsync(userConnection, request.Session.Id.ToString(), cancellationToken);

            if (request.Session.ChosenOracle == request.User.Id)
            {
                request.Session.ChosenOracle = request.Session.SessionHostId;
            }

            if (request.Session.SessionUsers.Count == 1 && request.Session.SessionHostId == request.User.Id)
            {
                Console.WriteLine($"Force leave user: {request.User.UserName} from the session before closing it");
                await _mediator.Send(new DeleteSession.Request(request.Session.Id), cancellationToken);
            }
            else
            {
                Console.WriteLine($"Remove user: {request.User.UserName} from the session");
                await _mediator.Send(new RemoveUserFromSession.Request(request.Session.Id, request.User), cancellationToken);
            }

            // Allows us to bypass the need for extensive front-end logic that is already handled by the back-end
            // await Clients.Groups(SessionId).RedirectToLink($"/Lobby/{SessionId}");
            if (!request.ClosedSession)
            {
                if (userConnection != null) await _hubContext.Clients.Client(userConnection).RedirectToLink("/");
                await _hubContext.Clients.Groups(request.Session.Id.ToString()).ReloadPage();
            }

            return Unit.Value;
        }
    }
}
