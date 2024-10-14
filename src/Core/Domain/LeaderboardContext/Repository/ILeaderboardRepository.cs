
namespace Image_guesser.Core.Domain.LeaderboardContext.Repository;

public interface ILeaderboardRepository
{
    Task<LeaderboardEntry?> GetLeaderboardEntry(string name, string? oracle);
    Task AddLeaderboardEntry(LeaderboardEntry leaderboardEntry);
    List<LeaderboardEntry> GetLeaderboardEntries();
    Task UpdateLeaderboardEntry(LeaderboardEntry leaderboardEntry, int points);
}