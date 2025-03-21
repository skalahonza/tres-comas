using Coravel;

using DataLayer;
using DataLayer.Entities;
using Dexcom;
using FHIR.Extensions;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using Syncfusion.Blazor;

using Tidepool.Extensions;

using TresComas.Components;
using TresComas.Components.Account;
using TresComas.Invocables;
using TresComas.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetConnectionString("SyncfusionLicense"));
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDataLayer(builder.Configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTidepoolClient((settings, configuration) => configuration.GetSection("Tidepool").Bind(settings));
builder.Services.AddDexcom((settings, configuration) => configuration.GetSection("Dexcom").Bind(settings));
builder.Services.AddTransient<TidepoolBgValuesSyncService>();
builder.Services.AddScheduler();
builder.Services.AddSyncfusionBlazor();
builder.AddFhir();
builder.Services.AddScoped<UserProvider>();
builder.Services.AddScoped<TidepoolCoreSyncService>();
builder.Services.AddScoped<DexcomCoreSyncService>();
builder.Services.AddScoped<TotalDataSyncService>();
builder.Services.AddScoped<DashboardDataService>();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


// Add health checks
builder.Services.AddHealthChecks();

builder.Services.UseMinimalHttpLogger();

// bg workers registration
builder.Services.AddTransient<TidepoolBgValuesSyncInvocable>();
builder.Services.AddTransient<TidepoolBolusValuesSyncInvocable>();
builder.Services.AddTransient<TidepoolCarbsValuesSyncInvocable>();
builder.Services.AddTransient<TidepoolProfileSyncInvocable>();
builder.Services.AddTransient<DexcomBgValuesSyncInvocable>();
builder.Services.AddTransient<FhirSync>();

var app = builder.Build();

// bg workers scheduling
app.Services.UseScheduler(s =>
{
    s.Schedule<TidepoolBgValuesSyncInvocable>().Hourly().RunOnceAtStart();
    s.Schedule<TidepoolBolusValuesSyncInvocable>().Hourly().RunOnceAtStart();
    s.Schedule<TidepoolCarbsValuesSyncInvocable>().Hourly().RunOnceAtStart();
    s.Schedule<TidepoolProfileSyncInvocable>().Hourly().RunOnceAtStart();
    s.Schedule<DexcomBgValuesSyncInvocable>().Hourly().RunOnceAtStart();
    s.Schedule<FhirSync>().Hourly().RunOnceAtStart();
});

// Add health check endpoint
app.UseHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts(); // we are using traefik
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();


await MigrateAsync();
await EnsureDemoUserExists();

app.Run();
return;


async Task MigrateAsync()
{
    using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    await using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

async Task EnsureDemoUserExists()
{
    using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var demoUser = await userManager.FindByIdAsync(ApplicationUser.DemoId);
    if (demoUser is null)
    {
        demoUser = ApplicationUser.CreateDemoUser();
        await userManager.CreateAsync(demoUser);
    }

    // set password
    var token = await userManager.GeneratePasswordResetTokenAsync(demoUser);
    await userManager.ResetPasswordAsync(demoUser, token, ApplicationUser.DemoPassword);

    // confirm email
    token = await userManager.GenerateEmailConfirmationTokenAsync(demoUser);
    await userManager.ConfirmEmailAsync(demoUser, token);
}