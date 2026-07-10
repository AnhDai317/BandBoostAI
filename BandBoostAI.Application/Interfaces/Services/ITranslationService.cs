namespace BandBoostAI.Application.Interfaces.Services;

public interface ITranslationService
{
    Task<string?> TranslateToVietnameseAsync(string text, CancellationToken cancellationToken = default);
}
