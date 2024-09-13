using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record PlayerGuessedIncorrectly : BaseDomainEvent
{
    public PlayerGuessedIncorrectly(Guid guesserId, Guid gameId)
    {
        GuesserId = guesserId;
        GameId = gameId;
    }
    public Guid GuesserId { get; }
    public Guid GameId { get; }
}