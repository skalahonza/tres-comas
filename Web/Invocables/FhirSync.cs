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
        await SyncPatients(users, fhir, dbContext);
    }

    private async Task SyncPatients(List<ApplicationUser> users, FhirClient fhir, ApplicationDbContext dbContext)
    {
        logger.LogInformation("Syncing {UserCount} patients", users.Count);
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
            }
        }
    }
}
