using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Domain.Entities;

public class LearningActivity : BaseEntity
{
    public Guid UserId { get; set; }
    public LearningActivityType Type { get; set; }
    public int ItemsCompleted { get; set; }
    public int MinutesSpent { get; set; }
    public int ExperiencePoints { get; set; }

    public User? User { get; set; }
}
