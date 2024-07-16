using Postman.PubSub.Business.Commands;
using Postman.PubSub.Business.Dto;

namespace Postman.PubSub.Business.Managers;

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