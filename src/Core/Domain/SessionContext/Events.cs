
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext;

public record ReturnToLobby(Guid SessionId) : BaseDomainEvent;

public record SessionClosed(Guid SessionId) : BaseDomainEvent;

public record UserLeftSession(Guid SessionId, string UserId) : BaseDomainEvent;

public record UserJoinedSession(string SessionId, string UserId) : BaseDomainEvent;
