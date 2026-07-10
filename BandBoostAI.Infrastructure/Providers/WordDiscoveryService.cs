using System.Text.Json;
using BandBoostAI.Application.DTOs.Learning;
using BandBoostAI.Application.Interfaces.Services;

namespace BandBoostAI.Infrastructure.Providers;

public class WordDiscoveryService : IWordDiscoveryService
{
    private readonly HttpClient _httpClient;

    public WordDiscoveryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WordSuggestionDto>> FindRelatedWordsAsync(
        string query,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var url = $"words?ml={Uri.EscapeDataString(query.Trim())}&md=p&max={Math.Clamp(take, 1, 100)}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var results = new List<WordSuggestionDto>();

        foreach (var item in document.RootElement.EnumerateArray())
        {
            if (!item.TryGetProperty("word", out var wordElement)) continue;
            var word = wordElement.GetString();
            if (string.IsNullOrWhiteSpace(word)) continue;
            if (word.Any(character => !char.IsLetter(character) && character is not '-' and not '\'')) continue;
            string? partOfSpeech = null;
            if (item.TryGetProperty("tags", out var tags) && tags.ValueKind == JsonValueKind.Array)
            {
                partOfSpeech = tags.EnumerateArray()
                    .Select(tag => tag.GetString())
                    .FirstOrDefault(tag => tag is "n" or "v" or "adj" or "adv");
            }

            results.Add(new WordSuggestionDto
            {
                Word = word,
                Score = item.TryGetProperty("score", out var score) ? score.GetInt32() : 0,
                PartOfSpeech = partOfSpeech
            });
        }

        return results;
    }
}
