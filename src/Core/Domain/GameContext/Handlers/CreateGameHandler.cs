using MediatR;
using Microsoft.AspNetCore.SignalR;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Core.Domain.GameContext.Pipelines;

namespace Image_guesser.Core.Domain.GameContext.Handlers;

public class CreateGameHandler(IMediator mediator,
        IHubContext<GameHub, IGameClient> hubContext,
        IOracleService oracleService) : INotificationHandler<CreateGame>
{
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        var numberOfRounds = notification.Options.NumberOfRounds;
        var chosenOracle = notification.Users.FirstOrDefault(u => u.Id == notification.Oracle) ?? throw new Exception("Chosen Oracle is not in the list of users");

        var OracleId = await _oracleService.CreateOracle(
                     notification.Options.UseAI, notification.ImageIdentifier,
                     numberOfRounds, chosenOracle,
                     notification.Options.GameMode.ToString());

        var game = new Game(notification.SessionId, notification.Users, notification.Options.GameMode.ToString(), OracleId, notification.Options.UseAI);

        var result = await _mediator.Send(new AddGame.Request(game), cancellationToken);

        if (result.Success)
        {
            await _hubContext.Clients.Group(result.CreatedGame.SessionId.ToString()).RedirectToLink($"/Game/{result.CreatedGame.Id}");
        }
    }
}