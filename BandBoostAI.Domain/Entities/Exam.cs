using BandBoostAI.Domain.Common;

namespace BandBoostAI.Domain.Entities;

public class Exam : BaseEntity
{
    public required string Title { get; set; }
    public int DurationInMinutes { get; set; }
    
    // Navigation Property: 1 Đề thi có nhiều Câu hỏi
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}