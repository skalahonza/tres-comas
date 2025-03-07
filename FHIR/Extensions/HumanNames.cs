using Hl7.Fhir.Model;

namespace FHIR.Extensions;

public static class HumanNames
{
    public static IEnumerable<HumanName> FromString(string name) =>
        name.Split().Select(x => new HumanName {Text = x});

    public static string ToString(this IEnumerable<HumanName> names) =>
        string.Join(" ", names.Select(x => x.Text));
}