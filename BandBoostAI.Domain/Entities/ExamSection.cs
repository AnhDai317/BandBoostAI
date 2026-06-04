using BandBoostAI.Domain.Common;

namespace BandBoostAI.Domain.Entities;

public class ExamSection : BaseEntity
{
    public Guid ExamId { get; set; } // Khóa ngoại trỏ về Exam
    
    public string Title { get; set; } = string.Empty; // VD: "Passage 1: The history of glass"
    
    // Nơi chứa nội dung chung: Bài đọc (HTML), Link Audio, hoặc Link Ảnh (cho Writing Task 1)
    public string? SharedContent { get; set; } 
    
    public int OrderIndex { get; set; } // Thứ tự sắp xếp (Part 1, Part 2...)

    // Mối quan hệ Navigation
    public virtual Exam? Exam { get; set; }
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
