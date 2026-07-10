using BandBoostAI.Application.DTOs.Learning;
using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Application.Interfaces.Services;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.Services;

public class LearningService : ILearningService
{
    private readonly ILearningRepository _repository;

    public LearningService(ILearningRepository repository)
    {
        _repository = repository;
    }

    public async Task<LearningProfileDto> GetProfileAsync(Guid userId)
    {
        var user = await GetUserOrThrowAsync(userId);
        var profile = await _repository.GetProfileAsync(userId);
        return MapProfile(profile, user);
    }

    public async Task<LearningProfileDto> UpdateProfileAsync(Guid userId, UpdateLearningProfileDto dto)
    {
        var user = await GetUserOrThrowAsync(userId);
        var availableTopics = await _repository.GetTopicsAsync();
        var validTopicSlugs = availableTopics.Select(topic => topic.Slug).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var preferredTopics = dto.PreferredTopics
            .Where(slug => validTopicSlugs.Contains(slug))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(5)
            .Select(slug => slug.ToLowerInvariant())
            .ToList();

        if (dto.TargetDate.HasValue && dto.TargetDate.Value.Date <= DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Ngày hoàn thành mục tiêu phải ở trong tương lai.");
        }

        if (dto.PrimaryGoal == LearningGoal.Ielts && (dto.TargetBandScore < 4 || dto.TargetBandScore > 9))
        {
            throw new ArgumentException("Mục tiêu IELTS cần nằm trong khoảng band 4.0 đến 9.0.");
        }

        var profile = await _repository.GetProfileAsync(userId) ?? new LearningProfile { UserId = userId };
        profile.CurrentLevel = dto.CurrentLevel;
        profile.TargetLevel = dto.TargetLevel;
        profile.PrimaryGoal = dto.PrimaryGoal;
        profile.DailyMinutes = Math.Clamp(dto.DailyMinutes, 10, 180);
        profile.WeeklyGoalDays = Math.Clamp(dto.WeeklyGoalDays, 1, 7);
        profile.TargetDate = dto.TargetDate?.Date;
        profile.PreferredTopics = string.Join(',', preferredTopics);
        profile.IsOnboardingCompleted = true;

        user.TargetBandScore = dto.PrimaryGoal == LearningGoal.Ielts ? dto.TargetBandScore : 0;
        await _repository.SaveProfileAsync(profile, user);
        return MapProfile(profile, user);
    }

    public async Task<IReadOnlyList<TopicSummaryDto>> GetTopicsAsync(Guid userId)
    {
        await GetUserOrThrowAsync(userId);
        var topics = await _repository.GetTopicsAsync();
        var progress = await _repository.GetProgressAsync(userId);
        var progressByWord = progress.ToDictionary(item => item.VocabularyWordId);

        return topics.Select(topic => new TopicSummaryDto
        {
            Id = topic.Id,
            Slug = topic.Slug,
            Name = topic.Name,
            Description = topic.Description,
            Icon = topic.Icon,
            WordCount = topic.Words.Count,
            StartedCount = topic.Words.Count(word => progressByWord.ContainsKey(word.Id)),
            MasteredCount = topic.Words.Count(word =>
                progressByWord.TryGetValue(word.Id, out var item) && item.IsMastered)
        }).ToList();
    }

    public async Task<IReadOnlyList<VocabularyWordDto>> GetVocabularyAsync(
        Guid userId,
        string? topic,
        EnglishLevel? level,
        int take)
    {
        var profile = await _repository.GetProfileAsync(userId);
        await GetUserOrThrowAsync(userId);
        var selectedLevel = level ?? profile?.CurrentLevel;
        var words = await _repository.GetWordsAsync(topic, selectedLevel, Math.Clamp(take, 1, 50));
        var progress = await _repository.GetProgressAsync(userId);
        var progressByWord = progress.ToDictionary(item => item.VocabularyWordId);

        return words
            .OrderBy(word => progressByWord.TryGetValue(word.Id, out var item) && item.IsMastered)
            .ThenBy(word => progressByWord.TryGetValue(word.Id, out var item) ? item.NextReviewAt : null)
            .Select(word => MapWord(word, progressByWord.GetValueOrDefault(word.Id)))
            .ToList();
    }

    public async Task<VocabularyReviewResultDto> ReviewWordAsync(
        Guid userId,
        Guid wordId,
        VocabularyReviewDto dto)
    {
        await GetUserOrThrowAsync(userId);
        var word = await _repository.GetWordAsync(wordId)
            ?? throw new KeyNotFoundException("Không tìm thấy từ vựng này.");
        var progress = await _repository.GetWordProgressAsync(userId, wordId) ?? new UserVocabularyProgress
        {
            UserId = userId,
            VocabularyWordId = word.Id
        };

        if (dto.IsCorrect)
        {
            progress.CorrectAnswers++;
            progress.MasteryLevel = Math.Min(5, progress.MasteryLevel + 1);
        }
        else
        {
            progress.IncorrectAnswers++;
            progress.MasteryLevel = Math.Max(0, progress.MasteryLevel - 1);
        }

        var reviewIntervals = new[] { 0, 1, 3, 7, 14, 30 };
        progress.LastReviewedAt = DateTime.UtcNow;
        progress.NextReviewAt = DateTime.UtcNow.AddDays(reviewIntervals[progress.MasteryLevel]);
        progress.IsMastered = progress.MasteryLevel >= 5;
        var points = dto.IsCorrect ? 10 : 3;

        await _repository.SaveReviewAsync(progress, new LearningActivity
        {
            UserId = userId,
            Type = LearningActivityType.Vocabulary,
            ItemsCompleted = 1,
            MinutesSpent = 1,
            ExperiencePoints = points
        });

        return new VocabularyReviewResultDto
        {
            MasteryLevel = progress.MasteryLevel,
            IsMastered = progress.IsMastered,
            NextReviewAt = progress.NextReviewAt.Value,
            ExperiencePointsEarned = points
        };
    }

    public async Task<VocabularyWordDto> AddDiscoveredWordAsync(Guid userId, AddDiscoveredVocabularyDto dto)
    {
        await GetUserOrThrowAsync(userId);
        var normalizedWord = dto.Word.Trim().ToLowerInvariant();
        var existing = await _repository.FindWordAsync(normalizedWord, dto.TopicSlug);
        if (existing is not null)
        {
            var existingProgress = await _repository.GetWordProgressAsync(userId, existing.Id);
            return MapWord(existing, existingProgress);
        }

        var topic = (await _repository.GetTopicsAsync())
            .FirstOrDefault(item => item.Slug.Equals(dto.TopicSlug, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Chủ đề không hợp lệ.");
        var word = new VocabularyWord
        {
            TopicId = topic.Id,
            Topic = topic,
            Level = dto.Level,
            Word = normalizedWord,
            Pronunciation = dto.Pronunciation?.Trim() ?? string.Empty,
            PartOfSpeech = dto.PartOfSpeech?.Trim() ?? string.Empty,
            Meaning = dto.Meaning?.Trim() ?? string.Empty,
            MeaningVietnamese = dto.MeaningVietnamese.Trim(),
            ExampleSentence = dto.ExampleSentence?.Trim() ?? string.Empty,
            ExampleTranslation = string.Empty
        };
        await _repository.AddWordAsync(word);
        return MapWord(word, null);
    }

    public async Task<LearningDashboardDto> GetDashboardAsync(Guid userId)
    {
        var user = await GetUserOrThrowAsync(userId);
        var profile = await _repository.GetProfileAsync(userId);
        var profileDto = MapProfile(profile, user);
        var progress = await _repository.GetProgressAsync(userId);
        var activities = await _repository.GetActivitiesSinceAsync(userId, DateTime.UtcNow.Date.AddDays(-60));
        var examAttempts = await _repository.GetExamAttemptsSinceAsync(userId, DateTime.UtcNow.Date.AddDays(-60));
        var topics = await GetTopicsAsync(userId);
        var today = DateTime.UtcNow.Date;
        var startOfWeek = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        var todayActivities = activities.Where(activity => activity.CreatedAt.Date == today).ToList();
        var activeDates = activities.Select(activity => activity.CreatedAt.Date)
            .Concat(examAttempts.Select(attempt => attempt.CreatedAt.Date))
            .Distinct()
            .ToHashSet();
        var streak = CalculateStreak(activeDates, today);
        var vocabularyDoneToday = todayActivities
            .Where(activity => activity.Type == LearningActivityType.Vocabulary)
            .Sum(activity => activity.ItemsCompleted);
        var dailyMinutes = profileDto.DailyMinutes;
        var preferred = profileDto.PreferredTopics.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var recommendedTopics = topics
            .OrderByDescending(topic => preferred.Contains(topic.Slug))
            .ThenBy(topic => topic.StartedCount)
            .Take(3)
            .ToList();

        var vocabularyTarget = Math.Max(5, Math.Min(15, dailyMinutes / 2));
        var examsDoneToday = examAttempts.Where(attempt => attempt.CreatedAt.Date == today).ToList();
        var todayMinutes = todayActivities.Sum(activity => activity.MinutesSpent)
            + examsDoneToday.Sum(attempt => attempt.Exam?.DurationInMinutes ?? 15);
        var activeDaysThisWeek = activeDates.Count(date => date >= startOfWeek && date <= today);
        var wordsMastered = progress.Count(item => item.IsMastered);
        var wordsStarted = progress.Count;
        var readiness = user.TargetBandScore > 0 && user.CurrentBandScore.HasValue
            ? (int)Math.Round(Math.Min(100, user.CurrentBandScore.Value / user.TargetBandScore * 100))
            : Math.Min(100, wordsStarted * 2);

        return new LearningDashboardDto
        {
            RequiresOnboarding = profile is null || !profile.IsOnboardingCompleted,
            Profile = profileDto,
            Stats = new LearningStatsDto
            {
                CurrentStreak = streak,
                TodayMinutes = todayMinutes,
                DailyMinutesTarget = dailyMinutes,
                ActiveDaysThisWeek = activeDaysThisWeek,
                WeeklyGoalDays = profileDto.WeeklyGoalDays,
                WordsStarted = wordsStarted,
                WordsMastered = wordsMastered,
                ExperiencePoints = activities.Sum(activity => activity.ExperiencePoints) + examAttempts.Count * 50
            },
            TodayPlan =
            [
                new DailyTaskDto
                {
                    Title = $"Học {vocabularyTarget} từ vựng",
                    Description = recommendedTopics.Count > 0
                        ? $"Chủ đề {recommendedTopics[0].Name} · Trình độ {profileDto.CurrentLevel}"
                        : $"Từ vựng phù hợp trình độ {profileDto.CurrentLevel}",
                    Icon = "style",
                    Route = recommendedTopics.Count > 0
                        ? $"/vocabulary?topic={recommendedTopics[0].Slug}"
                        : "/vocabulary",
                    EstimatedMinutes = vocabularyTarget,
                    IsCompleted = vocabularyDoneToday >= vocabularyTarget
                },
                new DailyTaskDto
                {
                    Title = "Luyện một kỹ năng trọng tâm",
                    Description = "Hoàn thành một bài luyện và nhận phản hồi tức thì",
                    Icon = "psychology",
                    Route = "/practice",
                    EstimatedMinutes = Math.Max(10, dailyMinutes - vocabularyTarget),
                    IsCompleted = examsDoneToday.Count > 0
                }
            ],
            Milestones =
            [
                new LearningMilestoneDto
                {
                    Title = "Xây nền từ vựng",
                    Description = $"Đã học {wordsStarted}/50 từ đầu tiên · {wordsMastered} từ thành thạo",
                    ProgressPercent = Math.Min(100, wordsStarted * 2),
                    Status = wordsStarted >= 50 ? "completed" : "in_progress"
                },
                new LearningMilestoneDto
                {
                    Title = "Tạo thói quen học đều",
                    Description = $"Học {activeDaysThisWeek}/{profileDto.WeeklyGoalDays} ngày trong tuần này",
                    ProgressPercent = Math.Min(100, activeDaysThisWeek * 100 / Math.Max(1, profileDto.WeeklyGoalDays)),
                    Status = activeDaysThisWeek >= profileDto.WeeklyGoalDays ? "completed" : "in_progress"
                },
                new LearningMilestoneDto
                {
                    Title = profileDto.PrimaryGoal == LearningGoal.Ielts
                        ? $"Chinh phục IELTS {profileDto.TargetBandScore:0.0}"
                        : $"Tiến tới trình độ {profileDto.TargetLevel}",
                    Description = profileDto.TargetDate.HasValue
                        ? $"Mốc dự kiến {profileDto.TargetDate:dd/MM/yyyy}"
                        : "Thiết lập ngày đích để duy trì động lực",
                    ProgressPercent = readiness,
                    Status = readiness >= 100 ? "completed" : "upcoming"
                }
            ],
            RecommendedTopics = recommendedTopics
        };
    }

    private async Task<User> GetUserOrThrowAsync(Guid userId) =>
        await _repository.GetUserAsync(userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

    private static LearningProfileDto MapProfile(LearningProfile? profile, User user) => new()
    {
        CurrentLevel = profile?.CurrentLevel ?? EnglishLevel.A1,
        TargetLevel = profile?.TargetLevel ?? EnglishLevel.B2,
        PrimaryGoal = profile?.PrimaryGoal ?? LearningGoal.GeneralEnglish,
        DailyMinutes = profile?.DailyMinutes ?? 20,
        WeeklyGoalDays = profile?.WeeklyGoalDays ?? 5,
        TargetDate = profile?.TargetDate,
        TargetBandScore = user.TargetBandScore,
        CurrentBandScore = user.CurrentBandScore,
        PreferredTopics = ParseTopics(profile?.PreferredTopics),
        IsOnboardingCompleted = profile?.IsOnboardingCompleted ?? false
    };

    private static VocabularyWordDto MapWord(VocabularyWord word, UserVocabularyProgress? progress) => new()
    {
        Id = word.Id,
        Word = word.Word,
        Pronunciation = word.Pronunciation,
        PartOfSpeech = word.PartOfSpeech,
        Meaning = word.Meaning,
        MeaningVietnamese = word.MeaningVietnamese,
        ExampleSentence = word.ExampleSentence,
        ExampleTranslation = word.ExampleTranslation,
        TopicSlug = word.Topic?.Slug ?? string.Empty,
        TopicName = word.Topic?.Name ?? string.Empty,
        Level = word.Level,
        MasteryLevel = progress?.MasteryLevel ?? 0,
        HasStarted = progress is not null,
        IsMastered = progress?.IsMastered ?? false,
        NextReviewAt = progress?.NextReviewAt
    };

    private static List<string> ParseTopics(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    private static int CalculateStreak(HashSet<DateTime> activeDates, DateTime today)
    {
        var cursor = activeDates.Contains(today) ? today : today.AddDays(-1);
        var streak = 0;
        while (activeDates.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }
}
