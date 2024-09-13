using Image_guesser.Core.Domain.SessionContext.Events;
using MediatR;

namespace Image_guesser.Core.Domain.SessionContext.Handlers;

public class SessionClosedHandler : INotificationHandler<SessionClosed>
{
    public Task Handle(SessionClosed notification, CancellationToken cancellationToken)
    {
        if (notification.Session != null)
        {
            notification.Session.SessionStatus = SessionStatus.Closed;
        }

        return Task.CompletedTask;
    }
}