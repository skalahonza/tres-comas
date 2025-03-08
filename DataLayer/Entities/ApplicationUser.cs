using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FhirId { get; set; } = "";

    public const string DemoId = "5D7B57DE-5BDD-4477-A2CC-9069EDA7966A";
    public const string DemoPassword = "Demo1234!";
    public const string DemoUsername = "demo@example.com";
    
    public bool IsDemo => Id == DemoId;
    
    public static bool IsDemoUser(string id) => id == DemoId;

    public static ApplicationUser CreateDemoUser() =>
        new()
        {
            Id = DemoId,
            Email = DemoUsername,
            UserName = DemoUsername,
            FhirId = "1",
        };
}