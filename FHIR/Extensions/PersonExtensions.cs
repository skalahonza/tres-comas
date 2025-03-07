using Hl7.Fhir.Model;

namespace FHIR.Extensions;

public static class PersonExtensions
{
    private const string FireBaseToken = "https://hl7.org/fhir/StructureDefinition/user-firebase-token";
    private const string IsCprCapable = "https://hl7.org/fhir/StructureDefinition/user-is-cpr-capable";

    public static void SetEmail(this Person person, string email)
    {
        var telecom = person.GetTelecom(ContactPoint.ContactPointSystem.Email);
        telecom.Value = email;
    }

    public static void SetPhone(this Person person, string phone)
    {
        var telecom = person.GetTelecom(ContactPoint.ContactPointSystem.Phone);
        telecom.Value = phone;
    }

    public static void SetFireBaseToken(this Person person, string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return;
        }

        var extension = person.Extension.FirstOrDefault(x => x.Url == FireBaseToken) ??
                        person.AddExtension(FireBaseToken, new FhirString(token));
        extension.Value = new FhirString(token);
    }

    public static string? GetFireBaseToken(this Person person)
    {
        var extension = person.Extension.FirstOrDefault(x => x.Url == FireBaseToken);
        return extension?.Value?.ToString();
    }

    public static void SetIsCprCapable(this Person person, bool isCprCapable)
    {
        var extension = person.Extension.FirstOrDefault(x => x.Url == IsCprCapable) ??
                        person.AddExtension(IsCprCapable, new FhirBoolean(isCprCapable));
        extension.Value = new FhirBoolean(isCprCapable);
    }

    public static bool GetIsCprCapable(this Person person)
    {
        var extension = person.Extension.FirstOrDefault(x => x.Url == IsCprCapable);
        return extension?.Value?.ToString() == "true";
    }

    private static ContactPoint GetTelecom(this Person person, ContactPoint.ContactPointSystem system)
    {
        var telecom = person.Telecom.FirstOrDefault(t => t.System == system);
        if (telecom is null)
        {
            telecom = new ContactPoint
            {
                System = system
            };
            person.Telecom.Add(telecom);
        }

        return telecom;
    }
}