using Microsoft.Extensions.Logging;
using Postman.Extensions;
using Postman.PubSub.Business.Commands;

namespace Postman.PubSub.Business.Publishers
{
    public class FooClientPublisher(ILogger<FooClientPublisher> logger, IQueuePublisher publisher)
        : IFooClientPublisher
    {
        readonly ILogger _logger = logger;

        public async Task Publish(FooMessage message)
        {
            using var logScope = _logger.BeginScope();
            _logger.LogInformation("About to start publishing the foo message");
            await publisher.Publish(message);
            _logger.LogInformation($"Sent the confirmation message with id {message.Id}; foo-id {message.FooId}; name {message.Name}");
        }
    }
}
