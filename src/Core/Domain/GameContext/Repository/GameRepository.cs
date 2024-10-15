
using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.GameContext.Repository;

public class GameRepository(IRepository repository) : IGameRepository
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Game<T>?> GetGameById<T>(Guid Id) where T : class
    {
        return await _repository.WhereAndInclude_SingleOrDefault<Game<T>, T>(g => g.Id == Id, g => g.Oracle.Entity);
    }

    public async Task<BaseGame> GetBaseGameById(Guid Id)
    {
        return await _repository.WhereAndInclude2x_SingleOrDefault<BaseGame, List<Guesser>, List<Guess>>(bg => bg.Id == Id, bg => bg.Guessers, bg => bg.GuessLog) ?? throw new EntityNotFoundException($"BaseGame with Id: {Id} was not found");
    }

    public async Task<Guesser> GetGuesserById(Guid GuesserId)
    {
        return await _repository.GetById<Guesser, Guid>(GuesserId);
    }

    public List<BaseGame> GetGames()
    {
        return _repository.GetAll<BaseGame>();
    }

    public async Task UpdateGameOrGuesser<TGameOrGuesser>(TGameOrGuesser game) where TGameOrGuesser : BaseEntity
    {
        await _repository.Update(game);
    }

    public async Task DeleteGuesserById(Guid Id)
    {
        var guesser = await GetGuesserById(Id);
        await _repository.Delete(guesser);
    }
}