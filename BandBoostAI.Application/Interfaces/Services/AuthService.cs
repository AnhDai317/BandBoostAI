using BandBoostAI.Application.DTOs.Auth;
using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Application.Interfaces.Services;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.Services;

public class AuthService : IAuthService
{
   private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;

    public AuthService(IUserRepository userRepository, ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
    }
    public async Task<string> RegisterAsync(RegisterRequestDto request)
    {
        // 1. Kiểm tra Email đã tồn tại chưa
        var isEmailUnique = await _userRepository.IsEmailUniqueAsync(request.Email);
        if (!isEmailUnique)
        {
            return "Email này đã được đăng ký trong hệ thống!";
        }

        // 2. Hash mật khẩu (Tạm thời dùng thư viện BCrypt.Net-Next)
        // LƯU Ý: Tuyệt đối không lưu mật khẩu dạng Text (như "123456") vào Database
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Tạo Entity mới để lưu vào DB
        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = hashedPassword,
            FullName = request.FullName,
            Role = Role.Student, // Mặc định ai đăng ký cũng là Học viên
            TargetBandScore = 0 // Chưa setup mục tiêu
        };

        // 4. Gọi Repository để lưu xuống SQL Server
        await _userRepository.AddAsync(newUser);

        return "Đăng ký tài khoản thành công!";
    }
    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // 1. Tìm User bằng Email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        // 2. Nếu không tìm thấy hoặc Pass sai -> Trả về null
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null; // Trả về null để Controller biết là lỗi đăng nhập
        }

        // 3. Nếu đúng, nhờ TokenProvider in cho cái JWT
        var token = _tokenProvider.GenerateToken(user);

        // 4. Đóng gói kết quả trả về Frontend
        return new AuthResponseDto
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}