using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.GameContext.Services;
using Image_guesser.Core.Domain.LeaderboardContext.Repository;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SignalRContext.Services.Hub;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.LeaderboardContext.Services;

public class LeaderboardService(ILeaderboardRepository leaderBoardRepository, IGameService gameService, IHubService hubService) : ILeaderboardService
{
    private readonly ILeaderboardRepository _leaderBoardRepository = leaderBoardRepository ?? throw new ArgumentNullException(nameof(leaderBoardRepository));
    private readonly IGameService _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    private readonly IHubService _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));

    private async Task<(string? oracle, string? oracleType)> GetNameAndTypeOfOracle(BaseGame baseGame)
    {
        var userOracle = (await _gameService.GetGameById<User>(baseGame.Id))?.Oracle.Entity;
        var AIOracle = (await _gameService.GetGameById<AI>(baseGame.Id))?.Oracle.Entity;
        var OracleUsername = userOracle?.UserName;
        var OracleAIType = AIOracle?.AI_Type;
        var oracleType = userOracle?.GetType().Name ?? AIOracle?.GetType().Name;
        var oracle = OracleUsername ?? OracleAIType.ToString();

        return (oracle, oracleType);
    }

    public async Task EvaluateSessionGamesForLeaderboard(List<BaseGame> baseGames)
    {
        foreach (var baseGame in baseGames)
        {
            var (oracle, oracleType) = await GetNameAndTypeOfOracle(baseGame);

            baseGame.Guessers.ForEach(async guesser =>
            {
                var name = guesser.Name;
                var leaderboardEntry = await _leaderBoardRepository.GetLeaderboardEntry(name, oracle) ?? await CreateLeaderboardEntry(name, oracle, oracleType);

                await UpdateLeaderboardEntry(leaderboardEntry, guesser.Points);
            });
        }

        await _hubService.ReloadAllClientsPageAtGivenPathName("/");
    }

    public async Task<LeaderboardEntry> CreateLeaderboardEntry(string name, string? oracle, string? oracleType)
    {
        var leaderboardEntry = new LeaderboardEntry(name, oracle, oracleType);

        await _leaderBoardRepository.AddLeaderboardEntry(leaderboardEntry);

        return leaderboardEntry;
    }

    public List<LeaderboardEntry> GetLeaderboardEntries()
    {
        return _leaderBoardRepository.GetLeaderboardEntries();
    }

    public async Task UpdateLeaderboardEntry(LeaderboardEntry leaderboardEntry, int points)
    {
        await _leaderBoardRepository.UpdateLeaderboardEntry(leaderboardEntry, points);
    }
}