namespace BandBoostAI.Application.DTOs.Exam;

public class CreateSectionDto
{
    public required string Title { get; set; }
    public string? SharedContent { get; set; } // Bài đọc HTML hoặc link Audio
    public int OrderIndex { get; set; }
    public List<CreateQuestionDto> Questions { get; set; } = new();
}