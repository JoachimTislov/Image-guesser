using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Infrastructure.GenericRepository;

public class Repository(ImageGameContext context) : IRepository
{
    public readonly ImageGameContext _context = context;

    public DbSet<T> GetEntities<T>() where T : class
    {
        return _context.Set<T>();
    }

    public async Task Add<T>(T entity) where T : class
    {
        GetEntities<T>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete<T>(T entity) where T : class
    {
        GetEntities<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public List<T> GetAll<T>() where T : class
    {
        return [.. GetEntities<T>()];
    }

    public async Task<T> GetById<T>(Guid Id) where T : class
    {
        var entity = await _context.FindAsync<T>(Id) ?? throw new Exception($"Entity of type {typeof(T)} with Id {Id} was not found");
        return entity;
    }

    public IEnumerable<T> Where<T>(Expression<Func<T, bool>> exp) where T : class
    {
        return GetEntities<T>().Where(exp);
    }

    public async Task<T> GetSingleWhere<T, I>(Expression<Func<T, bool>> exp, I Id) where T : class
    {
        return await GetEntities<T>().Where(exp).SingleOrDefaultAsync()
                ?? throw new Exception($"Entity of type {typeof(T)} with expression {exp} and Identifier: {Id} of type: {typeof(I)}, was not found");
    }

    public async Task Update<T>(T entity) where T : class
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
}