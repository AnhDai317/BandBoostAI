using System.Collections.Concurrent;
using System.Text.Json;
using BandBoostAI.Application.Interfaces.Services;

namespace BandBoostAI.Infrastructure.Providers;

public class TranslationService : ITranslationService
{
    private static readonly ConcurrentDictionary<string, string> Cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly HttpClient _httpClient;

    public TranslationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> TranslateToVietnameseAsync(string text, CancellationToken cancellationToken = default)
    {
        if (Cache.TryGetValue(text, out var cached)) return cached;
        var url = $"get?q={Uri.EscapeDataString(text)}&langpair=en|vi";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        if (!document.RootElement.TryGetProperty("responseData", out var responseData)
            || !responseData.TryGetProperty("translatedText", out var translatedText)) return null;
        var value = translatedText.GetString();
        if (!string.IsNullOrWhiteSpace(value)) Cache[text] = value;
        return value;
    }
}
