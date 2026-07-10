using System;
using BandBoostAI.Domain.Common;

namespace BandBoostAI.Domain.Entities;

public class ExamAttempt : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    
    public required string AnswersJson { get; set; }
    
    public decimal Score { get; set; }
    public bool IsCompleted { get; set; }
    public string? Feedback { get; set; }
    
    public virtual User? User { get; set; }
    public virtual Exam? Exam { get; set; }
}
