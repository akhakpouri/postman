using System.Transactions;
using Microsoft.Extensions.Logging;
using Postman.Extensions;
using Postman.PubSub.Business.Commands;

namespace Postman.PubSub.Business.Services.Hosted
{
    public class UserConfirmationService(
        ILogger<UserConfirmationService> logger,
        IQueueListener listener,
        IUserService userService)
        : HostedServiceBase
    {
        readonly ILogger _logger = logger;

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping user confirmation background service");
            await listener.Unsubscribe<UserConfirmMessage>(StopWorking);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting user confirmation background service");
            await listener.Subscribe<UserConfirmMessage>(Perform);
        }

        void StopWorking(UserConfirmMessage message)
        {
            //work
        }

        void Perform(UserConfirmMessage message)
        {
            using var logScope = _logger.BeginScope();
            _logger.LogInformation($"Handling background execution of user confirmation {message.Id}.");

            using var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);
            userService.Confirm(message.UserId, message.Email, message.Id);
            transactionScope.Complete();
            _logger.LogInformation("Confirmation Completed.");
        }
    }
}