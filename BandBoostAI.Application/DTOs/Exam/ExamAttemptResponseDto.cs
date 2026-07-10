using System;

namespace BandBoostAI.Application.DTOs.Exam;

public class ExamAttemptResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public string AnswersJson { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public bool IsCompleted { get; set; }
    public string? Feedback { get; set; }
    public DateTime CreatedAt { get; set; }
}
