using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Events;

public record PlayerGuessedCorrectly : BaseDomainEvent
{
    public PlayerGuessedCorrectly(int points, Guid gameId, Guid guesserId, string gameMode, int numberOfGames)
    {
        Points = points;
        GameId = gameId;
        GuesserId = guesserId;
        GameMode = gameMode;
        NumberOfGames = numberOfGames;
    }
    public Guid GuesserId { get; }
    public Guid GameId { get; }
    public int Points { get; }

    //Leaderboard
    public string GameMode { get; }
    public int NumberOfGames { get; set; }
}