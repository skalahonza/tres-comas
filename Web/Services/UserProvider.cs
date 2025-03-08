using DataLayer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TresComas.Services;

public class UserProvider(
    AuthenticationStateProvider authStateProvider,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<string?> GetCurrentUserId()
    {
        var state = await authStateProvider.GetAuthenticationStateAsync();
        var username = state.User.Identity?.Name ?? "";
        if (string.IsNullOrEmpty(username))
        {
            return null;
        }
        
        await using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Users.Where(u => u.UserName == username).Select(u => u.Id).FirstOrDefaultAsync();
    }
}