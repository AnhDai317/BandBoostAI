using BandBoostAI.Application.DTOs.Exam;
using BandBoostAI.Domain.Entities;

namespace BandBoostAI.Application.Interfaces.Services;

public interface IExamService
{
    Task<Guid> CreateExamAsync(CreateExamDto dto);
    Task<Exam?> GetExamByIdAsync(Guid id);
    Task<IReadOnlyList<Exam>> GetAllExamsAsync();
    Task<ExamAttemptResponseDto> SubmitExamAsync(Guid examId, Guid userId, SubmitExamDto dto);
    Task<List<ExamAttemptResponseDto>> GetUserAttemptsAsync(Guid userId);
    Task<ExamAttemptResponseDto?> GetAttemptByIdAsync(Guid attemptId);
}