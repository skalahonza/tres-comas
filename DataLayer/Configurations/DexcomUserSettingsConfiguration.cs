using DataLayer.Entities;
using DataLayer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations;

internal class DexcomUserSettingsConfiguration : IEntityTypeConfiguration<DexcomUserSettings>
{
    public void Configure(EntityTypeBuilder<DexcomUserSettings> builder)
    {
        builder.ConfigureEntity();
    }
}
