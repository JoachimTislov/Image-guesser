using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext.Events;

public record UserClickedJoinSession : BaseDomainEvent
{
    public UserClickedJoinSession(string sessionId, string userId)
    {
        SessionId = sessionId;
        UserId = userId;
    }
    public string SessionId { get; }
    public string UserId { get; }
}
