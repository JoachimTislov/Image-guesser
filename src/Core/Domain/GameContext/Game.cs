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
        int numberOfGames,
        Guid oracleId,
        bool oracleIsAI
    )
    {
        Id = Guid.NewGuid(); // Initialize Id if needed
        SessionId = sessionId;
        GameMode = gameMode;
        NumberOfGames = numberOfGames;
        OracleId = oracleId;
        OracleIsAI = oracleIsAI;

        foreach (var user in users)
        {
            var guesser = new Guesser(user.UserName!, Guid.NewGuid());
            Guessers.Add(guesser);
        }
    }

    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid OracleId { get; set; }
    public List<Guesser> Guessers { get; set; } = [];
    public string GameMode { get; set; } = string.Empty;
    public int NumberOfGames { get; set; }
    public bool OracleIsAI { get; set; }
    public GameStatus GameStatus { get; set; } = GameStatus.Started;
    public DateTime Timer { get; set; } = DateTime.Now;

}