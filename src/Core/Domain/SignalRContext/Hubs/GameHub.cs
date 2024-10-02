using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.OracleContext.Events;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;

namespace Image_guesser.Core.Domain.SignalRContext.Hubs;

public class GameHub(IConnectionMappingService connectionMappingService, IUserService userService, ISessionService sessionService, IMediator mediator, IOracleService oracleService) : Hub<IGameClient>
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));

    // To keep SignalR connection consistency across the application we had to implement a ConnectionMappingService.
    // This just keeps track of which users are associated with which Group so that we can easily reconnect them.
    private readonly IConnectionMappingService _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await _connectionMappingService.AddConnection(userId, Context.ConnectionId);

            var groupId = _connectionMappingService.GetGroupId(userId);
            if (groupId != string.Empty)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
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

    private async Task AddToGroups(string sessionId, string userId, string connectionId)
    {
        await Groups.AddToGroupAsync(connectionId, sessionId);
        await _connectionMappingService.AddToGroup(userId, sessionId);
    }

    public async Task CreateGroup(string sessionId)
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await AddToGroups(sessionId, userId, Context.ConnectionId);
        }
    }

    private static Guid GuidParseString(string value)
    {
        return Guid.Parse(value);
    }

    public async Task CreateANewGame(string SessionId)
    {
        var session = await _sessionService.GetSessionById(GuidParseString(SessionId));

        await _mediator.Publish(new CreateGame(session));
    }

    public async Task JoinGroup(string sessionId)
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await AddToGroups(sessionId, userId, Context.ConnectionId);
            await _sessionService.AddUserToSession(userId, sessionId);

            // We use RedirectToLink because this allows us to bypass the need for extensive front-end logic
            // that is already handled by the back-end rendering of the webpage.
            await Clients.Groups(sessionId).RedirectToLink($"/Lobby/{sessionId}");
        }
    }

    public async Task LeaveGroup(string userId, string SessionId)
    {
        string userConnection = _connectionMappingService.GetConnection(userId);

        await _sessionService.UpdateChosenOracleIfUserWasOracle(Guid.Parse(SessionId), Guid.Parse(userId));

        await Groups.RemoveFromGroupAsync(userConnection, SessionId);

        await _connectionMappingService.RemoveFromGroup(userId, SessionId);

        User user = await _userService.GetUserById(userId);
        await _sessionService.RemoveUserFromSession(user, SessionId);

        // Allows us to bypass the need for extensive front-end logic that is already handled by the back-end
        // await Clients.Groups(SessionId).SendAsync("RedirectToLink", $"/Lobby/{SessionId}");
        await Clients.Client(userConnection).RedirectToLink("/");
        await Clients.Groups(SessionId).ReloadPage();
    }

    public async Task CloseGroup(string SessionId)
    {
        var session = await _sessionService.GetSessionById(Guid.Parse(SessionId));

        foreach (var user in session.SessionUsers)
        {
            var userConnection = _connectionMappingService.GetConnection(user.Id.ToString());

            await LeaveGroup(user.Id.ToString(), SessionId);

            if (userConnection != null)
            {
                await Clients.Client(userConnection).RedirectToLink("/");
            }
        }

        await _mediator.Publish(new SessionClosed(Guid.Parse(SessionId)));
    }

    public async Task SendGuess(
            string guess, string userId, string sessionId,
            string gameId, string guesserId, string imageIdentifier)
    {
        string userName = await _userService.GetUserNameByUserId(userId);

        await Clients.Group(sessionId).ReceiveGuess(guess, userName);

        //This sets of an event that Oracle will handle 
        await _mediator.Publish(new PlayerGuessed(Guid.Parse(sessionId), guess, guesserId, Guid.Parse(gameId)));

        var session = await _sessionService.GetSessionById(Guid.Parse(sessionId));
        var (IsGuessCorrect, WinnerText) = await _oracleService.CheckGuess(guess, imageIdentifier, userName, session.ChosenOracleId, session.Options.GameMode);
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

    public async Task ShowNextPieceForAllPlayers(string sessionId)
    {
        await Clients.Group(sessionId).ShowNextPieceForAll();
    }
}