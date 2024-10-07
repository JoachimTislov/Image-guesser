using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record GameTerminated : BaseDomainEvent
{
    public GameTerminated(Guid gameId, Guid sessionId)
    {
        GameId = gameId;
        SessionId = sessionId;
    }
    public Guid GameId { get; }
    public Guid SessionId { get; }
}