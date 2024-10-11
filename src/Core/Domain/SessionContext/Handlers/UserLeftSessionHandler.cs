using Image_guesser.Core.Domain.SessionContext.Services;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class UserLeftSessionHandler(ISessionService sessionService, IMediator mediator, IHubService hubService) : INotificationHandler<UserLeftSession>
{
    private readonly ISessionService _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    public async Task Handle(UserLeftSession notification, CancellationToken cancellationToken)
    {
        var (sessionId, userId) = notification;

        var IsUserSessionHost = await _sessionService.CheckIfUserIsSessionHost(sessionId, Guid.Parse(userId));
        if (IsUserSessionHost)
        {
            await _mediator.Publish(new SessionClosed(sessionId), cancellationToken);
        }
        else
        {
            await _sessionService.RemoveUserFromSession(userId, sessionId);
            await _hubService.RedirectClientToPage(userId, "/");

            await _hubService.RemoveFromGroupAsync(sessionId.ToString(), userId);
            await _hubService.ReloadGroupPage(sessionId.ToString());
        }
    }
}
