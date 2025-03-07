using DataLayer.Common;

namespace DataLayer.Entities;

public class BgValue : TimeSeriesEntity
{
    public required double Value { get; set; }
}
