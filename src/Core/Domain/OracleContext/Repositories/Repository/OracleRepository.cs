using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext.Repositories.Repository;

public class OracleRepository(IRepository repository) : IOracleRepository
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task AddOracle<T>(T oracle) where T : BaseEntity
    {
        await _repository.Add(oracle);
    }

    public async Task<Oracle<T>> GetOracleById<T>(Guid Id) where T : BaseEntity
    {
        return await _repository.GetSingleWhere<Oracle<T>>(o => o.Id == Id) ?? throw new EntityNotFoundException($"Oracle was not found by id: {Id}");
    }

    public async Task<BaseOracle> GetBaseOracleById(Guid Id)
    {
        return await _repository.GetSingleWhere<BaseOracle>(o => o.Id == Id) ?? throw new EntityNotFoundException($"Base Oracle was not found by id: {Id}");
    }

    public List<string> GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(Guid sessionId)
    {
        IQueryable<Guid> BaseOracleIds = _repository.WhereAndSelect<BaseGame, Guid>(g => g.SessionId == sessionId, g => g.BaseOracleId);

        return [.. _repository.WhereAndSelect<BaseOracle, string>(b => BaseOracleIds.Contains(b.Id), b => b.ImageIdentifier)];
    }

    public async Task DeleteOracle<T>(Guid Id) where T : BaseEntity
    {
        await _repository.Delete(await GetOracleById<T>(Id));
    }

    public async Task UpdateOracle<T>(T baseOracle) where T : BaseEntity
    {
        await _repository.Update(baseOracle);
    }
}