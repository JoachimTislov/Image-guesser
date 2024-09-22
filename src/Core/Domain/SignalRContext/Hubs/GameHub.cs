using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Image_guesser.Core.Domain.SignalRContext.Services;
using Image_guesser.Core.Domain.SessionContext.Pipelines;
using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.OracleContext.Events;

namespace Image_guesser.Core.Domain.SignalRContext.Hubs;

public class GameHub(IConnectionMappingService connectionMappingService, UserManager<User> userManager, IMediator mediator, OracleService oracleService) : Hub<IGameClient>
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
            _connectionMappingService.AddConnection(userId, Context.ConnectionId);

            var groupId = _connectionMappingService.GetGroups(userId);
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
            await _connectionMappingService.RemoveConnection(userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task CreateGroup(string SessionId)
    {
        var UserIdentifier = Context.UserIdentifier;
        if (UserIdentifier != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, SessionId);

            var user = await _userManager.FindByIdAsync(UserIdentifier);
            if (user != null)
                await _connectionMappingService.AddToGroup(user.Id, Guid.Parse(SessionId));
        }
    }

    public async Task StartNextRound(string SessionId)
    {
        var session = await _mediator.Send(new GetSessionById.Request(Guid.Parse(SessionId)));
        if (session != null)
        {
            await _mediator.Publish(new CreateGame(session));
        }
    }

    public async Task JoinGroup(string SessionId)
    {
        var UserIdentifier = Context.UserIdentifier;
        if (UserIdentifier != null)
        {
            var sessionGuid = Guid.Parse(SessionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, SessionId);

            var user = await _userManager.FindByIdAsync(UserIdentifier);
            if (user != null)
            {
                await _connectionMappingService.AddToGroup(user.Id, sessionGuid);
                await _mediator.Send(new AddPlayerToSession.Request(sessionGuid, user));
            }

            // We use RedirectToLink because this allows us to bypass the need for extensive front-end logic
            // that is already handled by the back-end rendering of the webpage.
            await Clients.Groups(SessionId).RedirectToLink($"/Lobby/{SessionId}");
        }
    }

    public async Task LeaveGroup(string userId, string SessionId)
    {
        var userConnection = _connectionMappingService.GetConnections(userId);
        var user = await _userManager.FindByIdAsync(userId);
        var sessionGuid = Guid.Parse(SessionId);

        var session = await _mediator.Send(new GetSessionById.Request(sessionGuid));

        if (session != null && user != null && session.ChosenOracleId == user.Id)
        {
            session.ChosenOracleId = session.SessionHostId;
        }

        if (user != null && userConnection != null)
        {
            await Groups.RemoveFromGroupAsync(userConnection, SessionId.ToString());

            await _connectionMappingService.RemoveFromGroup(user.Id, sessionGuid);

            await _mediator.Send(new RemoveUserFromSession.Request(sessionGuid, user));

            // Allows us to bypass the need for extensive front-end logic that is already handled by the back-end
            // await Clients.Groups(SessionId).SendAsync("RedirectToLink", $"/Lobby/{SessionId}");
            await Clients.Client(userConnection).RedirectToLink("/");
            await Clients.Groups(SessionId).ReloadPage();
        }
    }

    public async Task RemovePlayersFromGroup(string SessionId)
    {
        var session = await _mediator.Send(new GetSessionById.Request(Guid.Parse(SessionId)));

        session.Events.Add(new SessionClosed(session));

        var players = new List<User>();
        if (session != null) players = session.SessionUsers.ToList();

        foreach (var player in players)
        {
            var userConnection = _connectionMappingService.GetConnections(player.Id.ToString());

            await LeaveGroup(player.Id.ToString(), SessionId);

            if (userConnection != null) await Clients.Client(userConnection).RedirectToLink("/");
        }

        await _mediator.Send(new DeleteSession.Request(Guid.Parse(SessionId)));
    }

    // Temp function to handle chat messages in the lobby, (not a feature, only for testing purposes)
    public async Task SendMessage(string message, string SessionId)
    {
        await Clients.Group(SessionId).ReceiveMessage(message);
    }

    public async Task SendGuess(
            string guess, string userId, string sessionId,
            string gameId, string guesserId, string imageIdentifier)
    {
        var user = await _userManager.FindByIdAsync(userId);

        // This makes it so that everyone can see the guesses made
        if (user != null && user.UserName != null)
            await Clients.Group(sessionId).ReceiveGuess(guess, user.UserName);

        //This sets of an event that Oracle will handle 
        await _mediator.Publish(new PlayerGuessed(Guid.Parse(sessionId), guess, guesserId, Guid.Parse(gameId)));

        var session = await _mediator.Send(new GetSessionById.Request(Guid.Parse(sessionId)));

        if (user != null && user.UserName != null && session != null)
        {
            var Response = await _oracleService.CheckGuess(guess, imageIdentifier, user.UserName, session.ChosenOracleId, session.Options.GameMode);
            if (Response.IsGuessCorrect) await Clients.Group(sessionId).CorrectGuess(Response.WinnerText, guess);
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

    // Used in multiplayer AI games, where the leader reveals the next piece for everyone
    public async Task ShowNextPieceForAllPlayers(string sessionId)
    {
        await Clients.Group(sessionId).ShowNextPieceForAll();
    }
}