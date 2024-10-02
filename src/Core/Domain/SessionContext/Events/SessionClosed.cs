using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext.Events;

public record SessionClosed : BaseDomainEvent
{
    public SessionClosed(Guid sessionId)
    {
        SessionId = sessionId;
    }
    public Guid SessionId { get; }
}
