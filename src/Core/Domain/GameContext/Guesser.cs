using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

public class Guesser(string name, Guid gameId) : BaseEntity
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; } = gameId;
    public string Name { get; set; } = name;
    public int Points { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public int Guesses { get; private set; }
    public int WrongGuessCounter { get; private set; }

    public void IncrementGuesses()
    {
        Guesses++;
    }

    public bool ReachedMaxWrongGuess()
    {
        // > to prevent case where the wrong guess counter is somehow higher than three
        return WrongGuessCounter >= 3;
    }

    public void ResetWrongGuesses()
    {
        WrongGuessCounter = 0;
    }

    public void IncrementWrongGuessCounter()
    {
        WrongGuessCounter++;
    }
}
