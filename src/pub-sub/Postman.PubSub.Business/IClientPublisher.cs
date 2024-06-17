namespace Sontiq.Queue.PubSub.Business
{
    public interface IClientPublisher<TMessage> where TMessage : QueueMessage
    {
        Task Publish(TMessage message);
    }
}
