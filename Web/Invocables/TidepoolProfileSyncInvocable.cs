using Coravel.Invocable;

using DataLayer;
using Hl7.Fhir.Rest;

using Microsoft.EntityFrameworkCore;

using Tidepool.Services.Tidepool;
using TresComas.Services;

namespace TresComas.Invocables;

public class TidepoolProfileSyncInvocable(IDbContextFactory<ApplicationDbContext> contextFactory, ITidepoolClientFactory tidepoolFactory, TidepoolCoreSyncService syncService) : IInvocable
{
    public async Task Invoke()
    {
        var dbContext = contextFactory.CreateDbContext();
        var users = await dbContext.TidepoolUserSettings.ToListAsync();

        foreach (var user in users)
        {
            var client = await tidepoolFactory.CreateAsync(user.TidepoolUsername, user.TidepoolPassword);

            var lastValue = await dbContext.Profiles
                .Where(p => p.UserId == user.UserId)
                .OrderByDescending(p => p.Time)
                .FirstOrDefaultAsync();
            var syncFrom = lastValue?.Time.AddMinutes(1) ?? DateTime.Now.AddDays(-7);

            var values = await client.GetPumpSettingsAsync(syncFrom);

            await syncService.SaveProfiles(values, user.UserId);
        }
    }
}