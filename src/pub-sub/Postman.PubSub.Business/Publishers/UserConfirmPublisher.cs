using Microsoft.Extensions.Logging;
using Sontiq.Queue.Extensions;
using Sontiq.Queue.PubSub.Business.Commands;

namespace Sontiq.Queue.PubSub.Business.Publishers
{
    public class UserConfirmPublisher : IUserConfirmPublisher
    {
        readonly ILogger _logger;
        readonly IQueuePublisher _publisher;

        public UserConfirmPublisher(ILogger<UserConfirmPublisher> logger, IQueuePublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public async Task Publish(UserConfirmMessage message)
        {
            using var logScope = _logger.BeginScope();
            _logger.LogInformation("About to start publishing the foo message");
            await _publisher.Publish(message);
            _logger.LogInformation($"Sent the confimation message with id {message.Id} & user-id {message.Id} & email: {message.Email}");
        }
    }
}
