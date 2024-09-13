using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record GameFinished : BaseDomainEvent
{
    public GameFinished(Guid gameId)
    {
        GameId = gameId;
    }
    public Guid GameId { get; }
}