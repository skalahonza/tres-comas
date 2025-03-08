using DataLayer;

using Microsoft.EntityFrameworkCore;

using Tidepool.Services.Tidepool;

namespace TresComas.Services;

public class TidepoolBgValuesSyncService(ITidepoolClientFactory tidepoolFactory, IDbContextFactory<ApplicationDbContext> factory, TidepoolCoreSyncService syncService)
{
    public async Task DoSync()
    {
        var connection = factory.CreateDbContext();
        var users = await connection.TidepoolUserSettings.ToListAsync();

        foreach (var user in users)
        {
            var client = await tidepoolFactory.CreateAsync(user.TidepoolUsername, user.TidepoolPassword);

            var lastValue = await connection.BgValues.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.Time).FirstOrDefaultAsync();
            var lastTime = lastValue?.Time.AddMinutes(1) ?? DateTime.Today.AddDays(-7);
            var values = await client.GetBgValues(lastTime);

            await syncService.SaveBgValues(values, user.UserId);
        }
    }
}
