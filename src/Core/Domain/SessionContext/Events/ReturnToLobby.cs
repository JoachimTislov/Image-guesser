
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext.Events;

public record ReturnToLobby : BaseDomainEvent
{
    public ReturnToLobby(Guid sessionId)
    {
        SessionId = sessionId;
    }
    public Guid SessionId { get; }
}