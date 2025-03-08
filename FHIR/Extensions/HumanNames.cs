using Hl7.Fhir.Model;

namespace FHIR.Extensions;

public static class HumanNames
{
    public static IEnumerable<HumanName> FromString(string name) =>
        name.Split().Select(x => new HumanName {Text = x});

    public static string ToFullName(this IEnumerable<HumanName> names)
    {
        var name = names.FirstOrDefault();
        return name is not null ? string.Join(" ", name.Given) + " " + name.Family : "Unknown";
    }
}