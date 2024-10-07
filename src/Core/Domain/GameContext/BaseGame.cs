using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

public class BaseGame : BaseEntity
{
    public BaseGame() { }

    public Guid Id { get; private set; }
    public Guid SessionId { get; protected set; }
    public Guid BaseOracleId { get; protected set; }
    public List<Guesser> Guessers { get; private set; } = [];
    public string GameMode { get; set; } = string.Empty;
    public GameStatus GameStatus { get; private set; } = GameStatus.Started;
    public DateTime TimeOfCreation { get; private set; } = DateTime.Now;

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

    public static Guesser CreateGuesser(string username, Guid gameId)
    {
        return new Guesser(username, gameId);
    }

    public void GameOver() => GameStatus = GameStatus.Finished;

    public void Terminated() => GameStatus = GameStatus.Terminated;

    public bool IsFinished() => GameStatus == GameStatus.Finished;
}