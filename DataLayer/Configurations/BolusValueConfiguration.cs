using DataLayer.Entities;
using DataLayer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations;

internal class BolusValueConfiguration : IEntityTypeConfiguration<BolusValue>
{
    public void Configure(EntityTypeBuilder<BolusValue> builder)
    {
        builder.ConfigureTimeSeriesEntity();
    }
}
