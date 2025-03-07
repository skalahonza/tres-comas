using Coravel.Invocable;

using TresComas.Services;

namespace TresComas.Invocables;

public class TidepoolBgValuesSyncInvocable(TidepoolBgValuesSyncService service) : IInvocable
{
    public async Task Invoke() => await service.DoSync();
}
