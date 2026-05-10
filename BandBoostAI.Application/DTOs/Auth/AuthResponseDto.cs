namespace BandBoostAI.Application.DTOs.Auth;

public class AuthResponseDto
{
    public required string Token { get; set; } // Chuỗi JWT để xài dần
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string Role { get; set; } = string.Empty;
}