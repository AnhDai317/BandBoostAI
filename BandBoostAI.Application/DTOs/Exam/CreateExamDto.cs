using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.DTOs.Exam;

public class CreateExamDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public SkillCategory Category { get; set; }
    public int DurationInMinutes { get; set; }
    public bool IsPremium { get; set; }
    public List<CreateSectionDto> Sections { get; set; } = new();
}
