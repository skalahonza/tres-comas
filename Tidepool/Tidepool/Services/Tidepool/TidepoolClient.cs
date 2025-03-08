using Pathoschild.Http.Client;

using System.Globalization;

using Tidepool.Extensions;
using Tidepool.Model.Tidepool;

namespace Tidepool.Services.Tidepool;

public class TidepoolClient : ITidepoolClient
{
    private readonly string _userId;
    private readonly IClient _client;

    internal TidepoolClient(IClient client, string userId)
    {
        _client = client;
        _userId = userId;
    }

    public async Task<IReadOnlyList<Bolus>> GetBolusAsync(DateTime? start = null, DateTime? end = null) =>
        await _client
            .GetAsync($"data/{_userId}")
            .WithArgument("startDate", start?.ToUniversalTime().ToString("o"))
            .WithArgument("endDate", end?.ToUniversalTime().ToString("o"))
            .WithArgument("type", nameof(DataType.Bolus).ToCamelCase())
            .AsArray<Bolus>();

    public async Task<IReadOnlyList<Food>> GetFoodAsync(DateTime? start = null, DateTime? end = null) =>
        await _client
            .GetAsync($"data/{_userId}")
            .WithArgument("startDate", start?.ToUniversalTime().ToString("o"))
            .WithArgument("endDate", end?.ToUniversalTime().ToString("o"))
            .WithArgument("type", nameof(DataType.Food).ToCamelCase())
            .AsArray<Food>();

    public async Task<IReadOnlyList<PhysicalActivity>> GetPhysicalActivityAsync(DateTime? start = null,
        DateTime? end = null) =>
        await _client
            .GetAsync($"data/{_userId}")
            .WithArgument("startDate", start?.ToUniversalTime().ToString("o"))
            .WithArgument("endDate", end?.ToUniversalTime().ToString("o"))
            .WithArgument("type", nameof(DataType.PhysicalActivity).ToCamelCase())
            .AsArray<PhysicalActivity>();

    public async Task<IReadOnlyList<PumpSettings>> GetPumpSettingsAsync(DateTime? start = null,
        DateTime? end = null) =>
        await _client
            .GetAsync($"data/{_userId}")
            .WithArgument("startDate", start?.ToUniversalTime().ToString("o"))
            .WithArgument("endDate", end?.ToUniversalTime().ToString("o"))
            .WithArgument("type", nameof(DataType.PumpSettings).ToCamelCase())
            .AsArray<PumpSettings>();

    public async Task<IReadOnlyList<BgValue>> GetBgValues(DateTime? start = null,
        DateTime? end = null)
    {
        // https://tidepool.stoplight.io/docs/tidepool-api/47411eab004f3-common-fields
        // type
        // Allowed values: cbg, smbg
        var types = string.Join(',', nameof(DataType.Cbg), nameof(DataType.Smbg));
        return await _client
            .GetAsync($"data/{_userId}")
            .WithArgument("startDate", start?.ToUniversalTime().ToString("o"))
            .WithArgument("endDate", end?.ToUniversalTime().ToString("o"))
            .WithArgument("type", types.ToLower(CultureInfo.InvariantCulture))
            .AsArray<BgValue>();
    }

    public async Task<IReadOnlyList<WizardValue>> GetWizardAsync(DateTime? start = null, DateTime? end = null)
        => await _client
            .GetAsync($"data/{_userId}")
            .WithArgument("startDate", start?.ToUniversalTime().ToString("o"))
            .WithArgument("endDate", end?.ToUniversalTime().ToString("o"))
            .WithArgument("type", nameof(DataType.Wizard).ToCamelCase())
            .AsArray<WizardValue>();
}