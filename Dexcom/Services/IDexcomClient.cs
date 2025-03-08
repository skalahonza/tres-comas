using Dexcom.Models;

namespace Dexcom.Services;

public interface IDexcomClient
{
    Task<EgvsResponse> GetEgvs(DateTime? startDate = null, DateTime? endDate = null);
}
