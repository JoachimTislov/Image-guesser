using MediatR;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.SessionContext.Events;

namespace Image_guesser.Core.Domain.SignalRContext.Handlers;

public class UserClickedJoinSessionHandler(IHubService hubService) : INotificationHandler<UserClickedJoinSession>
{
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    public async Task Handle(UserClickedJoinSession notification, CancellationToken cancellationToken)
    {
        await _hubService.RedirectGroupToPage(notification.SessionId, $"/Lobby/{notification.SessionId}");
    }
}