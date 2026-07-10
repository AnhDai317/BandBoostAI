using BandBoostAI.Application.DTOs.Learning;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.Interfaces.Services;

public interface ILearningService
{
    Task<LearningDashboardDto> GetDashboardAsync(Guid userId);
    Task<LearningProfileDto> GetProfileAsync(Guid userId);
    Task<LearningProfileDto> UpdateProfileAsync(Guid userId, UpdateLearningProfileDto dto);
    Task<IReadOnlyList<TopicSummaryDto>> GetTopicsAsync(Guid userId);
    Task<IReadOnlyList<VocabularyWordDto>> GetVocabularyAsync(Guid userId, string? topic, EnglishLevel? level, int take);
    Task<VocabularyReviewResultDto> ReviewWordAsync(Guid userId, Guid wordId, VocabularyReviewDto dto);
    Task<VocabularyWordDto> AddDiscoveredWordAsync(Guid userId, AddDiscoveredVocabularyDto dto);
}
