using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;
using BandBoostAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Repositories;

public class LearningRepository : ILearningRepository
{
    private readonly ApplicationDbContext _context;

    public LearningRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetUserAsync(Guid userId) =>
        _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

    public Task<LearningProfile?> GetProfileAsync(Guid userId) =>
        _context.LearningProfiles.FirstOrDefaultAsync(profile => profile.UserId == userId);

    public async Task SaveProfileAsync(LearningProfile profile, User user)
    {
        if (_context.Entry(profile).State == EntityState.Detached)
        {
            await _context.LearningProfiles.AddAsync(profile);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<VocabularyTopic>> GetTopicsAsync() =>
        await _context.VocabularyTopics
            .Include(topic => topic.Words)
            .OrderBy(topic => topic.SortOrder)
            .ToListAsync();

    public async Task<IReadOnlyList<VocabularyWord>> GetWordsAsync(
        string? topicSlug,
        EnglishLevel? level,
        int take)
    {
        var query = _context.VocabularyWords
            .Include(word => word.Topic)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(topicSlug))
        {
            query = query.Where(word => word.Topic != null && word.Topic.Slug == topicSlug);
        }

        if (level.HasValue)
        {
            query = query.Where(word => word.Level == level.Value);
        }

        return await query
            .OrderBy(word => word.Level)
            .ThenBy(word => word.Word)
            .Take(Math.Clamp(take, 1, 100))
            .ToListAsync();
    }

    public Task<VocabularyWord?> GetWordAsync(Guid wordId) =>
        _context.VocabularyWords
            .Include(word => word.Topic)
            .FirstOrDefaultAsync(word => word.Id == wordId);

    public Task<VocabularyWord?> FindWordAsync(string word, string topicSlug) =>
        _context.VocabularyWords
            .Include(item => item.Topic)
            .FirstOrDefaultAsync(item => item.Word == word && item.Topic != null && item.Topic.Slug == topicSlug);

    public async Task<VocabularyWord> AddWordAsync(VocabularyWord word)
    {
        await _context.VocabularyWords.AddAsync(word);
        await _context.SaveChangesAsync();
        return word;
    }

    public async Task<IReadOnlyList<UserVocabularyProgress>> GetProgressAsync(Guid userId) =>
        await _context.UserVocabularyProgress
            .Where(progress => progress.UserId == userId)
            .ToListAsync();

    public Task<UserVocabularyProgress?> GetWordProgressAsync(Guid userId, Guid wordId) =>
        _context.UserVocabularyProgress.FirstOrDefaultAsync(progress =>
            progress.UserId == userId && progress.VocabularyWordId == wordId);

    public async Task SaveReviewAsync(UserVocabularyProgress progress, LearningActivity activity)
    {
        if (_context.Entry(progress).State == EntityState.Detached)
        {
            await _context.UserVocabularyProgress.AddAsync(progress);
        }

        await _context.LearningActivities.AddAsync(activity);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<LearningActivity>> GetActivitiesSinceAsync(Guid userId, DateTime sinceUtc) =>
        await _context.LearningActivities
            .Where(activity => activity.UserId == userId && activity.CreatedAt >= sinceUtc)
            .OrderByDescending(activity => activity.CreatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<ExamAttempt>> GetExamAttemptsSinceAsync(Guid userId, DateTime sinceUtc) =>
        await _context.ExamAttempts
            .Include(attempt => attempt.Exam)
            .Where(attempt => attempt.UserId == userId && attempt.CreatedAt >= sinceUtc && attempt.IsCompleted)
            .OrderByDescending(attempt => attempt.CreatedAt)
            .ToListAsync();
}
