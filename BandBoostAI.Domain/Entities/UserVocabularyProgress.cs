using BandBoostAI.Domain.Common;

namespace BandBoostAI.Domain.Entities;

public class UserVocabularyProgress : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid VocabularyWordId { get; set; }
    public int MasteryLevel { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }
    public DateTime? LastReviewedAt { get; set; }
    public DateTime? NextReviewAt { get; set; }
    public bool IsMastered { get; set; }

    public User? User { get; set; }
    public VocabularyWord? VocabularyWord { get; set; }
}
