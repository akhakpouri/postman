using System.Transactions;
using Microsoft.Extensions.Logging;
using Postman.Extensions;
using Postman.PubSub.Business.Commands;

namespace Postman.PubSub.Business.Services.Hosted;

public class FooHostedService(
    ILogger<FooHostedService> logger,
    IQueueListener listener,
    IFooService fooService)
    : HostedServiceBase
{
    readonly ILogger _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting foo background service");
        await listener.Subscribe<FooMessage>(Perform);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping foo background service");
        await listener.Unsubscribe<FooMessage>(Perform);
    }

    void Perform(FooMessage message)
    {
        using var logScope = _logger.BeginScope();
        _logger.LogInformation($"Handling background execution of user confirmation {message.Id}.");

        using var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        fooService.Confirm(message.FooId, message.Name);
        transactionScope.Complete();
        _logger.LogInformation("Confirmation Completed.");
    }
}