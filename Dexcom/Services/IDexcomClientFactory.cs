namespace Dexcom.Services;

public interface IDexcomClientFactory
{
    Task<IDexcomClient> Create(string authCode);
}
