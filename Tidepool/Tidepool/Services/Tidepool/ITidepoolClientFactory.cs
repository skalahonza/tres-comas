namespace Tidepool.Services.Tidepool;

public interface ITidepoolClientFactory
{
    Task<ITidepoolClient> CreateAsync();
}