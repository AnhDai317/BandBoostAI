using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Repositories;

public class ExamRepository : GenericRepository<Exam>, IExamRepository
{
    public ExamRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Exam?> GetExamWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(e => e.Sections) // Lấy kèm các phần thi
                .ThenInclude(s => s.Questions) // Lấy kèm các câu hỏi trong phần đó
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}