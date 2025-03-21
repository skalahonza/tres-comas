using DataLayer.Entities;
using FHIR.Extensions;

namespace FHIR;

using Hl7.Fhir.Model;

// udělat identifier z ikemu
// tabular ML, vector search
// fhir sql builder
// vector db - vytvoř nám recommendation pro pacienta
// zjistit od doktora Béma guidelines pro tu vector DB
// gri dát do observation - a do recommendation dát že to je based on observation

public static class Mappings
{
    public static Observation CreateObservation(this BgValue bgValue, string patientId)
    {
        var id = Guid.CreateVersion7();
        bgValue.FhirId = id.ToString();
        return new Observation
        {
            Id = id.ToString(),
            Subject = new ResourceReference($"Patient/{patientId}"),
            Status = ObservationStatus.Final,
            Code = new CodeableConcept("http://loinc.org", "15074-8", "Glucose [Moles/volume] in Blood"),
            Value = new Quantity
            {
                Value = (decimal)bgValue.Value,
                Unit = "mmol/L",
                System = "http://unitsofmeasure.org",
                Code = "mmol/L"
            },
            Effective = new FhirDateTime(bgValue.Time)
        };
    }
    
    public static MedicationAdministration CreateMedicationAdministration(this BolusValue bolus, string patientId)
    {
        var id = Guid.CreateVersion7();
        bolus.FhirId = id.ToString();
        return new MedicationAdministration
        {
            Id = id.ToString(),
            Subject = new ResourceReference($"Patient/{patientId}"),
            Status = MedicationAdministration.MedicationAdministrationStatusCodes.Completed,
            Medication = new CodeableConcept("http://snomed.info/sct", "16076005", "Insulin"),
            Effective = new FhirDateTime(bolus.Time),
            Dosage = new MedicationAdministration.DosageComponent
            {
                Dose = new Quantity
                {
                    Value = bolus.Unit,
                    Unit = "units",
                    System = "http://unitsofmeasure.org",
                    Code = "U"
                }
            }
        };
    }

    public static Observation CreateCarbsValueObservation(CarbsValue carbsValue)
    {
        return new Observation
        {
            Status = ObservationStatus.Final, // "Final" since the intake has occurred
            Code = new CodeableConcept("http://loinc.org", "2339-0", "Glucose [Mass/volume] in Blood"),
            Value = new Quantity
            {
                Value = (decimal)carbsValue.Value,
                Unit = "grams",
                System = "http://unitsofmeasure.org",
                Code = "g"
            },
            Effective = new FhirDateTime(carbsValue.Time)
        };
    }

    public static Patient CreatePatient(Profile profile)
    {
        return new Patient
        {
            Id = profile.UserId,
            //BirthDate = profile.User.BirthDate.ToString("yyyy-MM-dd"),
            Name = new List<HumanName>
            {
                new HumanName
                {
                    //Family = profile.User.LastName,
                    //Given = new List<string> { profile.User.FirstName }
                }
            }
        };
    }

    public static Patient CreatePatient(ApplicationUser user)
    {
        var patient = new Patient();
        patient.Update(user);
        return patient;
    }

    public static void Update(this Patient patient, ApplicationUser user)
    {
        patient.SetEmail(user.Email ?? "");
    }

    public static Observation CreateProfileSettingObservation(ProfileSetting setting)
    {
        return new Observation
        {
            Status = ObservationStatus.Final,
            Code = new CodeableConcept("http://loinc.org", "30525-0", "Insulin basal rate"),
            Value = new Quantity
            {
                Value = setting.BasalRate,
                Unit = "units/hour",
                System = "http://unitsofmeasure.org",
                Code = "U/h"
            },
            Effective = new FhirDateTime(setting.ValidFrom.ToString("HH:mm:ss"))
        };
    }

    public static Communication CreateRecommendationCommunication(string patientId, string recommendation,
        DateTime issuedDate)
    {
        return new Communication
        {
            Status = EventStatus.Completed,
            Category = new List<CodeableConcept>
            {
                new CodeableConcept("http://terminology.hl7.org/CodeSystem/communication-category", "recommendation",
                    "Recommendation")
            },
            Subject = new ResourceReference($"Patient/{patientId}"),
            // Sent = issuedDate,
            SentElement = new FhirDateTime(issuedDate),
            Payload = new List<Communication.PayloadComponent>
            {
                new Communication.PayloadComponent
                {
                    Content = new FhirString(recommendation)
                }
            }
        };
    }
}