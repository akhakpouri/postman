using Postman.PubSub.Business.Commands;

namespace Postman.PubSub.Business;

public interface IFooClientPublisher : IClientPublisher<FooMessage>;