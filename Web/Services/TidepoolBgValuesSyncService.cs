using DataLayer;
using DataLayer.Entities;

using Microsoft.EntityFrameworkCore;

using Tidepool.Services.Tidepool;

namespace TresComas.Services;

public class TidepoolBgValuesSyncService(ITidepoolClientFactory tidepoolFactory, IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task DoSync()
    {
        var connection = factory.CreateDbContext();
        var users = await connection.TidepoolUserSettings.ToListAsync();

        foreach (var user in users)
        {
            var client = await tidepoolFactory.CreateAsync(user.TidepoolUsername, user.TidepoolPassword);

            var lastValue = await connection.BgValues.OrderByDescending(x => x.Time).FirstOrDefaultAsync();
            var lastTime = lastValue?.Time.AddMinutes(1) ?? DateTime.Today.AddDays(-7);
            var values = await client.GetBgValues(lastTime);

            await connection.BgValues.AddRangeAsync(values.Select(x => new BgValue()
            {
                ExternalId = x.Id,
                Time = x.Time!.Value,
                Value = x.Units == "mmol/l" ? x.Value : x.Value / 18,
                UserId = user.UserId,
            }));

            await connection.SaveChangesAsync();
        }
    }
}
