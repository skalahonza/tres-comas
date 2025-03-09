using Coravel.Invocable;
using DataLayer;
using DataLayer.Entities;
using FHIR;
using FHIR.Client;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace TresComas.Invocables;

public class FhirSync(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<FhirSync> logger,
    FhirClientFactory fhirClientFactory) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogInformation("FHIR Sync started");
        var fhir = fhirClientFactory.Client;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var users = await dbContext.Users.ToListAsync();
        var patients = await SyncPatients(users, fhir, dbContext);
        await SyncBgValues(users.ToDictionary(x => x.FhirId), fhir, dbContext);
        await SyncBoluses(users.ToDictionary(x => x.FhirId), fhir, dbContext);
    }
    
    public async Task DeleteValues(string patientId)
    {
        await RemoveObservations(patientId, fhirClientFactory.Client);
    }

    private async Task<List<Patient>> SyncPatients(List<ApplicationUser> users, FhirClient fhir,
        ApplicationDbContext dbContext)
    {
        logger.LogInformation("Syncing {UserCount} patients", users.Count);
        var patients = new List<Patient>();
        foreach (var user in users)
        {
            if (!string.IsNullOrEmpty(user.FhirId))
            {
                var patient = await fhir.ReadAsync<Patient>($"Patient/{user.FhirId}");
                if (patient is null)
                {
                    logger.LogError("Patient with ID {UserFhirId} not found", user.FhirId);
                    continue;
                }

                patient.Update(user);
                await fhir.UpdateAsync(patient);
                patients.Add(patient);
            }
            else
            {
                var patient = Mappings.CreatePatient(user);
                var response = await fhir.CreateAsync(patient);

                if (response is null)
                {
                    logger.LogError("Create Patient with ID {UserFhirId} failed", user.FhirId);
                    continue;
                }

                user.FhirId = response.Id;
                await dbContext.SaveChangesAsync();
                patients.Add(patient);
            }
        }

        logger.LogInformation("Synced {PatientCount} patients", patients.Count);
        return patients;
    }

    /// <summary>
    /// Sync Blood Glucose values to FHIR server as Observations
    /// </summary>
    /// <param name="users">Users lookup, key is FHIR ID</param>
    private async Task SyncBgValues(Dictionary<string, ApplicationUser> users,
        FhirClient fhir,
        ApplicationDbContext dbContext)
    {
        logger.LogInformation("Syncing BG values for {UserCount} users", users.Count);
        foreach (var user in users.Values)
        {
            logger.LogInformation("Syncing BG values for {UserEmail}", user.Email);
            var date = DateTime.UtcNow.AddDays(-7);
            var bgValues = await dbContext.BgValues
                .Where(x => x.UserId == user.Id)
                // last week only
                .Where(x => x.Time >= date)
                .Where(x => string.IsNullOrEmpty(x.FhirId))
                .ToListAsync();
            
            logger.LogInformation("Found {BgValueCount} BG values for {UserEmail}", bgValues.Count, user.Email);
            if (bgValues.Count == 0)
            {
                continue;
            }
            
            var observations = bgValues.Select(x => x.CreateObservation(user.FhirId)).ToList();

            // Create a FHIR Bundle of type Transaction
            var bundle = new Bundle
            {
                Type = Bundle.BundleType.Transaction,
                Entry = new List<Bundle.EntryComponent>()
            };

            // Add Observations to the Bundle
            foreach (var obs in observations)
            {
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    Resource = obs,
                    Request = new Bundle.RequestComponent
                    {
                        Method = Bundle.HTTPVerb.POST,
                        Url = "Observation"
                    }
                });
            }

            // Send the transaction Bundle to FHIR server
            try
            {
                var responseBundle = await fhir.TransactionAsync(bundle);
                logger.LogInformation("Batch insert successful! Response Bundle ID: {ResponseBundleId}", responseBundle!.Id);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting batch observations");
            }
        }
    }

    private async Task SyncBoluses(Dictionary<string, ApplicationUser> users,
        FhirClient fhir,
        ApplicationDbContext dbContext)
    {
        logger.LogInformation("Syncing boluses for {UserCount} users", users.Count);
        foreach (var user in users.Values)
        {
            logger.LogInformation("Syncing boluses for {UserEmail}", user.Email);
            var date = DateTime.UtcNow.AddDays(-7);
            var bolusValues = await dbContext.BolusValues
                .Where(x => x.UserId == user.Id)
                // last week only
                .Where(x => x.Time >= date)
                .Where(x => string.IsNullOrEmpty(x.FhirId))
                .ToListAsync();
            
            logger.LogInformation("Found {BgValueCount} boluses for {UserEmail}", bolusValues.Count, user.Email);
            if (bolusValues.Count == 0)
            {
                continue;
            }
            
            var administrations = bolusValues.Select(x => x.CreateMedicationAdministration(user.FhirId)).ToList();

            // Create a FHIR Bundle of type Transaction
            var bundle = new Bundle
            {
                Type = Bundle.BundleType.Transaction,
                Entry = new List<Bundle.EntryComponent>()
            };

            // Add Administrations to the Bundle
            foreach (var administration in administrations)
            {
                bundle.Entry.Add(new Bundle.EntryComponent
                {
                    Resource = administration,
                    Request = new Bundle.RequestComponent
                    {
                        Method = Bundle.HTTPVerb.POST,
                        Url = "MedicationAdministration"
                    }
                });
            }

            // Send the transaction Bundle to FHIR server
            try
            {
                var responseBundle = await fhir.TransactionAsync(bundle);
                logger.LogInformation("Batch insert successful! Response Bundle ID: {ResponseBundleId}", responseBundle!.Id);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting batch observations");
            }
        }
    }
    
    private async Task RemoveObservations(string patientId, FhirClient fhir)
    {
        var count = 1;
        while (count > 0)
        {
            try
            {
                // Search for Observations linked to the Patient
                var q = new SearchParams()
                    .Where($"subject=Patient/{patientId}")
                    .LimitTo(1000);
                q.Count = 100;
                
                var searchResult = await fhir.SearchAsync<Observation>(q);

                if (searchResult is null || searchResult.Entry.Count == 0)
                {
                    Console.WriteLine("No observations found for the patient.");
                    return;
                }

                count = searchResult.Entry.Count;
                Console.WriteLine($"Found {searchResult.Entry.Count} observations. Deleting...");
                
                if (count == 0)
                {
                    break;
                }

                // Collect Observation IDs
                var observationIds = new List<string>();
                foreach (var entry in searchResult.Entry)
                {
                    if (entry.Resource is Observation observation)
                    {
                        observationIds.Add(observation.Id);
                    }
                }

                // Delete observations using a Transaction Bundle (for efficiency)
                var bundle = new Bundle
                {
                    Type = Bundle.BundleType.Transaction,
                    Entry = []
                };

                foreach (var obsId in observationIds)
                {
                    bundle.Entry.Add(new Bundle.EntryComponent
                    {
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.DELETE,
                            Url = $"Observation/{obsId}"
                        }
                    });
                }

                // Execute batch delete
                await fhir.TransactionAsync(bundle);
                logger.LogInformation("Batch deletion completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting observations");
            }
        }
    }
}