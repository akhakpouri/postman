namespace Postman;

public class QueueListenerSubscriber
{
    protected object OriginalCallback { get; set; } = new();
    protected Action<object> WrappedCallback { get; set; }
    protected Func<object, Task> WrappedAsyncCallback { get; set; }

    private QueueListenerSubscriber() { }

    public static QueueListenerSubscriber Create<T>(Action<T> originalCallback)
        where T : class, new()
    {
        return new QueueListenerSubscriber()
        {
            OriginalCallback = originalCallback,
            WrappedCallback = (message) => originalCallback.Invoke((T)message)
        };
    }

    public static QueueListenerSubscriber Create<T>(Func<T, Task> originalAsyncCallback)
        where T : class, new()
    {
        return new QueueListenerSubscriber()
        {
            OriginalCallback = originalAsyncCallback,
            WrappedAsyncCallback = (message) => originalAsyncCallback.Invoke((T)message)
        };
    }

    public async Task Invoke(object message)
    {
        if (WrappedAsyncCallback != null)
        {
            await WrappedAsyncCallback(message);
        }
        else
        {
            WrappedCallback.Invoke(message);
        }
    }

    public bool HasSameCallback(QueueListenerSubscriber other) => OriginalCallback.Equals(other?.OriginalCallback);

    public bool HasSameCallback(object callback) => OriginalCallback.Equals(callback);

    public override bool Equals(object obj) => obj is QueueListenerSubscriber other && OriginalCallback.Equals(other.OriginalCallback);

    public override int GetHashCode() => OriginalCallback.GetHashCode();

    public override string? ToString() => OriginalCallback?.ToString();
}