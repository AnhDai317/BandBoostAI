using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BandBoostAI.Application.Interfaces.Services;
using BandBoostAI.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BandBoostAI.Infrastructure.Providers;

public class JwtTokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;

    public JwtTokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // 1. Đọc secret key từ appsettings.json
        var secretKey = _configuration["JwtSettings:SecretKey"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 2. Tạo Payload chứa thông tin User (gọi là Claims)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Phân quyền admin/student
        };

        // 3. Cấu hình thời gian sống của Token
        var expiryMinutes = Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"]);

        // 4. Nhào nặn ra cái Token
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        // 5. Chuyển thành chuỗi string để trả về
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}