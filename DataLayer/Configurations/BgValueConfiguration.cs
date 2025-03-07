using DataLayer.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations;

internal class BgValueConfiguration : IEntityTypeConfiguration<BgValue>
{
    public void Configure(EntityTypeBuilder<BgValue> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
    }
}
