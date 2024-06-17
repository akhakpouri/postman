using Microsoft.Extensions.Logging;
using Sontiq.Queue.PubSub.Business.Commands;

namespace Sontiq.Queue.PubSub.Business.Services;

public class FooService : IFooService
{
    readonly ILogger _logger;

    public FooService(ILogger<FooService> logger)
    {
        _logger = logger;
    }
    
    public void Confirm(int id, string name)
    {
        _logger.LogInformation($"Foo Service received the user info. \n id: {id}, name: {name}");
    }
}