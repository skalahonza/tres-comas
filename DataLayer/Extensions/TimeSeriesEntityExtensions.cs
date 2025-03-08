using DataLayer.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Extensions;

internal static class TimeSeriesEntityExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureTimeSeriesEntity<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : TimeSeriesEntity
    {
        builder.ConfigureEntity();

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Time);

        return builder;
    }
}
