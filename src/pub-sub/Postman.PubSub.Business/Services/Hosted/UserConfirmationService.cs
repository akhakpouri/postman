using Microsoft.Extensions.Logging;
using Sontiq.Queue.Extensions;
using Sontiq.Queue.PubSub.Business.Commands;
using System.Transactions;

namespace Sontiq.Queue.PubSub.Business.Services.Hosted
{
    public class UserConfirmationService : HostedServiceBase
    {
        readonly ILogger _logger;
        readonly IQueueListener _listener;
        readonly IUserService _userService;

        public UserConfirmationService(
            ILogger<UserConfirmationService> logger,
            IQueueListener listener,
            IUserService userService)
        {
            _logger = logger;
            _listener = listener;
            _userService = userService;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping user confirmation background service");
            await _listener.Unsubscribe<UserConfirmMessage>(StopWorking);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting user confirmation background service");
            await _listener.Subscribe<UserConfirmMessage>(Perform);
        }

        void StopWorking(UserConfirmMessage message)
        {
            //work
        }

        void Perform(UserConfirmMessage message)
        {
            using var logScope = _logger.BeginScope();
            _logger.LogInformation($"Handling background execution of user confirmation {message.Id}.");

            using var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
            _userService.Confirm(message.UserId, message.Email, message.Id);
            transactionScope.Complete();
            _logger.LogInformation("Confirmation Completed.");
        }
    }
}
