using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.DTOs.Exam;

public class CreateQuestionDto
{
    public QuestionType Type { get; set; }
    public int QuestionNumber { get; set; }
    public required string ContentJson { get; set; } // Chuỗi JSON chứa câu hỏi & đáp án
    public string? Explanation { get; set; }
}