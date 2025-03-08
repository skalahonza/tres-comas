using Coravel.Invocable;
using DataLayer;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Tidepool.Services.Tidepool;

namespace TresComas.Invocables;

public class TidepoolCarbsValuesSyncInvocable(IDbContextFactory<ApplicationDbContext> contextFactory, ITidepoolClientFactory tidepoolFactory) : IInvocable
{
    public async Task Invoke()
    {
        var dbContext = contextFactory.CreateDbContext();
        var users = await dbContext.TidepoolUserSettings.ToListAsync();

        foreach (var user in users)
        {
            var client = await tidepoolFactory.CreateAsync(user.TidepoolUsername, user.TidepoolPassword);

            var lastValue = await dbContext.CarbsValues
                .Where(v => v.UserId == user.UserId)
                .OrderByDescending(v => v.Time)
                .FirstOrDefaultAsync();
            var syncFrom = lastValue?.Time.AddMinutes(1) ?? DateTime.Now.AddDays(-7);

            var values = await client.GetWizardAsync(syncFrom);
            dbContext.AddRange(values
                .Select(v => new CarbsValue()
                {
                    ExternalId = v.Id!,
                    Time = v.Time,
                    Value = v.CarbInput,
                    UserId = user.UserId
                }));

            await dbContext.SaveChangesAsync();
        }
    }
}
