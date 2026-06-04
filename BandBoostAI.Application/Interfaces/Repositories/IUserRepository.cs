using BandBoostAI.Domain.Entities;

namespace BandBoostAI.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}