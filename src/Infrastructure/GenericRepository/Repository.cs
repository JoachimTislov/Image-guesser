using System.Linq.Expressions;
using Image_guesser.Core.Exceptions;
using Image_guesser.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Infrastructure.GenericRepository;

public class Repository(ImageGameContext context) : IRepository
{
    public readonly ImageGameContext _context = context ?? throw new ArgumentNullException(nameof(context));

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

    public async Task<T> GetById<T, I>(I Id) where T : class
    {
        var entity = await _context.FindAsync<T>(Id) ?? throw new EntityNotFoundException($"Entity of type {typeof(T)} and Id of type {typeof(I)} with value {Id} was not found");
        return entity;
    }

    public IEnumerable<T> Where<T>(Expression<Func<T, bool>> exp) where T : class
    {
        return GetEntities<T>().Where(exp);
    }

    public async Task<Entity> GetSingleWhere<Entity, IdentifierType>(Expression<Func<Entity, bool>> WhereExp, IdentifierType Id) where Entity : BaseEntity
    {
        return await GetEntities<Entity>().Where(WhereExp).SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException($"Entity of type {typeof(Entity)} with where expression {WhereExp} and Identifier: {Id} of type: {typeof(IdentifierType)}, was not found");
    }

    public async Task<ReturnType> GetSingleWhereAndSelectItem<Entity, IdentifierType, ReturnType>(Expression<Func<Entity, bool>> WhereExp, Expression<Func<Entity, ReturnType>> SelectExp, IdentifierType Id) where Entity : BaseEntity
    {
        return await GetEntities<Entity>().Where(WhereExp).Select(SelectExp).SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException($"Entity of type {typeof(Entity)} with where expression {WhereExp}, select expression {SelectExp} and Identifier: {Id} of type: {typeof(IdentifierType)}, was not found");
    }

    public bool Any<T>() where T : class
    {
        return GetEntities<T>().Any();
    }

    public async Task Update<T>(T entity) where T : class
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
}