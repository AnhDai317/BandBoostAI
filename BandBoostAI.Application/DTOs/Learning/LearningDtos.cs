using System.ComponentModel.DataAnnotations;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.DTOs.Learning;

public class UpdateLearningProfileDto
{
    [Required]
    public EnglishLevel CurrentLevel { get; set; }

    [Required]
    public EnglishLevel TargetLevel { get; set; }

    [Required]
    public LearningGoal PrimaryGoal { get; set; }

    [Range(10, 180)]
    public int DailyMinutes { get; set; }

    [Range(1, 7)]
    public int WeeklyGoalDays { get; set; }

    public DateTime? TargetDate { get; set; }

    [Range(0, 9)]
    public decimal TargetBandScore { get; set; }

    public List<string> PreferredTopics { get; set; } = [];
}

public class LearningProfileDto
{
    public EnglishLevel CurrentLevel { get; set; }
    public EnglishLevel TargetLevel { get; set; }
    public LearningGoal PrimaryGoal { get; set; }
    public int DailyMinutes { get; set; }
    public int WeeklyGoalDays { get; set; }
    public DateTime? TargetDate { get; set; }
    public decimal TargetBandScore { get; set; }
    public decimal? CurrentBandScore { get; set; }
    public List<string> PreferredTopics { get; set; } = [];
    public bool IsOnboardingCompleted { get; set; }
}

public class LearningDashboardDto
{
    public bool RequiresOnboarding { get; set; }
    public required LearningProfileDto Profile { get; set; }
    public required LearningStatsDto Stats { get; set; }
    public List<DailyTaskDto> TodayPlan { get; set; } = [];
    public List<LearningMilestoneDto> Milestones { get; set; } = [];
    public List<TopicSummaryDto> RecommendedTopics { get; set; } = [];
}

public class LearningStatsDto
{
    public int CurrentStreak { get; set; }
    public int TodayMinutes { get; set; }
    public int DailyMinutesTarget { get; set; }
    public int ActiveDaysThisWeek { get; set; }
    public int WeeklyGoalDays { get; set; }
    public int WordsStarted { get; set; }
    public int WordsMastered { get; set; }
    public int ExperiencePoints { get; set; }
}

public class DailyTaskDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public required string Route { get; set; }
    public int EstimatedMinutes { get; set; }
    public bool IsCompleted { get; set; }
}

public class LearningMilestoneDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int ProgressPercent { get; set; }
    public required string Status { get; set; }
}

public class TopicSummaryDto
{
    public Guid Id { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public int WordCount { get; set; }
    public int StartedCount { get; set; }
    public int MasteredCount { get; set; }
}

public class VocabularyWordDto
{
    public Guid Id { get; set; }
    public required string Word { get; set; }
    public required string Pronunciation { get; set; }
    public required string PartOfSpeech { get; set; }
    public required string Meaning { get; set; }
    public required string MeaningVietnamese { get; set; }
    public required string ExampleSentence { get; set; }
    public required string ExampleTranslation { get; set; }
    public required string TopicSlug { get; set; }
    public required string TopicName { get; set; }
    public EnglishLevel Level { get; set; }
    public int MasteryLevel { get; set; }
    public bool HasStarted { get; set; }
    public bool IsMastered { get; set; }
    public DateTime? NextReviewAt { get; set; }
}

public class VocabularyReviewDto
{
    public bool IsCorrect { get; set; }
}

public class VocabularyReviewResultDto
{
    public int MasteryLevel { get; set; }
    public bool IsMastered { get; set; }
    public DateTime NextReviewAt { get; set; }
    public int ExperiencePointsEarned { get; set; }
}

public class DictionaryEntryDto
{
    public required string Word { get; set; }
    public string? Phonetic { get; set; }
    public string? AudioUrl { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Definition { get; set; }
    public string? Example { get; set; }
    public string? MeaningVietnamese { get; set; }
}

public class WordSuggestionDto
{
    public required string Word { get; set; }
    public int Score { get; set; }
    public string? PartOfSpeech { get; set; }
}

public class AddDiscoveredVocabularyDto
{
    public required string Word { get; set; }
    public required string TopicSlug { get; set; }
    public EnglishLevel Level { get; set; }
    public string? Pronunciation { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Meaning { get; set; }
    public required string MeaningVietnamese { get; set; }
    public string? ExampleSentence { get; set; }
}
