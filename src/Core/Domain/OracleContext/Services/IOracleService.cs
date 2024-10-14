using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using OneOf;

namespace Image_guesser.Core.Domain.OracleContext.Services
{
    public interface IOracleService
    {
        Task<OneOf<Oracle<User>, Oracle<AI>>> CreateOracle(Guid ChosenOracleId, string imageIdentifier, AI_Type AI_Type, bool OracleIsAI);
        Task<Oracle<AI>> CreateAIOracle(string imageIdentifier, AI_Type AI_Type);
        Task<Oracle<T>> GetOracleById<T>(Guid Id) where T : class;
        Task<BaseOracle> GetBaseOracleById(Guid Id);
        List<string> GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(Guid sessionId);
        int CalculatePoints(int pieceCount, int numberOfTilesRevealed);
        bool CheckGuess(string guess, string imageName);
        Task<(bool IsGuessCorrect, string WinnerText, string ImageName)> HandleGuess(string Guess, string ImageIdentifier, string Username, Guid ChosenOracleId, GameMode GameMode);
        Task DeleteOracle<T>(Guid Id) where T : class;
        Task UpdateBaseOracle(BaseOracle baseOracle);
    }
}