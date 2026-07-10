using System.Collections.Concurrent;
using System.Text.Json;
using BandBoostAI.Application.DTOs.Learning;
using BandBoostAI.Application.Interfaces.Services;

namespace BandBoostAI.Infrastructure.Providers;

public class DictionaryLookupService : IDictionaryLookupService
{
    private static readonly ConcurrentDictionary<string, CacheEntry> Cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly HttpClient _httpClient;

    public DictionaryLookupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DictionaryEntryDto?> LookupAsync(string word, CancellationToken cancellationToken = default)
    {
        var normalizedWord = word.Trim().ToLowerInvariant();
        if (Cache.TryGetValue(normalizedWord, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
        {
            return cached.Value;
        }

        using var response = await _httpClient.GetAsync(
            $"api/v2/entries/en/{Uri.EscapeDataString(normalizedWord)}",
            cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() == 0)
        {
            return null;
        }

        var entry = document.RootElement[0];
        var result = new DictionaryEntryDto
        {
            Word = entry.TryGetProperty("word", out var wordElement) ? wordElement.GetString() ?? normalizedWord : normalizedWord,
            Phonetic = entry.TryGetProperty("phonetic", out var phoneticElement) ? phoneticElement.GetString() : null
        };

        if (entry.TryGetProperty("phonetics", out var phonetics) && phonetics.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in phonetics.EnumerateArray())
            {
                if (string.IsNullOrWhiteSpace(result.Phonetic) && item.TryGetProperty("text", out var text))
                {
                    result.Phonetic = text.GetString();
                }

                if (item.TryGetProperty("audio", out var audio) && !string.IsNullOrWhiteSpace(audio.GetString()))
                {
                    var audioUrl = audio.GetString()!;
                    result.AudioUrl = audioUrl.StartsWith("//") ? $"https:{audioUrl}" : audioUrl;
                    break;
                }
            }
        }

        if (entry.TryGetProperty("meanings", out var meanings) && meanings.ValueKind == JsonValueKind.Array)
        {
            foreach (var meaning in meanings.EnumerateArray())
            {
                result.PartOfSpeech = meaning.TryGetProperty("partOfSpeech", out var part) ? part.GetString() : null;
                if (!meaning.TryGetProperty("definitions", out var definitions) || definitions.GetArrayLength() == 0) continue;

                var definition = definitions[0];
                result.Definition = definition.TryGetProperty("definition", out var definitionText) ? definitionText.GetString() : null;
                result.Example = definition.TryGetProperty("example", out var exampleText) ? exampleText.GetString() : null;
                break;
            }
        }

        Cache[normalizedWord] = new CacheEntry(result, DateTime.UtcNow.AddHours(12));
        return result;
    }

    private sealed record CacheEntry(DictionaryEntryDto Value, DateTime ExpiresAt);
}
