using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext.Events;

public record UserLeftSessionOrWasKicked : BaseDomainEvent
{
    public UserLeftSessionOrWasKicked(string userId, Guid sessionId)
    {
        UserId = userId;
        SessionId = sessionId;
    }
    public string UserId { get; }
    public Guid SessionId { get; }
}



