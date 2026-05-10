using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var existingUser = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        return existingUser == null; 
    }
}