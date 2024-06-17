namespace Sontiq.Queue
{
    /// <summary>
    /// Queue listener interface. Listeners are able to subscribe callbacks for concrete events.
    /// </summary>
    public interface IQueueListener : IAsyncDisposable, IDisposable
    {
        bool IsConnected { get; }

        Task Subscribe<T>(Action<T> callback)
            where T : class, IQueueMessage, new();

        Task Subscribe<T>(Func<T, Task> asyncCallback)
            where T : class, IQueueMessage, new();

        Task Subscribe<T, TProxy>(Action<TProxy> callback)
            where T : IQueueMessage
            where TProxy : class, new();

        Task Subscribe<T, TProxy>(Func<TProxy, Task> asyncCallback)
            where T : IQueueMessage
            where TProxy : class, new();

        Task Unsubscribe<T>(Action<T> callback)
            where T : class, IQueueMessage, new();

        Task Unsubscribe<T>(Func<T, Task> asyncCallback)
            where T : class, IQueueMessage, new();

        Task Unsubscribe<T, TProxy>(Action<TProxy> callback)
            where T : IQueueMessage
            where TProxy : class, new();

        Task Unsubscribe<T, TProxy>(Func<TProxy, Task> asyncCallback)
            where T : IQueueMessage
            where TProxy : class, new();

        Task UnsubscribeAll<T>()
            where T : class, IQueueMessage, new();

        Task UnsubscribeAll();

        bool IsSubscribed<T>()
            where T : class, IQueueMessage, new();

        bool IsSubscribed<T>(Action<T> callback)
            where T : class, IQueueMessage, new();

        bool IsSubscribed<T>(Func<T, Task> asyncCallback)
            where T : class, IQueueMessage, new();

        bool IsSubscribed<T, TProxy>(Action<TProxy> callback)
            where T : IQueueMessage
            where TProxy : class, new();

        bool IsSubscribed<T, TProxy>(Func<TProxy, Task> asyncCallback)
            where T : IQueueMessage
            where TProxy : class, new();

        Task Disconnect();
    }

    /// <summary>
    /// Queue listener interface. Listeners are able to subscribe callbacks for concrete events.
    /// </summary>
    public interface IQueueListener<in TConfig> : IQueueListener
    {
        Task Connect(TConfig config, string name);
    }
}
