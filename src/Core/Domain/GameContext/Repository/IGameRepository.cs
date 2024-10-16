
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Repository;

public interface IGameRepository
{
    Task<Game<T>?> GetGameById<T>(Guid Id) where T : class;
    Task<List<BaseGame>> GetXAmountOfRecentGames(int amount);
    Task<BaseGame> GetBaseGameById(Guid Id);
    Task<Guesser> GetGuesserById(Guid GuesserId);
    List<BaseGame> GetGames();
    Task UpdateGameOrGuesser<TGameOrGuesser>(TGameOrGuesser game) where TGameOrGuesser : BaseEntity;
    Task DeleteGuesserById(Guid Id);
}