using BandBoostAI.Application.DTOs.Exam;
using BandBoostAI.Application.Interfaces.Services;
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
}