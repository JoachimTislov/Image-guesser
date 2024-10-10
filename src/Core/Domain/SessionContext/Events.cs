
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext;

public record ReturnToLobby(Guid SessionId) : BaseDomainEvent;

public record SessionClosed(Guid SessionId) : BaseDomainEvent;