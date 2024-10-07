using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using MediatR;

namespace Image_guesser.Core.Domain.SignalRContext.Handlers;

public class UserLeftSessionOrWasKickedHandler(IHubService hubService) : INotificationHandler<UserLeftSessionOrWasKicked>
{
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    public async Task Handle(UserLeftSessionOrWasKicked notification, CancellationToken cancellationToken)
    {
        await _hubService.LeaveGroup(notification.UserId, notification.SessionId.ToString());
    }
}