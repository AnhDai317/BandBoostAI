using BandBoostAI.Application.DTOs.Exam;
using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Application.Interfaces.Services;
using BandBoostAI.Domain.Entities;

namespace BandBoostAI.Application.Services;

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;

    public ExamService(IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task<Guid> CreateExamAsync(CreateExamDto dto)
    {
        // Chuyển đổi cấu trúc phức tạp từ DTO sang Entities một cách thủ công (hoặc dùng AutoMapper sau này)
        var exam = new Exam
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            DurationInMinutes = dto.DurationInMinutes,
            IsPremium = dto.IsPremium,
            Sections = dto.Sections.Select(s => new ExamSection
            {
                Title = s.Title,
                SharedContent = s.SharedContent,
                OrderIndex = s.OrderIndex,
                Questions = s.Questions.Select(q => new Question
                {
                    Type = q.Type,
                    QuestionNumber = q.QuestionNumber,
                    ContentJson = q.ContentJson,
                    Explanation = q.Explanation
                }).ToList()
            }).ToList()
        };

        var result = await _examRepository.AddAsync(exam);
        return result.Id;
    }

    public async Task<Exam?> GetExamByIdAsync(Guid id)
    {
        return await _examRepository.GetExamWithDetailsAsync(id);
    }

    public async Task<IReadOnlyList<Exam>> GetAllExamsAsync()
    {
        return await _examRepository.GetAllAsync();
    }
}