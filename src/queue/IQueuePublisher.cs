namespace Sontiq.Queue
{
    /// <summary>
    /// Interface for queue publishers. Publishers are responsible for sending message to the messageing queue.
    /// </summary>
    public interface IQueuePublisher : IAsyncDisposable, IDisposable
    {
        bool IsConnected { get; }

        Task Publish(IQueueMessage message);

        Task Disconnect();
    }

    /// <summary>
    /// Interface for queue publishers. Publishers are responsible for sending message to the messageing queue.
    /// </summary>
    public interface IQueuePublisher<in TConfig> : IQueuePublisher
    {
        Task Connect(TConfig config);
    }
}
