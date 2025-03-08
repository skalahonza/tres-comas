using DataLayer.Common;

namespace DataLayer.Entities;

public class Profile : Entity
{
    public required string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public required DateTime Time { get; set; }

    public ICollection<ProfileSetting> Settings { get; set; } = [];
}

//TODO better name
public class ProfileSetting : Entity
{
    public int ProfileId { get; set; }

    public required TimeOnly ValidFrom { get; set; }

    public required decimal BasalRate { get; set; }
    public required decimal TargetBg { get; set; }
    public required decimal CarbRation { get; set; }
    /// <summary>
    /// In minutes
    /// </summary>
    public required int InsulineDuration { get; set; }
}
