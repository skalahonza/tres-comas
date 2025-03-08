using DataLayer;
using DataLayer.Entities;

using Microsoft.EntityFrameworkCore;

using Tidepool.Services.Tidepool;
using TresComas.Helpers;

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

            var lastValue = await connection.BgValues.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.Time).FirstOrDefaultAsync();
            var lastTime = lastValue?.Time.AddMinutes(1) ?? DateTime.Today.AddDays(-7);
            var values = await client.GetBgValues(lastTime);

            connection.BgValues.AddRange(values.Select(x => new BgValue()
            {
                ExternalId = x.Id,
                Time = x.Time!.Value,
                Value = UnitsHelper.ConvertBg(x.Value, x.Units),
                UserId = user.UserId,
            }));

            await connection.SaveChangesAsync();
        }
    }
}
