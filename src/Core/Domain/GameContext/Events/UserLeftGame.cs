
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record UserLeftGame : BaseDomainEvent
{
    public UserLeftGame(string userId, Guid? guesserId, Guid gameId, Guid sessionId)
    {
        UserId = userId;
        GuesserId = guesserId;
        GameId = gameId;
        SessionId = sessionId;
    }

    public string UserId { get; }
    public Guid? GuesserId { get; }
    public Guid GameId { get; }
    public Guid SessionId { get; }
}