using Image_guesser.Core.Domain.SessionContext;

namespace Image_guesser.Core.Domain.OracleContext.Services
{
    public interface IOracleService
    {
        Task<Oracle<AI>> CreateAIOracle(string ImageIdentifier, AI_Type AI_Type);
        Oracle<T> CreateOracle<T>(T ChosenOracle, string ImageIdentifier) where T : class;
        Task<Oracle<T>> GetOracleById<T>(Guid Id) where T : class;
        Task<BaseOracle> GetBaseOracleById(Guid Id);
        int CalculatePoints(int pieceCount, int numberOfTilesRevealed);
        bool CheckGuess(string guess, string imageName);
        Task<(bool IsGuessCorrect, string WinnerText)> HandleGuess(string Guess, string ImageIdentifier, string Username, Guid ChosenOracleId, GameMode GameMode);
    }
}