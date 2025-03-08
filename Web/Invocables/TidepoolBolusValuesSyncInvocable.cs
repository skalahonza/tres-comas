using Coravel.Invocable;
using DataLayer;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Tidepool.Services.Tidepool;

namespace TresComas.Invocables;

public class TidepoolBolusValuesSyncInvocable(IDbContextFactory<ApplicationDbContext> contextFactory, ITidepoolClientFactory tidepoolFactory) : IInvocable
{
    public async Task Invoke()
    {
        var dbContext = contextFactory.CreateDbContext();
        var users = await dbContext.TidepoolUserSettings.ToListAsync();

        foreach (var user in users)
        {
            var client = await tidepoolFactory.CreateAsync(user.TidepoolUsername, user.TidepoolPassword);

            var lastValue = await dbContext.BolusValues
                .Where(b => b.UserId == user.UserId)
                .OrderByDescending(b => b.Time)
                .FirstOrDefaultAsync();
            var syncFrom = lastValue?.Time.AddMinutes(1) ?? DateTime.Now.AddDays(-7);

            var values = await client.GetBolusAsync(syncFrom);
            dbContext.BolusValues.AddRange(values
                .Where(v => v.Id != null && v.Time != null && v.Normal != null)
                .Select(v => new BolusValue()
                {
                    ExternalId = v.Id!,
                    Time = v.Time!.Value,
                    Unit = (decimal)v.Normal!.Value,
                    UserId = user.UserId,
                }));
            await dbContext.SaveChangesAsync();
        }
    }
}
