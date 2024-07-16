using Microsoft.Extensions.Logging;
using Postman.Extensions;
using Postman.PubSub.Business.Commands;

namespace Postman.PubSub.Business.Publishers
{
    public class UserConfirmPublisher(ILogger<UserConfirmPublisher> logger, IQueuePublisher publisher)
        : IUserConfirmPublisher
    {
        readonly ILogger _logger = logger;

        public async Task Publish(UserConfirmMessage message)
        {
            using var logScope = _logger.BeginScope();
            _logger.LogInformation("About to start publishing the foo message");
            await publisher.Publish(message);
            _logger.LogInformation(
                $"Sent the confimation message with id {message.Id} & user-id {message.Id} & email: {message.Email}");
        }
    }
}