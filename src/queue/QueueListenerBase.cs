using ConcurrentCollections;
using Sontiq.Queue.Codecs;
using Sontiq.Queue.Exceptions;
using Sontiq.Queue.Helpers;
using System.Collections.Concurrent;

namespace Sontiq.Queue
{
    public abstract partial class QueueListenerBase<TConfig> : IQueueListener<TConfig>
    {
        protected readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, ConcurrentHashSet<QueueListenerSubscriber>>> subscribers = new();
        private readonly TypeHelper typeHelper = new();
        protected volatile bool isConnected;

        protected virtual async Task DoSubscribe<T, TProxy>(QueueListenerSubscriber subscriber)
        {
            var type = typeof(T);
            var tproxy = typeof(TProxy);
            var createdNew = false;

            var byType = subscribers.GetOrAdd(typeHelper.EncodeType(type),
                _ =>
                { createdNew = true; return new ConcurrentDictionary<Type, ConcurrentHashSet<QueueListenerSubscriber>>(); })
                ?? throw new QueueListenerSubscibeException($"Problem with creating or obtaining list of subscribers for given type [{typeHelper.EncodeType(type)}].");
            var byProxyType = byType.GetOrAdd(tproxy,
                _ => new ConcurrentHashSet<QueueListenerSubscriber>())
                ?? throw new QueueListenerSubscibeException($"Problem with creating or obtaining list of subscribers for given proxy type [{typeHelper.EncodeType(tproxy)}].");

            if (!byProxyType.Add(subscriber))
                throw new QueueListenerSubscibeException("Subscriber was not added to the list");

            if (createdNew)
                await OnNewSubscription(type);
        }

        protected virtual async Task DoUnsubscribe<T, TProxy>(QueueListenerSubscriber subscriber)
        {
            var type = typeof(T);
            var tproxy = typeof(TProxy);

            if (subscribers.TryGetValue(typeHelper.EncodeType(type), out var byType))
                if (byType.TryGetValue(tproxy, out var byProxyType))
                {
                    var toRemove = byProxyType.FirstOrDefault(x => x.HasSameCallback(subscriber));
                    if (toRemove != null)
                        if (byProxyType.TryRemove(toRemove))
                            if (byProxyType.IsEmpty && byType.Remove(tproxy, out var _) && byType.IsEmpty && subscribers.TryRemove(typeHelper.EncodeType(type), out var _))
                                await OnLastUnsubscription(type);
                            else
                                throw new QueueListenerSubscibeException("Subscriber was not removed from the list");
                        else
                            throw new QueueListenerUnsubscibeException($"Given callback wasn't subscribed at all");
                }
                else
                    throw new QueueListenerUnsubscibeException($"Subscription list for given proxy type [{typeHelper.EncodeType(type)}] doesn't exist");
            else
                throw new QueueListenerUnsubscibeException($"Subscription list for given type [{typeHelper.EncodeType(type)}] doesn't exist");
        }

        public virtual async Task UnsubscribeAll<T>()
            where T : class, IQueueMessage, new()
        {
            var type = typeof(T);
            await OnLastUnsubscription(type);
            subscribers.Remove(GetTypeKey(type), out var _);
        }

        public virtual async Task UnsubscribeAll()
        {
            foreach (var type in subscribers.Keys.ToArray())
            {
                await OnLastUnsubscription(Type.GetType(type));
                subscribers.Remove(type, out var _);
            }
        }

        public bool IsSubscribed<T, TProxy>(Action<TProxy> callback)
            where T : IQueueMessage
            where TProxy : class, new()
        {
            return subscribers.TryGetValue(typeHelper.EncodeType(typeof(T)), out var byType) &&
                    byType.TryGetValue(typeof(TProxy), out var byProxyType) &&
                    byProxyType.Any(x => x.HasSameCallback(callback));
        }

        public bool IsSubscribed<T, TProxy>(Func<TProxy, Task> asyncCallback)
            where T : IQueueMessage
            where TProxy : class, new()
        {
            return subscribers.TryGetValue(typeHelper.EncodeType(typeof(T)), out var byType) &&
                    byType.TryGetValue(typeof(TProxy), out var byProxyType) &&
                    byProxyType.Any(x => x.HasSameCallback(asyncCallback));
        }

        protected virtual async Task BroadcastMessage(byte[] data, IQueueMessageDecoder decoder)
        {
            using var ms = new MemoryStream(data);
            await BroadcastMessage(ms, decoder);
        }

        protected virtual async Task BroadcastMessage(Stream data, IQueueMessageDecoder decoder)
        {
            await decoder.Decode(data, async (typeNames, decodeCallback) =>
            {
                foreach (var typeName in typeNames)
                    if (subscribers.TryGetValue(typeName, out var byType))
                        foreach (var kvp in byType)
                        {
                            var msg = await decodeCallback(kvp.Key);
                            foreach (var sub in kvp.Value)
                                await sub.Invoke(msg);
                        }
            });
        }
    }
}
