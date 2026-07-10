using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.DTOs.Exam;

public class ExamResponseDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public SkillCategory Category { get; set; }
    public int DurationInMinutes { get; set; }
    public bool IsPremium { get; set; }
    public List<ExamSectionResponseDto> Sections { get; set; } = [];
}

public class ExamSectionResponseDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public required string Title { get; set; }
    public string? SharedContent { get; set; }
    public int OrderIndex { get; set; }
    public List<QuestionResponseDto> Questions { get; set; } = [];
}

public class QuestionResponseDto
{
    public Guid Id { get; set; }
    public Guid ExamSectionId { get; set; }
    public QuestionType Type { get; set; }
    public int QuestionNumber { get; set; }
    public required string ContentJson { get; set; }
    public string? Explanation { get; set; }
}
