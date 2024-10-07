using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record CreateGame : BaseDomainEvent
{
    public CreateGame(Guid sessionId)
    {
        SessionId = sessionId;
    }
    public Guid SessionId { get; }
}