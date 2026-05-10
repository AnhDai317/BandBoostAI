using BandBoostAI.Application.DTOs.Auth;

namespace BandBoostAI.Application.Interfaces.Services;

public interface IAuthService
{
    // Hàm này nhận vào DTO từ API, xử lý và trả về string (có thể là thông báo lỗi hoặc thành công)
    Task<string> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
}