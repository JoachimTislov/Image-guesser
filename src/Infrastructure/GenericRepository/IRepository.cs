using System.Linq.Expressions;
using Image_guesser.SharedKernel;

namespace Image_guesser.Infrastructure.GenericRepository;

public interface IRepository
{
    Task Add<T>(T entity) where T : BaseEntity;
    Task Delete<T>(T entity) where T : BaseEntity;
    List<T> GetAll<T>() where T : BaseEntity;
    Task<T> GetById<T, I>(I Id) where T : class;
    List<T> WhereAndInclude<T, I>(Expression<Func<T, bool>> whereExp, Expression<Func<T, I>> includeExp) where T : BaseEntity;
    Task<T?> WhereAndInclude_SingleOrDefault<T, I>(Expression<Func<T, bool>> whereExp, Expression<Func<T, I>> includeExp) where T : BaseEntity;
    Task<T?> WhereAndInclude2x_SingleOrDefault<T, I, J>(Expression<Func<T, bool>> whereExp, Expression<Func<T, I>> includeExp, Expression<Func<T, J>> includeExp2) where T : BaseEntity;
    Task<T?> GetSingleWhere<T>(Expression<Func<T, bool>> whereExp) where T : BaseEntity;
    IEnumerable<T> Where<T>(Expression<Func<T, bool>> exp) where T : BaseEntity;
    IQueryable<ReturnType> WhereAndSelect<T, ReturnType>(Expression<Func<T, bool>> whereExp, Expression<Func<T, ReturnType>> selectExp) where T : BaseEntity;
    Task<ReturnType?> WhereAndSelect_SingleOrDefault<T, ReturnType>(Expression<Func<T, bool>> whereExp, Expression<Func<T, ReturnType>> selectExp) where T : class;
    bool Any<T>() where T : BaseEntity;
    Task Update<T>(T entity) where T : class;
}