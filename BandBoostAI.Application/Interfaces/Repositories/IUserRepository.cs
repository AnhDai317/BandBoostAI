using BandBoostAI.Domain.Entities;

namespace BandBoostAI.Application.Interfaces.Repositories;

// Kế thừa IGenericRepository để có sẵn Thêm/Sửa/Xóa, chỉ viết thêm hàm đặc thù
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}