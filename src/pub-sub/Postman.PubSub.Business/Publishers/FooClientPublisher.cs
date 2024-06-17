using Microsoft.Extensions.Logging;
using Sontiq.Queue.Extensions;
using Sontiq.Queue.PubSub.Business;
using Sontiq.Queue.PubSub.Business.Commands;

namespace Sontiq.Queue.Business.PubSub.Publishers
{
    public class FooClientPublisher : IFooClientPublisher
    {
        readonly ILogger _logger;
        readonly IQueuePublisher _publisher;

        public FooClientPublisher(ILogger<FooClientPublisher> logger, IQueuePublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public async Task Publish(FooMessage message)
        {
            using var logScope = _logger.BeginScope();
            _logger.LogInformation("About to start publishing the foo message");
            await _publisher.Publish(message);
            _logger.LogInformation($"Sent the confirmation message with id {message.Id}; foo-id {message.FooId}; name {message.Name}");
        }
    }
}
