using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext;

public record OracleRevealedATile(Guid OracleId) : BaseDomainEvent;
