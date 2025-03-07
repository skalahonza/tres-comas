using DataLayer.Common;

namespace DataLayer.Entities;

public class BolusValue : TimeSeriesEntity
{
    public required decimal Unit { get; set; }
}
