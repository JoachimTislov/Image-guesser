using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.LeaderboardContext;

public class LeaderboardEntry : BaseEntity
{
    public LeaderboardEntry() { }

    public LeaderboardEntry(string name, string? oracle, string? oracleType)
    {
        Name = name;
        Oracle = oracle ?? "Unknown";
        OracleType = oracleType ?? "Unknown";
    }

    public string Name { get; protected set; } = string.Empty;
    public string Oracle { get; protected set; } = "Unknown";
    public string OracleType { get; protected set; } = "Unknown";
    public int Score { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }

    public int AverageScorePerGame => GamesPlayed == 0 ? 0 : Score / GamesPlayed;
    public int AverageScorePerWin => GamesWon == 0 ? 0 : Score / GamesWon;
    public double WinRate => GamesPlayed == 0 ? 0 : Math.Round((double)GamesWon / GamesPlayed * 100);

    public void UpdateLeaderboardEntry(int score)
    {
        Score += score;
        GamesPlayed++;

        // Simple way of checking if the player won the game
        if (score > 0) GamesWon++;
    }
}