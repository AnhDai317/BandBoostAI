using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BandBoostAI.Domain.Entities;

namespace BandBoostAI.Application.Interfaces.Repositories;

public interface IExamAttemptRepository : IGenericRepository<ExamAttempt>
{
    Task<List<ExamAttempt>> GetAttemptsByUserIdAsync(Guid userId);
    Task<ExamAttempt?> GetAttemptWithExamAsync(Guid attemptId);
}
