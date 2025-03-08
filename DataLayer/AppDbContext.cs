using DataLayer.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<BgValue> BgValues => Set<BgValue>();
    public DbSet<BolusValue> BolusValues => Set<BolusValue>();
    public DbSet<CarbsValue> CarbsValues => Set<CarbsValue>();

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<ProfileSetting> ProfileSettings => Set<ProfileSetting>();
    public DbSet<TidepoolUserSettings> TidepoolUserSettings => Set<TidepoolUserSettings>();
    public DbSet<DexcomUserSettings> DexcomUserSettings => Set<DexcomUserSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

