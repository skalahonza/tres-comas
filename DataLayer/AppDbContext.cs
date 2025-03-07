using DataLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<BgValue> BgValues => Set<BgValue>();
    public DbSet<BolusValue> BolusValues => Set<BolusValue>();
    public DbSet<CarbsValue> CarbsValues => Set<CarbsValue>();
    public DbSet<TidepoolUserSettings> TidepoolUserSettings => Set<TidepoolUserSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

