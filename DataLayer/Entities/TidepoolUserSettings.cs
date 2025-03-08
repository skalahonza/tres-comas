namespace DataLayer.Entities;

public class TidepoolUserSettings
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string? TidepoolUserId { get; set; }
    public string TidepoolUsername { get; set; } = "";
    public string TidepoolPassword { get; set; } = "";
}
