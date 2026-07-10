using BandBoostAI.Application.DTOs.Auth;
using BandBoostAI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace BandBoostAI.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController] // Khai báo này giúp .NET tự động bắt lỗi [Required] từ DTO mà không cần if(!ModelState.IsValid)
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    // Dependency Injection: Nhận "ổ cắm" AuthService từ hệ thống
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            // Quăng data cho Service xử lý
            var resultMessage = await _authService.RegisterAsync(request);

            // Kiểm tra kết quả trả về
            if (resultMessage == "Đăng ký tài khoản thành công!")
            {
                // Trả về mã HTTP 200 (Thành công) cùng JSON
                return Ok(new { message = resultMessage });
            }

            // Trả về mã HTTP 400 (Lỗi từ phía người dùng - VD: Trùng email)
            return BadRequest(new { message = resultMessage });
        }
        catch (Exception ex)
        {
            // Trả về mã HTTP 500 (Lỗi hệ thống - VD: Chết Database)
            return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            // Trả về mã HTTP 401: Không được phép truy cập
            return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác!" });
        }

        return Ok(result); // Trả về Token kèm thông tin
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        // Bóc tách thông tin từ Token (Claims) mà không cần gọi Database
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? User.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = User.FindFirst("FullName")?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            message = "🎉 Chúc mừng! Bạn đã vượt qua Trạm gác JWT thành công!",
            user = new 
            {
                Id = userId,
                FullName = fullName,
                Email = email,
                Role = role
            }
        });
    }
}
