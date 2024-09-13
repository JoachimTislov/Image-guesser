using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext.Events;

public record SessionClosed : BaseDomainEvent
{
    public SessionClosed(Session session)
    {
        Session = session;
    }
    public Session Session { get; }
}
