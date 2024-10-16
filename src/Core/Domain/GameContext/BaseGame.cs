using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

public class BaseGame : BaseEntity
{
    public BaseGame() { }

    public Guid Id { get; private set; }
    public Guid SessionId { get; protected set; }
    public Guid BaseOracleId { get; set; }
    public List<Guesser> Guessers { get; private set; } = [];
    public GameMode GameMode { get; set; }
    public GameStatus GameStatus { get; private set; } = GameStatus.Started;
    public List<Guess> GuessLog { get; private set; } = [];

    public bool AddGuesser(Guesser guesser)
    {
        if (!Guessers.Contains(guesser))
        {
            Guessers.Add(guesser);

            return true;
        }

        return false;
    }

    public bool RemoveGuesser(Guesser guesser)
    {
        return Guessers.Remove(guesser);
    }

    public static Guesser CreateGuesser(string username)
    {
        return new Guesser(username);
    }

    public void CreateAndAddGuess(string guess, string nameOfGuesser, string timeOfGuess)
    {
        GuessLog.Add(new Guess(guess, nameOfGuesser, timeOfGuess));
    }

    public void GameOver() => GameStatus = GameStatus.Finished;

    public void Terminated() => GameStatus = GameStatus.Terminated;

    public bool IsFinished() => GameStatus == GameStatus.Finished;
    public bool IsTerminated() => GameStatus == GameStatus.Terminated;
}