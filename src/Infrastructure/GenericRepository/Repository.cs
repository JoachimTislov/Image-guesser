using System.Linq.Expressions;
using Image_guesser.Core.Exceptions;
using Image_guesser.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Infrastructure.GenericRepository;

public class Repository(ImageGameContext context) : IRepository
{
    public readonly ImageGameContext _context = context ?? throw new ArgumentNullException(nameof(context));

    private DbSet<T> GetEntities<T>() where T : class
    {
        return _context.Set<T>();
    }

    public async Task Add<T>(T entity) where T : BaseEntity
    {
        GetEntities<T>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete<T>(T entity) where T : BaseEntity
    {
        GetEntities<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public List<T> GetAll<T>() where T : BaseEntity
    {
        return [.. GetEntities<T>()];
    }

    public async Task<T> GetById<T, I>(I Id) where T : class
    {
        return await _context.FindAsync<T>(Id) ?? throw new EntityNotFoundException($"Entity of type {typeof(T)} and Id of type {typeof(I)} with value {Id} was not found");
    }

    public List<T> WhereAndInclude<T, I>(Expression<Func<T, bool>> whereExp, Expression<Func<T, I>> includeExp) where T : BaseEntity
    {
        return [.. GetEntities<T>().Where(whereExp).Include(includeExp)];
        // An exception is not thrown here since in the game page we are attempting to get both types of games; User And AI and it should only find one of them
        //?? throw new EntityNotFoundException($"Entity not found with where statement: {whereExp} and include expression: {includeExp}")
    }

    public async Task<T?> WhereAndInclude_SingleOrDefault<T, I>(Expression<Func<T, bool>> whereExp, Expression<Func<T, I>> includeExp) where T : BaseEntity
    {
        return await GetEntities<T>().Where(whereExp).Include(includeExp).SingleOrDefaultAsync();
        // An exception is not thrown here since in the game page we are attempting to get both types of games; User And AI and it should only find one of them
        //?? throw new EntityNotFoundException($"Entity not found with where statement: {whereExp} and include expression: {includeExp}")
    }

    public async Task<T?> WhereAndInclude2x_SingleOrDefault<T, I, J>(Expression<Func<T, bool>> whereExp, Expression<Func<T, I>> includeExp, Expression<Func<T, J>> includeExp2) where T : BaseEntity
    {
        return await GetEntities<T>().Where(whereExp).Include(includeExp).Include(includeExp2).SingleOrDefaultAsync();
    }

    public async Task<T?> GetSingleWhere<T>(Expression<Func<T, bool>> whereExp) where T : BaseEntity
    {
        return await GetEntities<T>().Where(whereExp).SingleOrDefaultAsync();
        //?? throw new EntityNotFoundException($"Entity of type {typeof(T)} with where expression {whereExp} and Identifier: {Id}, was not found");
    }

    public IEnumerable<T> Where<T>(Expression<Func<T, bool>> exp) where T : BaseEntity
    {
        return GetEntities<T>().Where(exp);
    }

    public IQueryable<ReturnType> WhereAndSelect<T, ReturnType>(Expression<Func<T, bool>> whereExp, Expression<Func<T, ReturnType>> selectExp) where T : BaseEntity
    {
        return GetEntities<T>().Where(whereExp).Select(selectExp);
    }

    public async Task<ReturnType?> WhereAndSelect_SingleOrDefault<T, ReturnType>(Expression<Func<T, bool>> whereExp, Expression<Func<T, ReturnType>> selectExp) where T : class
    {
        return await GetEntities<T>().Where(whereExp).Select(selectExp).SingleOrDefaultAsync();
    }

    public bool Any<T>() where T : BaseEntity
    {
        return GetEntities<T>().Any();
    }

    public async Task Update<T>(T entity) where T : class
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
}