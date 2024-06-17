using Sontiq.Queue.PubSub.Business.Commands;
using Sontiq.Queue.PubSub.Business.Dto;

namespace Sontiq.Queue.PubSub.Business.Managers;

public class FooManager : IManager<FooDto>
{
    readonly IFooClientPublisher _publisher;

    public FooManager(IFooClientPublisher publisher)
    {
        _publisher = publisher;
    }
    
    public async Task Process(FooDto dto)
    {
        var message = new FooMessage { FooId = dto.Id, Name = dto.Name };
        await _publisher.Publish(message);
    }
}