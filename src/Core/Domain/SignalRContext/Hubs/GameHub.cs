using Microsoft.AspNetCore.SignalR;
using MediatR;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.SignalRContext.Services.ConnectionMapping;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.SignalRContext.Hubs;

public class GameHub(IConnectionMappingService connectionMappingService, IUserService userService, IMediator mediator, IHubService hubService) : Hub<IGameClient>, IGameServer
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

            // Gets persisted sessionId from the database, and only session hosts persists a sessionId.
            var sessionId = await _userService.GetSessionIdByUserId(Guid.Parse(userId));
            if (sessionId != null)
            {
                await AddToGroup(sessionId.Value.ToString(), userId);
            }

            Console.WriteLine($"User connected, userId: {userId}, connection: {Context.ConnectionId}, sessionId: {sessionId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"User disconnected, userId: {Context.UserIdentifier}, connection: {Context.ConnectionId}");

        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await _connectionMappingService.RemoveConnection(userId, Context.ConnectionId);

            var sessionId = await _userService.GetSessionIdByUserId(Guid.Parse(userId));
            if (sessionId != null)
            {
                await _hubService.RemoveFromGroupAsync(sessionId.Value.ToString(), userId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task ClientNavigatedToPage(string pathName)
    {
        var userId = Context.UserIdentifier;
        if (userId == null) return;

        await _mediator.Publish(new UserNavigatedToPage(pathName, userId));
    }

    public async Task AddToGroup(string sessionId, string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await _connectionMappingService.AddToGroup(sessionId, Context.ConnectionId);
    }

    public async Task JoinSession(string sessionId, string userId)
    {
        await AddToGroup(sessionId, userId);

        await _mediator.Publish(new UserJoinedSession(sessionId, userId));

        await _hubService.RedirectGroupToPage(sessionId, $"/Lobby/{sessionId}");
    }

    public async Task LeaveSession(string userId, string sessionId)
    {
        await _mediator.Publish(new UserLeftSession(Guid.Parse(sessionId), userId));
    }

    public async Task CloseSession(string sessionId)
    {
        await _hubService.RedirectGroupToPage(sessionId, "/");
        await _mediator.Publish(new SessionClosed(Guid.Parse(sessionId)));
    }

    public async Task SendGuess(
            string guess, string userId, string sessionId, string oracleId,
            string gameId, string guesserId, string imageIdentifier, string timeOfGuess)
    {
        string userName = await _userService.GetUserNameByUserId(userId);

        await Clients.Group(sessionId).ReceiveGuess(guess, userName);

        await _mediator.Publish(new PlayerGuessed(imageIdentifier, Guid.Parse(oracleId), guess, Guid.Parse(guesserId), Guid.Parse(gameId), Guid.Parse(sessionId), userName, timeOfGuess));
    }

    public async Task OracleRevealedATile(string oracleId, string imageId)
    {
        await _mediator.Publish(new OracleRevealedATile(Guid.Parse(oracleId), imageId));
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