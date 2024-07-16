using Postman;
using Postman.Exceptions;

namespace Postman;

/// <summary>
/// Base queue listener class, that offers out-of-the-box thread-safe subscribers management among other things.
/// </summary>
public abstract partial class QueueListenerBase<TConfig> : IQueueListener<TConfig>
{
    public bool IsConnected => isConnected;
    protected bool isDisposed = false;

    public abstract Task Connect(TConfig config, string name);
    public abstract Task Disconnect();
    protected abstract Task OnNewSubscription(Type messageType);
    protected abstract Task OnLastUnsubscription(Type messageType);

    #region Subscription

    public virtual Task Subscribe<T>(Action<T> callback)
        where T : class, IQueueMessage, new()
    {
        return DoSubscribe<T, T>(QueueListenerSubscriber.Create(callback));
    }

    public virtual Task Subscribe<T>(Func<T, Task> asyncCallback)
        where T : class, IQueueMessage, new()
    {
        return DoSubscribe<T, T>(QueueListenerSubscriber.Create(asyncCallback));
    }

    public virtual Task Subscribe<T, TProxy>(Action<TProxy> callback)
        where T : IQueueMessage
        where TProxy : class, new()
    {
        return DoSubscribe<T, TProxy>(QueueListenerSubscriber.Create(callback));
    }

    public virtual Task Subscribe<T, TProxy>(Func<TProxy, Task> asyncCallback)
        where T : IQueueMessage
        where TProxy : class, new()
    {
        return DoSubscribe<T, TProxy>(QueueListenerSubscriber.Create(asyncCallback));
    }

    #endregion

    #region Unsubscription

    public virtual Task Unsubscribe<T>(Action<T> callback)
        where T : class, IQueueMessage, new()
    {
        return DoUnsubscribe<T, T>(QueueListenerSubscriber.Create(callback));
    }

    public virtual Task Unsubscribe<T>(Func<T, Task> asyncCallback)
        where T : class, IQueueMessage, new()
    {
        return DoUnsubscribe<T, T>(QueueListenerSubscriber.Create(asyncCallback));
    }

    public virtual Task Unsubscribe<T, TProxy>(Action<TProxy> callback)
        where T : IQueueMessage
        where TProxy : class, new()
    {
        return DoUnsubscribe<T, TProxy>(QueueListenerSubscriber.Create(callback));
    }

    public virtual Task Unsubscribe<T, TProxy>(Func<TProxy, Task> asyncCallback)
        where T : IQueueMessage
        where TProxy : class, new()
    {
        return DoUnsubscribe<T, TProxy>(QueueListenerSubscriber.Create(asyncCallback));
    }

    #endregion

    #region Is subscribed

    public bool IsSubscribed<T>()
        where T : class, IQueueMessage, new()
    {
        return subscribers.ContainsKey(GetTypeKey(typeof(T)));
    }

    public bool IsSubscribed<T>(Action<T> callback)
        where T : class, IQueueMessage, new()
    {
        return IsSubscribed<T, T>(callback);
    }

    public bool IsSubscribed<T>(Func<T, Task> asyncCallback)
        where T : class, IQueueMessage, new()
    {
        return IsSubscribed<T, T>(asyncCallback);
    }

    #endregion

    public virtual async ValueTask DisposeAsync()
    {
        await UnsubscribeAll();
        await Disconnect();
    }

    protected void CheckConnection()
    {
        if (!isConnected)
            throw new QueueConnectionException("Connect to the server before calling this method");
    }

    protected string GetTypeKey(Type type)
    {
        return type.AssemblyQualifiedName;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
                DisposeAsync().AsTask().Wait();

            isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}