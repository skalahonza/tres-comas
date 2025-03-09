using DataLayer;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace TresComas.Services;

public class DashboardDataService(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    private const int STEP_SIZE = 3;

    public static DateTime PreviousStart => DateTime.Now.AddMonths(-2 * STEP_SIZE).Date.ToUniversalTime();
    public static DateTime PreviousEnd => DateTime.Now.AddMonths(-1 * STEP_SIZE).AddDays(1).Date.AddMilliseconds(-1).ToUniversalTime();

    public static DateTime CurrentStart => DateTime.Now.AddMonths(-1 * STEP_SIZE).AddDays(1).Date.ToUniversalTime();
    public static DateTime CurrentEnd => DateTime.Now.ToUniversalTime();

    public async Task<(List<BgValue> Prev, List<BgValue> Curr)> GetBgData()
    {
        using var dbContext = await contextFactory.CreateDbContextAsync();
        var previousBg = await dbContext.BgValues.Where(x => x.Time >= PreviousStart && x.Time <= PreviousEnd).ToListAsync();
        var currentBg = await dbContext.BgValues.Where(x => x.Time >= CurrentStart && x.Time <= CurrentEnd).ToListAsync();

        return (previousBg, currentBg);
    }

    public static Progress CalculateProgress(History prev, History curr)
    {
        Progress progress = new();

        if (curr.Gri < prev.Gri)
            progress.Emoji = "😁";
        else if (curr.Gri == prev.Gri)
            progress.Emoji = "🙂";
        else
            progress.Emoji = "☹️";

        var inRangeDiff = curr.InRange - prev.InRange;
        if (inRangeDiff > 0)
            progress.Description += $"Great you are in range about {inRangeDiff} % more than in previous period. Hurray 🥳";
        else if (inRangeDiff < 0)
            progress.Description += $"Oh man you are too sweet (about {Math.Round(Math.Abs(inRangeDiff), 2)} % more than in the past). You should try harder!";
        else
            progress.Description += "You were able to hold you own and not move a step!";

        return progress;
    }
}

public class Progress
{
    public string Emoji { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class History
{
    public int Gri { get; set; }
    public double VeryHigh { get; set; }
    public double High { get; set; }
    public double InRange { get; set; }
    public double Low { get; set; }
    public double VeryLow { get; set; }
}