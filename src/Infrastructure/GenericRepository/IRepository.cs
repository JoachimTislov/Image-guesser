using System.Linq.Expressions;

namespace Image_guesser.Infrastructure.GenericRepository;

public interface IRepository
{
    public Task Add<T>(T entity) where T : class;
    public Task Delete<T>(T entity) where T : class;
    public List<T> GetAll<T>() where T : class;
    public Task<T> GetById<T>(Guid Id) where T : class;
    IEnumerable<T> Where<T>(Expression<Func<T, bool>> exp) where T : class;
    Task<T> GetSingleWhere<T, I>(Expression<Func<T, bool>> exp, I Id) where T : class;
    public Task Update<T>(T entity) where T : class;
}