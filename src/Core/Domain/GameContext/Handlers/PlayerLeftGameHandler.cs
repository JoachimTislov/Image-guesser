using MediatR;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class PlayerLeftGameHandler(IGameService gameService, ISessionService sessionService, IMediator mediator, IHubService hubService) : INotificationHandler<PlayerLeftGame>
{
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    public async Task Handle(PlayerLeftGame notification, CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetSessionById(notification.SessionId);
        var game = await _gameService.GetBaseGameById(notification.GameId);

        var gameIsFreeForAll = session.Options.IsGameMode(GameMode.FreeForAll);

        // Can maybe transfer the oracle role over to someone else.. 
        var GameShouldBeTerminated =
            session.UserIsOracle(Guid.Parse(notification.UserId)) ||
            notification.GuesserId == null ||
            !gameIsFreeForAll ||
            gameIsFreeForAll && game.Guessers.Count <= 2;

        // check if user was oracle, probably not the best way to do it, but oracle won't be a guesser or in other words should'nt be a guesser
        if (GameShouldBeTerminated)
        {
            await _mediator.Publish(new GameTerminated(notification.GameId, notification.SessionId), cancellationToken);
        }
        else if (notification.GuesserId != null)
        {
            await _gameService.RemoveGuesserFromGame(notification.GuesserId.Value, notification.GameId);

            await _hubService.RedirectClientToPage(notification.UserId, $"/Lobby/{notification.SessionId}");
            await _hubService.ReloadGroupPage(notification.SessionId.ToString());
        }
    }
}