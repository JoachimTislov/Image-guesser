using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record GameFinished : BaseDomainEvent
{
    public GameFinished(BaseGame game, Guid guesserId, int points)
    {
        Game = game;
        GuesserId = guesserId;
        Points = points;
    }
    public BaseGame Game { get; }
    public Guid GuesserId { get; }
    public int Points { get; }
}