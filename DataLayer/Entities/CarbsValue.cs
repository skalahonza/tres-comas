using DataLayer.Common;

namespace DataLayer.Entities;

public class CarbsValue : TimeSeriesEntity
{
    public required decimal Value { get; set; }
}
