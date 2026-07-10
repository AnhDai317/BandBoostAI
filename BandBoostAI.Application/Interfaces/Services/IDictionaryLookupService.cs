using BandBoostAI.Application.DTOs.Learning;

namespace BandBoostAI.Application.Interfaces.Services;

public interface IDictionaryLookupService
{
    Task<DictionaryEntryDto?> LookupAsync(string word, CancellationToken cancellationToken = default);
}
