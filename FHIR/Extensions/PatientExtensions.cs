using Hl7.Fhir.Model;

namespace FHIR.Extensions;

public static class PatientExtensions
{
    public static string GetEmail(this Patient patient) =>
        patient.Telecom?
            .FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Email)?
            .Value ?? string.Empty;
    
    public static string SetEmail(this Patient patient, string email)
    {
        var telecom = patient.Telecom?
            .FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Email);
        
        if (telecom != null)
        {
            telecom.Value = email;
        }
        
        else
        {
            patient.Telecom ??= [];
            patient.Telecom.Add(new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Value = email
            });
        }

        return email;
    }
}