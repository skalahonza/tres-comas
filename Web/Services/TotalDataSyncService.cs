using DataLayer;
using DataLayer.Entities;
using Dexcom.Services;
using Microsoft.EntityFrameworkCore;
using Tidepool.Services.Tidepool;
using TresComas.Invocables;

namespace TresComas.Services;

public class TotalDataSyncService(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    UserProvider userProvider,
    ITidepoolClientFactory tidepoolClientFactory,
    IDexcomClientFactory dexcomClientFactory,
    TidepoolCoreSyncService tidepollSyncService,
    DexcomCoreSyncService dexcomSyncService,
    FhirSync fhirSync)
{
    private const int DAY_STEP_SIZE = 30;

    public async Task SyncAllData(DateTime startDate)
    {
        var userId = await userProvider.GetCurrentUserId();
        await ClearData(userId);

        using var dbcontext = await contextFactory.CreateDbContextAsync();

        var tidepoolSettings = await dbcontext.TidepoolUserSettings.FirstOrDefaultAsync(t => t.UserId == userId);
        if (tidepoolSettings is not null)
            await SyncTidepool(tidepoolSettings, startDate);

        var dexcomSettings = await dbcontext.DexcomUserSettings.FirstOrDefaultAsync(d => d.UserId == userId);
        if (dexcomSettings is not null)
            await SyncDexcom(dexcomSettings, startDate);
    }

    private async Task SyncTidepool(TidepoolUserSettings settings, DateTime startDate)
    {
        var client = await tidepoolClientFactory.CreateAsync(settings.TidepoolUsername, settings.TidepoolPassword);
        DateTime endDate;
        bool shouldContinue = true;

        do
        {
            endDate = startDate.AddDays(DAY_STEP_SIZE);
            if (endDate.Date >= DateTime.Now.Date)
            {
                endDate = DateTime.Now;
                shouldContinue = false;
            }

            var bgValues = await client.GetBgValues(startDate, endDate);
            var bolusValues = await client.GetBolusAsync(startDate, endDate);
            var wizardsValues = await client.GetWizardAsync(startDate, endDate);
            var pumpSettings = await client.GetPumpSettingsAsync(startDate, endDate);

            await tidepollSyncService.SaveBgValues(bgValues, settings.UserId);
            await tidepollSyncService.SaveBolusValues(bolusValues, settings.UserId);
            await tidepollSyncService.SaveCarbsValues(wizardsValues, settings.UserId);
            await tidepollSyncService.SaveProfiles(pumpSettings, settings.UserId);

            startDate = endDate;
        } while (shouldContinue);
    }

    private async Task SyncDexcom(DexcomUserSettings settings, DateTime startDate)
    {
        var client = await dexcomClientFactory.Create(settings.AuthCode);
        DateTime endDate;
        bool shouldContinue = true;

        do
        {
            endDate = startDate.AddDays(DAY_STEP_SIZE);
            if (endDate.Date >= DateTime.Now.Date)
            {
                endDate = DateTime.Now;
                shouldContinue = false;
            }

            var bgValues = await client.GetEgvs(startDate, endDate);

            await dexcomSyncService.SaveBgValues(bgValues, settings.UserId);

            startDate = endDate;
        } while (shouldContinue);
    }

    private async Task ClearData(string? userId)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();
        
        var patientId = await dbContext.Users.Where(x => x.Id == userId).Select(x => x.FhirId).FirstOrDefaultAsync();
        if (!string.IsNullOrEmpty(patientId))
        {
            await fhirSync.DeleteValues(patientId);
        }
        
        await dbContext.BgValues.Where(x => x.UserId == userId).ExecuteDeleteAsync();
        await dbContext.BolusValues.Where(x => x.UserId == userId).ExecuteDeleteAsync();
        await dbContext.CarbsValues.Where(x => x.UserId == userId).ExecuteDeleteAsync();
        await dbContext.Profiles.Where(x => x.UserId == userId).ExecuteDeleteAsync();
    }
}
