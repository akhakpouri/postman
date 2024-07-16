using Microsoft.Extensions.Logging;

namespace Postman.Extensions
{
    public static class LoggerExtension
    {
        public static IDisposable? BeginScope(this ILogger logger)
            => logger.BeginScope(Guid.NewGuid());
    }
}
