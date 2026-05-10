using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Domain.Common;
using BandBoostAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        // Khi gọi Remove, Entity Framework sẽ tự động chuyển thành Update IsDeleted = true
        // (nhờ logic chúng ta đã viết trong file ApplicationDbContext)
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}