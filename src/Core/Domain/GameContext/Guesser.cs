using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

public class Guesser(string name) : BaseEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = name;
    public int Points { get; set; }
    public int Guesses { get; private set; }
    public int WrongGuessCounter { get; private set; }

    public void IncrementGuesses()
    {
        Guesses++;
    }

    public bool ReachedMaxWrongGuesses()
    {
        // > to handle case where the wrong guess counter is somehow higher than three
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
