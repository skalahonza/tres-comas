using DataLayer;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Tidepool.Model.Tidepool;
using TresComas.Helpers;
using DbBgValue = DataLayer.Entities.BgValue;
using TidepoolBgValue = Tidepool.Model.Tidepool.BgValue;

namespace TresComas.Services;

public class TidepoolCoreSyncService(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task SaveBgValues(IReadOnlyCollection<TidepoolBgValue> bgValues, string userId)
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        dbContext.BgValues.AddRange(bgValues.Select(x => new DbBgValue()
        {
            ExternalId = x.Id,
            Time = x.Time!.Value,
            Value = UnitsHelper.ConvertBg(x.Value, x.Units),
            UserId = userId,
        }));
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveBolusValues(IReadOnlyCollection<Bolus> bolusValues, string userId)
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        dbContext.BolusValues.AddRange(bolusValues
            .Where(v => v.Id != null && v.Time != null && v.Normal != null)
            .Select(v => new BolusValue()
            {
                ExternalId = v.Id!,
                Time = v.Time!.Value,
                Unit = (decimal)v.Normal!.Value,
                UserId = userId,
            }));
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveCarbsValues(IReadOnlyCollection<WizardValue> wizardValues, string userId)
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        dbContext.AddRange(wizardValues
                .Select(v => new CarbsValue()
                {
                    ExternalId = v.Id!,
                    Time = v.Time,
                    Value = v.CarbInput,
                    UserId = userId
                }));
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveProfiles(IReadOnlyCollection<PumpSettings> pumpSettings, string userId)
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        foreach (var value in pumpSettings)
        {
            var keys = value.BgTargets.Select(p => p.Key);

            Dictionary<string, Profile> profiles = keys.ToDictionary(
                k => k,
                k => new Profile()
                {
                    Time = ConvertDateString(k),
                    UserId = userId
                });

            foreach (var profile in profiles)
            {
                var settings = value.BgTargets[profile.Key].ToDictionary(
                    x => x.Start,
                    x => new ProfileSettingPrototype()
                    {
                        TargetBg = (decimal)x.Target
                    });

                foreach (var basal in value.BasalSchedules[profile.Key])
                {
                    if (settings.TryGetValue(basal.Start, out var existingSettings))
                    {
                        existingSettings.BasalRate = (decimal)basal.Rate;
                    }
                    else
                    {
                        ProfileSettingPrototype newPrototype = new() { BasalRate = (decimal)basal.Rate };
                        settings.Add(basal.Start, newPrototype);
                    }
                }

                foreach (var carbRation in value.CarbRatios[profile.Key])
                {
                    if (settings.TryGetValue(carbRation.Start, out var existingSettings))
                    {
                        existingSettings.CarbRation = (decimal)carbRation.Amount;
                    }
                    else
                    {
                        ProfileSettingPrototype newPrototype = new() { CarbRation = (decimal)carbRation.Amount };
                        settings.Add(carbRation.Start, newPrototype);
                    }
                }

                foreach (var insuline in value.InsulinSensitivities[profile.Key])
                {
                    if (settings.TryGetValue(insuline.Start, out var existingSettings))
                    {
                        existingSettings.InsulineDuration = (decimal)insuline.Amount;
                    }
                    else
                    {
                        ProfileSettingPrototype newPrototype = new() { InsulineDuration = (decimal)insuline.Amount };
                        settings.Add(insuline.Start, newPrototype);
                    }
                }

                var dbSettings = settings
                    .Select(Build)
                    .ToList();
                profile.Value.Settings = dbSettings;
                dbContext.Add(profile.Value);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static ProfileSetting Build(KeyValuePair<int, ProfileSettingPrototype> prototype)
    {
        var timeSpan = TimeSpan.FromMilliseconds(prototype.Key);
        TimeOnly time = new(timeSpan.Ticks);

        return prototype.Value.Build(time);
    }

    private static DateTime ConvertDateString(string dateString)
    {
        var formatedDate = $"{DateTime.Now.Year}-{dateString}";
        var format = "yyyy-MMMd";
        var success = DateTime.TryParseExact(formatedDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime);
        if (!success)
            return DateTime.Now.ToUniversalTime();

        if (dateTime > DateTime.Now)
            return dateTime.AddYears(-1).ToUniversalTime();

        return dateTime.ToUniversalTime();
    }

    class ProfileSettingPrototype
    {
        public decimal BasalRate { get; set; }
        public decimal TargetBg { get; set; }
        public decimal CarbRation { get; set; }
        public decimal InsulineDuration { get; set; }

        public ProfileSetting Build(TimeOnly validFrom)
         => new()
         {
             BasalRate = BasalRate,
             CarbRation = CarbRation,
             InsulineDuration = InsulineDuration,
             TargetBg = TargetBg,
             ValidFrom = validFrom,
         };
    }
}
