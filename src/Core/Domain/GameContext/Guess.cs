
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

public class Guess : BaseEntity
{
    public Guess() { }
    public Guess(string guess, string nameOfGuesser, string timeOfGuess)
    {
        GuessMessage = guess;
        NameOfGuesser = nameOfGuesser;
        TimeOfGuess = timeOfGuess;
    }

    public Guid Id { get; private set; }
    public string GuessMessage { get; private set; } = string.Empty;
    public string NameOfGuesser { get; private set; } = string.Empty;
    public string TimeOfGuess { get; private set; } = string.Empty;
}