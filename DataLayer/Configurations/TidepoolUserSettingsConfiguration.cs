using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations;

internal class TidepoolUserSettingsConfiguration : IEntityTypeConfiguration<TidepoolUserSettings>
{
    public void Configure(EntityTypeBuilder<TidepoolUserSettings> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<TidepoolUserSettings>(e => e.UserId);
    }
}
