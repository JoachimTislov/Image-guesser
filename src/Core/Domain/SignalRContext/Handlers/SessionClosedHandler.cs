using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using MediatR;

namespace Image_guesser.Core.Domain.SignalRContext.Handlers;

public class SessionClosedHandler(IHubService hubService) : INotificationHandler<SessionClosed>
{
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    public async Task Handle(SessionClosed notification, CancellationToken cancellationToken)
    {
        await _hubService.CloseGroup(notification.SessionId.ToString());
    }
}