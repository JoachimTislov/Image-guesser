using System.Linq.Expressions;
using Image_guesser.SharedKernel;

namespace Image_guesser.Infrastructure.GenericRepository;

public interface IRepository
{
    Task Add<T>(T entity) where T : class;
    Task Delete<T>(T entity) where T : class;
    List<T> GetAll<T>() where T : class;
    Task<T> GetById<T, I>(I Id) where T : class;
    Task<T> WhereAndInclude_SingleOrDefault<T, I>(Expression<Func<T, bool>> whereExp, Expression<Func<T, I>> includeExp) where T : class;
    Task<T> GetSingleWhere<T, IdentifierType>(Expression<Func<T, bool>> whereExp, IdentifierType Id) where T : BaseEntity;
    IEnumerable<T> Where<T>(Expression<Func<T, bool>> exp) where T : class;
    Task<ReturnType> WhereAndSelect_SingleOrDefault<T, IdentifierType, ReturnType>(Expression<Func<T, bool>> whereExp, Expression<Func<T, ReturnType>> selectExp, IdentifierType Id) where T : BaseEntity;
    bool Any<T>() where T : class;
    Task Update<T>(T entity) where T : class;
}