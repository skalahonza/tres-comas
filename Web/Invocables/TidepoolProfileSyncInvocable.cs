using Coravel.Invocable;

using DataLayer;
using DataLayer.Entities;

using Hl7.Fhir.Rest;

using Microsoft.EntityFrameworkCore;

using System.Globalization;

using Tidepool.Services.Tidepool;

namespace TresComas.Invocables;

public class TidepoolProfileSyncInvocable(IDbContextFactory<ApplicationDbContext> contextFactory, ITidepoolClientFactory tidepoolFactory) : IInvocable
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

            foreach (var value in values)
            {
                //TODO: create profiles for all possible keys
                var keys = value.BgTargets.Select(p => p.Key);

                Dictionary<string, Profile> profiles = keys.ToDictionary(
                    k => k,
                    k => new Profile()
                    {
                        Time = ConvertDateString(k),
                        UserId = user.UserId
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