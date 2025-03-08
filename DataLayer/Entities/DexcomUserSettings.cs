using DataLayer.Common;

namespace DataLayer.Entities;

public class DexcomUserSettings : Entity
{
    public ApplicationUser User { get; set; } = null!;
    public required string UserId { get; set; }

    public required string AuthCode { get; set; }
}
