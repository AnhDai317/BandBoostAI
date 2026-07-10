using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Repositories;

public class ExamAttemptRepository : GenericRepository<ExamAttempt>, IExamAttemptRepository
{
    public ExamAttemptRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<ExamAttempt>> GetAttemptsByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(a => a.Exam)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<ExamAttempt?> GetAttemptWithExamAsync(Guid attemptId)
    {
        return await _dbSet
            .Include(a => a.Exam)
            .FirstOrDefaultAsync(a => a.Id == attemptId);
    }
}
