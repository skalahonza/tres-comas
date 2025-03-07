using DataLayer.Entities;
using DataLayer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations;

internal class BgValueConfiguration : IEntityTypeConfiguration<BgValue>
{
    public void Configure(EntityTypeBuilder<BgValue> builder)
    {
        builder.ConfigureTimeSeriesEntity();
    }
}
