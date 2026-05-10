using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Domain.Entities;

public class Question : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam? Exam { get; set; } // Navigation property

    public QuestionType QuestionType { get; set; }
    public SkillCategory SkillCategory { get; set; }

    public required string Content { get; set; } // Nội dung câu hỏi hoặc bài đọc
    public string? ResourceUrl { get; set; } // Link file Audio trên Cloud (nếu là Listening)
}