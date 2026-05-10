using BandBoostAI.Domain.Common;

namespace BandBoostAI.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    // Lấy dữ liệu
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    
    // Thao tác dữ liệu
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity); // Hàm này thực chất sẽ gọi Update để kích hoạt Soft Delete
}