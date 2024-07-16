using System.Reflection;
using Postman.Helpers;

namespace Postman.RabbitMq
{
    // <summary>
    /// <para>Standard router helper that setups routes for each event</para>
    /// <para>Idea is that all messages will go through same exchange.</para>
    /// 
    /// <para>Queues on the other hand are created one per group of same-type services (ie. cluster) so N instances of same service runtime shares one queue to distribute workload among them. 
    /// This can be optionally modified in configuration (<see cref="RabbitConfig.UseSeparateQueuesPerEvent"/>) so there will be one queue created per each type of event but still each of them will be shared by whole cluster of same type services (other cluster will have own set).</para>
    /// 
    /// <para>This RabbitMQ library uses Headers exchanges to route the messages to proper queues. On the listener side for each subscribed event queue is bound to exact type of that event. 
    /// Publisher on the other hand publishes messages with routing headers containing all parent types that can be casted to <seealso cref="IQueueMessage"/> by doing so it's possible to subscribe to range of events by their common interface.</para>
    /// 
    /// <para>All values are hashed with MurMur3 algorhitm to shrink the size of the messages for queue performance.</para>
    /// </summary>
    public class StandardRabbitMessageRouterHelper : IRabbitMessageRouterHelper
    {
        readonly TypeHelper typeHelper = new();

        public virtual string GetExchangeName(IQueueMessage message)
            => GetExchangeName(message.GetType());

        public virtual string GetExchangeName(Type messageType)
        {
            var name = messageType.FullName != null ? messageType.FullName.Split('.') : new string[] { "default" };
            return HashHelper.ToHashString(name[0]);
        }

        public string GetRoutingForListener(Type messageType)
            => HashHelper.ToHashString(messageType.FullName ?? "default-name");

        public IEnumerable<string> GetAllRoutingsForPublisher(Type messageType)
            => typeHelper.GetQueueMessageTypesFromHierarchy(messageType).Select(x => HashHelper.ToHashString(x.FullName ?? "default-name"));

        public virtual string GetQueueName(Type messageType, bool separatePerEvent, string extraName)
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                return HashHelper.ToHashString("default");


            var baseName = assembly.GetName().Name;
            if (baseName == null)
                return HashHelper.ToHashString("base");

            if (!string.IsNullOrEmpty(extraName))
            {
                baseName += "_" + extraName;
            }

            if (separatePerEvent)
            {
                baseName += "_" + messageType.FullName;
            }

            return HashHelper.ToHashString(baseName);
        }
    }
}
