using Image_guesser.Infrastructure.GenericRepository;

namespace Image_guesser.Core.Domain.LeaderboardContext.Repository;

public class LeaderboardRepository(IRepository repository) : ILeaderboardRepository
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<LeaderboardEntry?> GetLeaderboardEntry(string name, string? oracle)
    {
        return await _repository.GetSingleWhere<LeaderboardEntry>(e => e.Name == name && e.Oracle == oracle);
    }

    public async Task AddLeaderboardEntry(LeaderboardEntry leaderboardEntry)
    {
        await _repository.Add(leaderboardEntry);
    }

    public List<LeaderboardEntry> GetLeaderboardEntries()
    {
        return _repository.GetAll<LeaderboardEntry>();
    }

    public async Task UpdateLeaderboardEntry(LeaderboardEntry leaderboardEntry, int points)
    {
        leaderboardEntry.UpdateLeaderboardEntry(points);
        await _repository.Update(leaderboardEntry);
    }
}