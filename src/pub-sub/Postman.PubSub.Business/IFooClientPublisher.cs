using Sontiq.Queue.PubSub.Business.Commands;

namespace Sontiq.Queue.PubSub.Business
{
    public interface IFooClientPublisher : IClientPublisher<FooMessage> { }
}
