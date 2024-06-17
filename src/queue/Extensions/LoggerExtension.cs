using Microsoft.Extensions.Logging;

namespace Sontiq.Queue.Extensions
{
    public static class LoggerExtension
    {
        public static IDisposable BeginScope(this ILogger logger)
            => logger.BeginScope(Guid.NewGuid());
    }
}
