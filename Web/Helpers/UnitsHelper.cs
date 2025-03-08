namespace TresComas.Helpers;

public static class UnitsHelper
{
    public static double ConvertBg(double value, string unit) => 
        unit.Contains("mmol", StringComparison.OrdinalIgnoreCase) ? value : value / 18;
}
