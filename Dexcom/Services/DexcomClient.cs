using Dexcom.Models;
using Pathoschild.Http.Client;

namespace Dexcom.Services;

internal class DexcomClient(IClient client) : IDexcomClient
{
    public Task<EgvsResponse> GetEgvs(DateTime? startDate = null, DateTime? endDate = null)
        => client.GetAsync("v3/users/self/egvs")
        .WithArgument("startDate", startDate?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"))
        .WithArgument("endDate", endDate?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"))
        .As<EgvsResponse>();
}
