using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using BandBoostAI.Application.DTOs.Exam;
using BandBoostAI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BandBoostAI.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamDto dto)
    {
        var examId = await _examService.CreateExamAsync(dto);
        return Ok(new { message = "Tạo đề thi thành công!", id = examId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var exam = await _examService.GetExamByIdAsync(id);
        if (exam == null) return NotFound(new { message = "Không tìm thấy đề thi này." });
        return Ok(exam);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var exams = await _examService.GetAllExamsAsync();
        return Ok(exams);
    }

    [Authorize]
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] SubmitExamDto dto)
    {
        var userIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện tính năng này." });
        }

        try
        {
            var attempt = await _examService.SubmitExamAsync(id, userId, dto);
            return Ok(attempt);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("my-attempts")]
    public async Task<IActionResult> GetMyAttempts()
    {
        var userIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện tính năng này." });
        }

        var attempts = await _examService.GetUserAttemptsAsync(userId);
        return Ok(attempts);
    }

    [Authorize]
    [HttpGet("attempts/{attemptId}")]
    public async Task<IActionResult> GetAttempt(Guid attemptId)
    {
        var attempt = await _examService.GetAttemptByIdAsync(attemptId);
        if (attempt == null)
        {
            return NotFound(new { message = "Không tìm thấy kết quả làm bài này." });
        }

        return Ok(attempt);
    }
}
