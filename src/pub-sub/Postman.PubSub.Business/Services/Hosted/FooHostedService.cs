using System.Transactions;
using Microsoft.Extensions.Logging;
using Sontiq.Queue.Exceptions;
using Sontiq.Queue.Extensions;
using Sontiq.Queue.PubSub.Business.Commands;

namespace Sontiq.Queue.PubSub.Business.Services.Hosted;

public class FooHostedService : HostedServiceBase
{
    readonly ILogger _logger;
    readonly IQueueListener _listener;
    readonly IFooService _fooService;
    
    public FooHostedService(ILogger<FooHostedService> logger,
        IQueueListener listener,
        IFooService fooService)
    {
        _logger = logger;
        _listener = listener;
        _fooService = fooService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting foo background service");
        await _listener.Subscribe<FooMessage>(Perform);
        
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping foo background service");
        await _listener.Unsubscribe<FooMessage>(Perform);
    }
    
    void Perform(FooMessage message)
    {
        using var logScope = _logger.BeginScope();
        _logger.LogInformation($"Handling background execution of user confirmation {message.Id}.");

        using var transactionScope = new TransactionScope(TransactionScopeOption.Required, 
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
        _fooService.Confirm(message.FooId, message.Name);
        transactionScope.Complete();
        _logger.LogInformation("Confirmation Completed.");
    }
}