using BandBoostAI.Application.DTOs.Learning;

namespace BandBoostAI.Application.Interfaces.Services;

public interface IWordDiscoveryService
{
    Task<IReadOnlyList<WordSuggestionDto>> FindRelatedWordsAsync(
        string query,
        int take = 50,
        CancellationToken cancellationToken = default);
}
