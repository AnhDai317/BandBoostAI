using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Domain.Entities;

public class LearningProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public EnglishLevel CurrentLevel { get; set; } = EnglishLevel.A1;
    public EnglishLevel TargetLevel { get; set; } = EnglishLevel.B2;
    public LearningGoal PrimaryGoal { get; set; } = LearningGoal.GeneralEnglish;
    public int DailyMinutes { get; set; } = 20;
    public int WeeklyGoalDays { get; set; } = 5;
    public DateTime? TargetDate { get; set; }
    public string PreferredTopics { get; set; } = string.Empty;
    public bool IsOnboardingCompleted { get; set; }

    public User? User { get; set; }
}
