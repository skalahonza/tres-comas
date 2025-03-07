using DataLayer.Entities;

namespace DataLayer.Common;

public abstract class TimeSeriesEntity : Entity
{
    public required string ExternalId { get; set; }

    public required string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public required DateTime Time { get; set; }
}
