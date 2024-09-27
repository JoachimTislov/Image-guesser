using MediatR;
using Microsoft.AspNetCore.SignalR;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.GameContext.Responses;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class CreateGameHandler(IHubContext<GameHub, IGameClient> hubContext, IGameService gameService) : INotificationHandler<CreateGame>
{
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        InitializeGameResponse result;

        if (notification.Options.IsOracleAI())
        {
            result = await _gameService.SetupGameWithAIAsOracle(notification.SessionId, notification.Users, notification.Options.GameMode.ToString(), notification.Options.ImageIdentifier, notification.Options.AI_Type);
        }
        else
        {
            result = await _gameService.SetupGameWithUserAsOracle(notification.SessionId, notification.Users, notification.Options.GameMode.ToString(), notification.ChosenOracleId);

        }

        if (result.Success)
        {
            await _hubContext.Clients.Group(result.SessionId.ToString()).RedirectToLink($"/Game/{result.Id}");
        }
    }
}