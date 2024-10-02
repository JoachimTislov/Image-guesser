using Image_guesser.Core.Domain.SessionContext;

namespace Image_guesser.Core.Domain.GameContext.Services;

public interface IGameService
{
    Task CreateGame(Session session);
    Task<Game<T>> GetGameById<T>(Guid Id) where T : class;
    Task<BaseGame> GetBaseGameById(Guid gameId);
    Task<Guesser> GetGuesserById(Guid Id);
}