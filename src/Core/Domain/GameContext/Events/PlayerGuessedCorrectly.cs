using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record PlayerGuessedCorrectly : BaseDomainEvent
{
    public PlayerGuessedCorrectly(Guid guesserId, int points, Guid gameId)
    {
        GuesserId = guesserId;
        Points = points;
        GameId = gameId;
    }
    public Guid GuesserId { get; }
    public int Points { get; }
    public Guid GameId { get; }
}