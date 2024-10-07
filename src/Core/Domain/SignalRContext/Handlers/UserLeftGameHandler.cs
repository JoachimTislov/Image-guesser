using MediatR;
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;

namespace Image_guesser.Core.Domain.SignalRContext.Handlers;

public class UserLeftGameHandler(IHubService hubService) : INotificationHandler<UserLeftGame>
{
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    public async Task Handle(UserLeftGame notification, CancellationToken cancellationToken)
    {
        await _hubService.RedirectClientToPage(notification.UserId, $"/Lobby/{notification.SessionId}");
    }
}