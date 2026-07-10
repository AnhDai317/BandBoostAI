using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BandBoostAI.Application.DTOs.Learning;
using BandBoostAI.Application.Interfaces.Services;
using BandBoostAI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BandBoostAI.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LearningController : ControllerBase
{
    private readonly ILearningService _learningService;
    private readonly IDictionaryLookupService _dictionaryLookupService;
    private readonly IWordDiscoveryService _wordDiscoveryService;
    private readonly ITranslationService _translationService;

    public LearningController(
        ILearningService learningService,
        IDictionaryLookupService dictionaryLookupService,
        IWordDiscoveryService wordDiscoveryService,
        ITranslationService translationService)
    {
        _learningService = learningService;
        _dictionaryLookupService = dictionaryLookupService;
        _wordDiscoveryService = wordDiscoveryService;
        _translationService = translationService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _learningService.GetDashboardAsync(userId));
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _learningService.GetProfileAsync(userId));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateLearningProfileDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            return Ok(await _learningService.UpdateProfileAsync(userId, dto));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("topics")]
    public async Task<IActionResult> GetTopics()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _learningService.GetTopicsAsync(userId));
    }

    [HttpGet("vocabulary")]
    public async Task<IActionResult> GetVocabulary(
        [FromQuery] string? topic,
        [FromQuery] EnglishLevel? level,
        [FromQuery] int take = 20)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _learningService.GetVocabularyAsync(userId, topic, level, take));
    }

    [HttpPost("vocabulary/{wordId:guid}/review")]
    public async Task<IActionResult> ReviewVocabulary(Guid wordId, [FromBody] VocabularyReviewDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            return Ok(await _learningService.ReviewWordAsync(userId, wordId, dto));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpGet("dictionary/{word}")]
    public async Task<IActionResult> LookupDictionary(string word, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(word) || word.Length > 50 || word.Any(character => !char.IsLetter(character) && character is not '-' and not '\''))
        {
            return BadRequest(new { message = "Từ cần tra cứu không hợp lệ." });
        }

        try
        {
            var entry = await _dictionaryLookupService.LookupAsync(word, cancellationToken);
            if (entry is not null)
            {
                entry.MeaningVietnamese = await _translationService.TranslateToVietnameseAsync(word, cancellationToken);
            }
            return entry is null ? NotFound(new { message = "Không tìm thấy từ trong từ điển." }) : Ok(entry);
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new { message = "Dịch vụ từ điển đang tạm thời không khả dụng." });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, new { message = "Dịch vụ từ điển phản hồi quá chậm." });
        }
    }

    [HttpPost("vocabulary/discovered")]
    public async Task<IActionResult> AddDiscoveredWord([FromBody] AddDiscoveredVocabularyDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(dto.Word) || string.IsNullOrWhiteSpace(dto.MeaningVietnamese))
        {
            return BadRequest(new { message = "Từ và nghĩa tiếng Việt không được để trống." });
        }

        try
        {
            return Ok(await _learningService.AddDiscoveredWordAsync(userId, dto));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("discover")]
    public async Task<IActionResult> DiscoverWords(
        [FromQuery] string query,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length > 80)
        {
            return BadRequest(new { message = "Hãy nhập một chủ đề cần khám phá." });
        }

        try
        {
            return Ok(await _wordDiscoveryService.FindRelatedWordsAsync(query, take, cancellationToken));
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new { message = "Kho từ mở rộng đang tạm thời không khả dụng." });
        }
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out userId);
    }
}
