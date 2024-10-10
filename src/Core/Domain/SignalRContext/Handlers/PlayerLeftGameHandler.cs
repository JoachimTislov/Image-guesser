using MediatR;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.GameContext;

namespace Image_guesser.Core.Domain.SignalRContext.Handlers;

public class PlayerLeftGameHandler(IHubService hubService) : INotificationHandler<PlayerLeftGame>
{
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    public async Task Handle(PlayerLeftGame notification, CancellationToken cancellationToken)
    {
        await _hubService.RedirectClientToPage(notification.UserId, $"/Lobby/{notification.SessionId}");
        await _hubService.ReloadGroupPage(notification.SessionId.ToString());
    }
}