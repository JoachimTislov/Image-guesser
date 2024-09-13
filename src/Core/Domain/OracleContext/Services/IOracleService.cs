
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.OracleContext.Services
{
    public interface IOracleService
    {
        RandomNumbersAI CreateRandomNumbersAI(int PieceCountPerImage);

        Task<Guid> CreateOracle(bool OracleIsAI, string ImageIdentifier,
                        int NumberOfRounds, User ChosenOracle, string GameMode);

        public record Response(bool IsGuessCorrect, string WinnerText);
        Task<Response> CheckGuess(string Guess, string ImageIdentifier, User User, Session Session);
    }
}