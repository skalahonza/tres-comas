using DataLayer.Entities;
using DataLayer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations;

internal class CarbsValueConfiguration : IEntityTypeConfiguration<CarbsValue>
{
    public void Configure(EntityTypeBuilder<CarbsValue> builder)
    {
        builder.ConfigureTimeSeriesEntity();
    }
}
