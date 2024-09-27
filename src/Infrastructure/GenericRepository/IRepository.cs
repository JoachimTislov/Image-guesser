using System.Linq.Expressions;
using Image_guesser.SharedKernel;

namespace Image_guesser.Infrastructure.GenericRepository;

public interface IRepository
{
    Task Add<T>(T entity) where T : class;
    Task Delete<T>(T entity) where T : class;
    List<T> GetAll<T>() where T : class;
    Task<T> GetById<T, I>(I Id) where T : class;
    Task<Entity> GetSingleWhere<Entity, IdentifierType>(Expression<Func<Entity, bool>> WhereExp, IdentifierType Id) where Entity : BaseEntity;
    IEnumerable<T> Where<T>(Expression<Func<T, bool>> exp) where T : class;
    Task<ReturnType> GetSingleWhereAndSelectItem<Entity, IdentifierType, ReturnType>(Expression<Func<Entity, bool>> WhereExp, Expression<Func<Entity, ReturnType>> SelectExp, IdentifierType Id) where Entity : BaseEntity;
    bool Any<T>() where T : class;
    Task Update<T>(T entity) where T : class;
}