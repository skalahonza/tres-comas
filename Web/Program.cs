using Coravel;

using DataLayer;
using DataLayer.Entities;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using Tidepool.Extensions;

using TresComas.Components;
using TresComas.Components.Account;
using TresComas.Invocables;
using TresComas.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add MudBlazor services
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

builder.Services.AddDataLayer(configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTidepoolClient((settings, configuration) => configuration.GetSection("Tidepool").Bind(settings));
builder.Services.AddTransient<TidepoolBgValuesSyncInvocable>();
builder.Services.AddTransient<TidepoolBgValuesSyncService>();
builder.Services.AddScheduler();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

//app.Services.UseScheduler(s => s.Schedule<TidepoolBgValuesSyncInvocable>().Hourly());

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

app.Run();


async Task MigrateAsync()
{
    using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}