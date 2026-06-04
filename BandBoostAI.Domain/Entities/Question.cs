using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Domain.Entities;

public class Question : BaseEntity
{
    public Guid ExamSectionId { get; set; } // Thuộc phần thi nào
    
    public QuestionType Type { get; set; }
    public int QuestionNumber { get; set; } // Câu số mấy trong đề (VD: Câu 1, Câu 15)
    
    // ĐÂY LÀ "VŨ KHÍ BÍ MẬT": ContentJson
    // Thay vì tạo các cột OptionA, OptionB, CorrectAnswer cứng ngắc, 
    // ta lưu toàn bộ nội dung câu hỏi dưới dạng chuỗi JSON.
    // VD Multiple Choice: { "text": "What is...", "options": ["A", "B"], "correct": 0 }
    // VD Fill in blank: { "text": "The glass was invented in [___]", "answers": ["1990", "1990s"] }
    public required string ContentJson { get; set; }
    
    public string? Explanation { get; set; } // Lời giải thích khi xem đáp án chi tiết

    public virtual ExamSection? ExamSection { get; set; }
}