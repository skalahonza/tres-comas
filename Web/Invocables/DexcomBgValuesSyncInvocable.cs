using Coravel.Invocable;
using DataLayer;
using DataLayer.Entities;
using Dexcom.Services;
using Microsoft.EntityFrameworkCore;

namespace TresComas.Invocables;

public class DexcomBgValuesSyncInvocable(IDbContextFactory<ApplicationDbContext> contextFactory, IDexcomClientFactory dexcomClientFactory) : IInvocable
{
    public async Task Invoke()
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        var users = await dbContext.DexcomUserSettings.ToListAsync();

        foreach (var user in users)
        {
            var client = await dexcomClientFactory.Create(user.AuthCode);

            var lastValue = await dbContext.BgValues
                .Where(x => x.UserId == user.UserId)
                .OrderByDescending(x => x.Time)
                .FirstOrDefaultAsync();
            var syncFrom = lastValue?.Time.AddMinutes(1) ?? DateTime.Now.AddDays(-7);
            var syncTo = DateTime.Now;

            var data = await client.GetEgvs(syncFrom, syncTo);
            dbContext.BgValues.AddRange(data.Records.Select(r => new BgValue()
            {
                ExternalId = r.RecordId,
                Time = r.SystemTime,
                UserId = user.UserId,
                Value = r.Value
            }));
            await dbContext.SaveChangesAsync();
        }
    }
}
