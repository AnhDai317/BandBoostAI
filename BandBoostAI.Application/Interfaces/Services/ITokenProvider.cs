using BandBoostAI.Domain.Entities;

namespace BandBoostAI.Application.Interfaces.Services;

public interface ITokenProvider
{
    // Nhận vào 1 User và nhả ra 1 chuỗi JWT
    string GenerateToken(User user);
}