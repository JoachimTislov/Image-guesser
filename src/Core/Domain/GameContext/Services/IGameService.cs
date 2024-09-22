using Image_guesser.Core.Domain.GameContext.Responses;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.GameContext.Services;

public interface IGameService
{
    Task<InitializeGameResponse> SetupGameWithUserAsOracle(Guid sessionId, List<User> users, string gameMode, Guid ChosenOracle);
    Task<InitializeGameResponse> SetupGameWithAIAsOracle(Guid sessionId, List<User> users, string gameMode, string ImageIdentifier, AI_Type AI_Type);
    Task<Game<T>> GetGameById<T>(Guid Id) where T : class;
    Task<BaseGame> GetBaseGameById(Guid gameId);
    Task<Guesser> GetGuesserById(Guid Id);
}