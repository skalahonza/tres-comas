using DataLayer;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace TresComas.Services;

public class DashboardDataService(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    private const int STEP_SIZE = 3;

    public DateTime PreviousStart => DateTime.Now.AddMonths(-2 * STEP_SIZE).Date.ToUniversalTime();
    public DateTime PreviousEnd => DateTime.Now.AddMonths(-1 * STEP_SIZE).AddDays(1).Date.AddMilliseconds(-1).ToUniversalTime();

    public DateTime CurrentStart => DateTime.Now.AddMonths(-1 * STEP_SIZE).AddDays(1).Date.ToUniversalTime();
    public DateTime CurrentEnd => DateTime.Now.ToUniversalTime();

    public async Task<(List<BgValue> Prev, List<BgValue> Curr)> GetBgData()
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        var previousBg = await dbContext.BgValues.Where(x => x.Time >= PreviousStart && x.Time <= PreviousEnd).ToListAsync();
        var currentBg = await dbContext.BgValues.Where(x => x.Time >= CurrentStart && x.Time <= CurrentEnd).ToListAsync();

        return (previousBg, currentBg);
    }
}
