using Microsoft.Extensions.Hosting;

namespace Postman;

public abstract class HostedServiceBase : BackgroundService
{
    public override abstract Task StopAsync(CancellationToken cancellationToken);
}