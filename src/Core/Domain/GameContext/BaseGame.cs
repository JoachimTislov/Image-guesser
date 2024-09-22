using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext;

public class BaseGame : BaseEntity
{
    public BaseGame() { }

    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public List<Guesser> Guessers { get; set; } = [];
    public string GameMode { get; set; } = string.Empty;
    private GameStatus GameStatus = GameStatus.Started;
    public DateTime TimeOfCreation { get; set; } = DateTime.Now;

    public void GameOver()
    {
        GameStatus = GameStatus.Finished;
    }

    public bool IsGameOver()
    {
        return GameStatus == GameStatus.Finished;
    }
}