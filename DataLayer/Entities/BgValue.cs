namespace DataLayer.Entities;

public class BgValue
{
    public int Id { get; set; }
    public required string ExternalId { get; set; }
    public required double Value { get; set; }
    public required DateTime Time { get; set; }
}
