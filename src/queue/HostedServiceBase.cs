using Microsoft.Extensions.Hosting;

namespace Sontiq.Queue
{
    public abstract class HostedServiceBase : BackgroundService
    {
        public override abstract Task StopAsync(CancellationToken cancellationToken);
    }
}
