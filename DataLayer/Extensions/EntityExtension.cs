using DataLayer.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Extensions;

public static class EntityExtension
{
    public static EntityTypeBuilder<TEntity> ConfigureEntity<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : Entity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        return builder;
    }
}
