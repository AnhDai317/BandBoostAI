using System.ComponentModel.DataAnnotations;

namespace BandBoostAI.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Họ và tên không được để trống")]
    public required string FullName { get; set; }
}