using Postman.Exceptions;

namespace Postman;

/// <summary>
/// Base queue publisher class, that offers some basic features for convenience.
/// </summary>
/// <typeparam name="TConfig"></typeparam>
abstract public class QueuePublisherBase<TConfig> : IQueuePublisher<TConfig>
{
    private bool isDisposed = false;
    protected volatile bool isConnected;
    public bool IsConnected => isConnected;

    abstract public Task Connect(TConfig config);
    abstract public Task Disconnect();
    abstract public Task Publish(IQueueMessage message);

    public virtual async ValueTask DisposeAsync()
    {
        await Disconnect();
    }

    protected void CheckConnection()
    {
        if (!isConnected)
        {
            throw new QueueConnectionException("Connect to the server before calling this method");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                DisposeAsync().AsTask().Wait();
            }

            isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}