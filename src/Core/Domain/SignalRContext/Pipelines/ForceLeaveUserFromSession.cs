using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SignalRContext.Pipelines;

public class ForceLeaveUserFromSession
{
    public record Request(User User, Session Session, bool ClosedSession) : IRequest;

    public class Handler(
            ISessionService sessionService,
            IConnectionMappingService connectionMappingService,
            IHubContext<GameHub, IGameClient> hubContext) : IRequestHandler<Request>
    {
        private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));
        private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var userConnection = _connectionMappingService.GetConnection(request.User.Id.ToString());

            if (userConnection != null)
            {
                await _hubContext.Groups.RemoveFromGroupAsync(userConnection, request.Session.Id.ToString(), cancellationToken);
            }

            if (request.Session.ChosenOracleId == request.User.Id)
            {
                request.Session.ChosenOracleId = request.Session.SessionHostId;
            }

            if (request.Session.SessionUsers.Count == 1 && request.Session.SessionHostId == request.User.Id)
            {
                Console.WriteLine($"Force leave user: {request.User.UserName} from the session before deleting it");
                await _sessionService.DeleteSessionById(request.Session.Id);
            }
            else
            {
                Console.WriteLine($"Remove user: {request.User.UserName} from the session");
                await _sessionService.RemoveUserFromSession(request.User.Id.ToString(), request.Session.Id.ToString());
            }

            // Allows us to bypass the need for extensive front-end logic that is already handled by the back-end
            // await Clients.Groups(SessionId).RedirectToLink($"/Lobby/{SessionId}");
            if (!request.ClosedSession)
            {
                if (userConnection != null)
                {
                    await _hubContext.Clients.Client(userConnection).RedirectToLink("/");
                }

                await _hubContext.Clients.Groups(request.Session.Id.ToString()).ReloadPage();
            }

            return Unit.Value;
        }
    }
}
