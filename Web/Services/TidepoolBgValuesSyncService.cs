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
        var client = await tidepoolFactory.CreateAsync();

        var lastValue = await connection.BgValues.OrderByDescending(x => x.Time).FirstOrDefaultAsync();
        var lastTime = lastValue?.Time ?? DateTime.Now;

        var values = await client.GetBgValues(lastTime);

        await connection.BgValues.AddRangeAsync(values.Select(x => new BgValue()
        {
            ExternalId = x.Id,
            Time = x.Time!.Value,
            Value = x.Units == "mmol/l" ? x.Value : x.Value / 18,
        }));

        await connection.SaveChangesAsync();
    }
}
