namespace TresComas.Helpers;

public static class UnitsHelper
{
    public static double ConvertBg(double value, string unit)
    {
        if (unit.Contains("mmol/l", StringComparison.InvariantCulture))
            return value;

        return value / 18;
    }
}
