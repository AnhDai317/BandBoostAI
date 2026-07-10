using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.Interfaces.Repositories;

public interface ILearningRepository
{
    Task<User?> GetUserAsync(Guid userId);
    Task<LearningProfile?> GetProfileAsync(Guid userId);
    Task SaveProfileAsync(LearningProfile profile, User user);
    Task<IReadOnlyList<VocabularyTopic>> GetTopicsAsync();
    Task<IReadOnlyList<VocabularyWord>> GetWordsAsync(string? topicSlug, EnglishLevel? level, int take);
    Task<VocabularyWord?> GetWordAsync(Guid wordId);
    Task<VocabularyWord?> FindWordAsync(string word, string topicSlug);
    Task<VocabularyWord> AddWordAsync(VocabularyWord word);
    Task<IReadOnlyList<UserVocabularyProgress>> GetProgressAsync(Guid userId);
    Task<UserVocabularyProgress?> GetWordProgressAsync(Guid userId, Guid wordId);
    Task SaveReviewAsync(UserVocabularyProgress progress, LearningActivity activity);
    Task<IReadOnlyList<LearningActivity>> GetActivitiesSinceAsync(Guid userId, DateTime sinceUtc);
    Task<IReadOnlyList<ExamAttempt>> GetExamAttemptsSinceAsync(Guid userId, DateTime sinceUtc);
}
