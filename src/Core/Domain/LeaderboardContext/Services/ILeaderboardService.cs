using Image_guesser.Core.Domain.GameContext;

namespace Image_guesser.Core.Domain.LeaderboardContext.Services;

public interface ILeaderboardService
{
    Task EvaluateSessionGamesForLeaderboard(List<BaseGame> baseGames);
    Task<LeaderboardEntry> CreateLeaderboardEntry(string name, string? oracle, string? oracleType);
    List<LeaderboardEntry> GetLeaderboardEntries();
    Task UpdateLeaderboardEntry(LeaderboardEntry leaderboardEntry, int points);
}