using BandBoostAI.Domain.Common;

namespace BandBoostAI.Domain.Entities;

public class AiEvaluation : BaseEntity
{
    public Guid StudentResponseId { get; set; }

    // Điểm cho từng tiêu chí của IELTS Writing
    public decimal TaskResponseScore { get; set; }
    public decimal CoherenceScore { get; set; }
    public decimal LexicalScore { get; set; }
    public decimal GrammarScore { get; set; }
    public decimal OverallBand { get; set; }

    // Cột cực kỳ quan trọng: Lưu trữ toàn bộ mảng JSON chứa các lỗi sai và gợi ý sửa từ AI
    public required string FeedbackJson { get; set; }
}