using Microsoft.AspNetCore.SignalR;
using MediatR;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.OracleContext;

namespace Image_guesser.Core.Domain.SignalRContext.Hubs;

public class GameHub(IConnectionMappingService connectionMappingService, IUserService userService, ISessionService sessionService, IMediator mediator, IOracleService oracleService, IHubService hubService) : Hub<IGameClient>, IGameServer
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    // To keep SignalR connection consistency across the application we had to implement a ConnectionMappingService.
    // This just keeps track of which users are associated with which Group so that we can easily reconnect them.
    private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));


    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await _connectionMappingService.AddConnection(userId, Context.ConnectionId);

            // Gets persisted sessionId from the database
            var sessionId = await _userService.GetSessionIdByUserId(Guid.Parse(userId));
            if (sessionId != null)
            {
                await AddToGroup(sessionId.Value.ToString(), userId);
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await _connectionMappingService.RemoveConnection(userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task AddToGroup(string sessionId, string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }

    public async Task JoinSession(string sessionId, string userId)
    {
        await AddToGroup(sessionId, userId);

        await _sessionService.AddUserToSession(userId, sessionId);

        await _hubService.RedirectGroupToPage(sessionId, $"/Lobby/{sessionId}");
    }

    public async Task LeaveSession(string userId, string sessionId)
    {
        await RemoveFromGroupAsync(sessionId, Context.ConnectionId);

        await _sessionService.RemoveUserFromSession(userId, Guid.Parse(sessionId));

        // await _hubService.ReloadGroupPage(sessionId);
        await _hubService.RedirectClientToPage(userId, "/");

        await _hubService.ReloadGroupPage(sessionId);
    }

    private async Task RemoveFromGroupAsync(string sessionId, string connectionId)
    {
        await Groups.RemoveFromGroupAsync(connectionId, sessionId);
    }

    public async Task CloseSession(string sessionId)
    {
        await _mediator.Publish(new SessionClosed(Guid.Parse(sessionId)));

        await _hubService.RedirectGroupToPage(sessionId, "/");

        var sessionUsers = await _sessionService.GetUsersInSessionById(Guid.Parse(sessionId));
        foreach (var user in sessionUsers)
        {
            var connectionId = _connectionMappingService.GetConnection(user.Id.ToString());
            await RemoveFromGroupAsync(sessionId, connectionId);
        }
    }

    public async Task SendGuess(
            string guess, string userId, string sessionId, string oracleId,
            string gameId, string guesserId, string imageIdentifier)
    {
        string userName = await _userService.GetUserNameByUserId(userId);

        await Clients.Group(sessionId).ReceiveGuess(guess, userName);

        await _mediator.Publish(new PlayerGuessed(Guid.Parse(oracleId), guess, Guid.Parse(guesserId), Guid.Parse(gameId), Guid.Parse(sessionId)));

        var session = await _sessionService.GetSessionById(Guid.Parse(sessionId));
        var (IsGuessCorrect, WinnerText) = await _oracleService.HandleGuess(guess, imageIdentifier, userName, session.ChosenOracleId, session.Options.GameMode);
        if (IsGuessCorrect)
        {
            await Clients.Group(sessionId).CorrectGuess(WinnerText, guess);
        }
    }

    public async Task OracleRevealedATile(string oracleId)
    {
        await _mediator.Publish(new OracleRevealedATile(Guid.Parse(oracleId)));
    }

    public async Task ShowThisPiece(string pieceId, string sessionId)
    {
        await Clients.Group(sessionId).ShowPiece(pieceId);
    }

    public async Task ShowNextPieceForAll(string sessionId)
    {
        await Clients.Group(sessionId).ShowNextPieceForAll();
    }
}