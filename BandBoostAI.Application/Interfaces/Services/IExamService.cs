using BandBoostAI.Application.DTOs.Exam;

namespace BandBoostAI.Application.Interfaces.Services;

public interface IExamService
{
    Task<Guid> CreateExamAsync(CreateExamDto dto);
    Task<ExamResponseDto?> GetExamByIdAsync(Guid id);
    Task<IReadOnlyList<ExamResponseDto>> GetAllExamsAsync();
    Task<ExamAttemptResponseDto> SubmitExamAsync(Guid examId, Guid userId, SubmitExamDto dto);
    Task<List<ExamAttemptResponseDto>> GetUserAttemptsAsync(Guid userId);
    Task<ExamAttemptResponseDto?> GetAttemptByIdAsync(Guid attemptId);
}
