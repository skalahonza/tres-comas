using DataLayer.Entities;
using DataLayer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations;

internal class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ConfigureEntity();

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Settings)
            .WithOne()
            .HasForeignKey(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class ProfileSettingConfiguration : IEntityTypeConfiguration<ProfileSetting>
{
    public void Configure(EntityTypeBuilder<ProfileSetting> builder)
    {
        builder.ConfigureEntity();
    }
}