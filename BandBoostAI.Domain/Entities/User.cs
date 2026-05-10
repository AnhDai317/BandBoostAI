using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Domain.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public Role Role { get; set; } = Role.Student;

    public decimal TargetBandScore { get; set; }
    public decimal? CurrentBandScore { get; set; }
}