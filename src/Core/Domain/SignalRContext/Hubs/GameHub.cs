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

    public async Task AddToGroup(string sessionId)
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await AddToGroups(sessionId, userId, Context.ConnectionId);
        }
    }

    public async Task JoinGroup(string sessionId)
    {
        await AddToGroup(sessionId);

        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await _sessionService.AddUserToSession(userId, sessionId);
        }
        // We use RedirectToLink because this allows us to bypass the need for extensive front-end logic
        // that is already handled by the back-end rendering of the webpage.
        await Clients.Groups(sessionId).RedirectToLink($"/Lobby/{sessionId}");

    }

    public async Task SendGuess(
            string guess, string userId, string sessionId, string oracleId,
            string gameId, string guesserId, string imageIdentifier)
    {
        string userName = await _userService.GetUserNameByUserId(userId);

        await Clients.Group(sessionId).ReceiveGuess(guess, userName);

        //This sets of an event that Oracle will handle 
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