using BandBoostAI.Domain.Entities;

namespace BandBoostAI.Application.Interfaces.Repositories;

public interface IExamRepository : IGenericRepository<Exam>
{
    Task<Exam?> GetExamWithDetailsAsync(Guid id);
}