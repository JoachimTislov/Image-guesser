using Image_guesser.Core.Domain.SessionContext.Events;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using MediatR;

namespace Image_guesser.Core.Domain.SignalRContext.Handlers;

public class ReturnToLobbyHandler(IHubService hubService) : INotificationHandler<ReturnToLobby>
{
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
    public async Task Handle(ReturnToLobby notification, CancellationToken cancellationToken)
    {
        await _hubService.RedirectGroupToPage(notification.SessionId.ToString(), $"/Lobby/{notification.SessionId}");
    }
}