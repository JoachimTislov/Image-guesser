using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Services;

public interface IGameService
{
    Task CreateGame(Session session, string imageIdentifier);
    Task CreateAndAddGuess(string guess, string nameOfGuesser, string timeOfGuess, Guid gameId);
    Task RestartGameWithNewOracle(Guid gameId, AI_Type AI_Type);
    Task AddGuesserFromGame(Guid guesserId, Guid gameId);
    Task RemoveGuesserFromGame(Guid guesserId, Guid gameId);
    Task<Game<T>?> GetGameById<T>(Guid Id) where T : class;
    Task DeleteGuesserById(Guid Id);
    Task<BaseGame> GetBaseGameById(Guid gameId);
    Task<Guesser> GetGuesserById(Guid Id);
    List<BaseGame> GetGames();
    Task UpdateGame<TGame>(TGame game) where TGame : BaseEntity;
    Task UpdateGuesser(Guesser guesser);
}