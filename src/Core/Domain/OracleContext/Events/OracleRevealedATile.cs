using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext.Events;

public record OracleRevealedATile : BaseDomainEvent
{
    public OracleRevealedATile(Guid oracleId)
    {
        OracleId = oracleId;
    }
    public Guid OracleId { get; }
}