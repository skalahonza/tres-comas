using DataLayer;
using DataLayer.Entities;
using Dexcom.Models;
using Microsoft.EntityFrameworkCore;
using TresComas.Helpers;

namespace TresComas.Services;

public class DexcomCoreSyncService(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task SaveBgValues(EgvsResponse response, string userId)
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        dbContext.BgValues.AddRange(response.Records.Select(r => new BgValue()
        {
            ExternalId = r.RecordId,
            Time = r.SystemTime,
            UserId = userId,
            Value = UnitsHelper.ConvertBg(r.Value, r.Unit)
        }));
        await dbContext.SaveChangesAsync();
    }
}
