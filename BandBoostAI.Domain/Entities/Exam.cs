using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Domain.Entities;

public class Exam : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public SkillCategory Category { get; set; } 
    public int DurationInMinutes { get; set; } 
    public bool IsPremium { get; set; } 

    public virtual ICollection<ExamSection> Sections { get; set; } = new List<ExamSection>();

}