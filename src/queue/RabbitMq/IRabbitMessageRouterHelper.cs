namespace Sontiq.Queue.RabbitMq
{
    /// <summary>
    /// Interface for classes that resolves routing for given input messages.
    /// </summary>
    public interface IRabbitMessageRouterHelper
    {
        string GetExchangeName(Type messageType);
        string GetRoutingForListener(Type messageType);
        IEnumerable<string> GetAllRoutingsForPublisher(Type messageType);
        string GetQueueName(Type messageType, bool separatePerEvent, string extraName);
    }
}
