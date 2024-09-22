
using Image_guesser.Core.Domain.OracleContext.Responses;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.OracleContext.Services
{
    public interface IOracleService
    {
        Task<Oracle<AI>> CreateAIOracle(string ImageIdentifier, AI_Type AI_Type);
        Oracle<User> CreateUserOracle(User ChosenOracle);
        Task<Oracle<T>> GetOracleById<T>(Guid Id) where T : class;
        Task<BaseOracle> GetBaseOracleById(Guid Id);
        Task<Check_Guess_Response> CheckGuess(
            string Guess,
            string ImageIdentifier,
            string Username,
            Guid ChosenOracle,
            GameMode GameMode
        );
    }
}