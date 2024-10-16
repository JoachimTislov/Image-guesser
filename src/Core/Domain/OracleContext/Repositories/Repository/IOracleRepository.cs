using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.OracleContext.Repositories.Repository;

public interface IOracleRepository
{
    Task AddOracle<T>(T oracle) where T : BaseEntity;
    Task<Oracle<T>?> GetOracleById<T>(Guid Id) where T : class;
    Task<BaseOracle> GetBaseOracleById(Guid Id);
    List<string> GetImageIdentifierOfAllPreviousPlayedGamesInTheSession(Guid sessionId);
    Task DeleteOracle<T>(Guid Id) where T : BaseEntity;
    Task UpdateOracle<T>(T baseOracle) where T : BaseEntity;
}