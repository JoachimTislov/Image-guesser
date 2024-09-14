using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

public class Game : BaseEntity
{
    public Game() { }

    public Game(
        Guid sessionId,
        List<User> users,
        string gameMode,
        Guid oracleId,
        bool oracleIsAI
    )
    {
        Id = Guid.NewGuid();
        SessionId = sessionId;
        GameMode = gameMode;
        OracleId = oracleId;
        OracleIsAI = oracleIsAI;

        foreach (var user in users)
        {
            var guesser = new Guesser(user.UserName!, Id);
            Guessers.Add(guesser);
        }
    }

    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid OracleId { get; set; }
    public List<Guesser> Guessers { get; set; } = [];
    public string GameMode { get; set; } = string.Empty;
    public bool OracleIsAI { get; set; }
    private GameStatus GameStatus = GameStatus.Started;
    public DateTime TimeOfCreation { get; set; } = DateTime.Now;

    public void GameOver()
    {
        GameStatus = GameStatus.Finished;
    }

    public bool IsGameOver()
    {
        if (GameStatus == GameStatus.Finished)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}