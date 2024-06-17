using ConcurrentCollections;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sontiq.Queue.Codecs;
using Sontiq.Queue.Exceptions;
using Sontiq.Queue.Extensions;
using System.Collections.Concurrent;

namespace Sontiq.Queue.RabbitMq
{
    public class RabbitMessageQueueListener : QueueListenerBase<RabbitConfig>
    {
        protected struct QueueInfo
        {
            public string ConsumerTag { get; set; }
            public ConcurrentHashSet<string> RoutingKeys { get; set; }

            public QueueInfo(string consumerTag, ConcurrentHashSet<string>? routingKeys = null)
            {
                ConsumerTag = consumerTag;
                RoutingKeys = routingKeys ?? new ConcurrentHashSet<string>();
            }
        }

        private protected readonly RabbitMqConnector connector = new();

        protected readonly ConcurrentHashSet<string> createdExchanges = new();
        protected readonly ConcurrentDictionary<string, QueueInfo> createdQueues = new ConcurrentDictionary<string, QueueInfo>();
        protected readonly IQueueMessageDecoder decoder;
        protected readonly ILogger logger;
        protected readonly IRabbitMessageRouterHelper routerHelper;
        protected RabbitConfig? config;
        protected AsyncEventingBasicConsumer? consumer;
        protected string listenerName = string.Empty;

        public RabbitMessageQueueListener(ILogger<RabbitMessageQueueListener> logger, IRabbitMessageRouterHelper routerHelper, IQueueMessageDecoder messageDecoder)
        {
            this.logger = logger;
            this.routerHelper = routerHelper;
            this.decoder = messageDecoder;
        }

        public override Task Connect(RabbitConfig config, string name)
        {
            try
            {
                this.config = config;
                this.listenerName = name;
                connector.Connect(config);
                consumer = new AsyncEventingBasicConsumer(connector.Channel);
                consumer.Received += Consumer_Received;

                logger.LogInformation("Listener successfully connected to the server. Extra name: {ExtraName}.", listenerName ?? "-");

                isConnected = true;

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while trying to connect to listener to server: {ExceptionMessage}.", e.Message);
                throw;
            }
        }

        public override Task Disconnect()
        {
            try
            {
                if (isConnected)
                {
                    this.config = null;
                    if (consumer != null)
                    {
                        consumer.Received -= Consumer_Received;
                        consumer = null;
                    }
                    connector.Disconnect();
                    logger.LogInformation("Listener successfully disconnected from the server.");
                    isConnected = false;

                }

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while trying to disconnect listener from server: {ExceptionMessage}.", e.Message);
                throw;
            }
        }

        protected override Task OnNewSubscription(Type messageType)
        {
            CheckConnection();

            var exchangeName = routerHelper.GetExchangeName(messageType);
            var deadLetterExchangeName = $"{exchangeName}.dead-letter";
            var routingHeader = routerHelper.GetRoutingForListener(messageType);

            ArgumentNullException.ThrowIfNull(config, nameof(config));
            var queueName = routerHelper.GetQueueName(messageType, config.UseSeparateQueuesPerEvent, listenerName);
            using (logger.BeginScope())
            {

                // Call declare only if we didn't yet for this exchange.
                // It's ok that multiple services in cluster will do that as final thread-safety is up to RabbitMQ server, here we just try to avoid too many calls.
                if (createdExchanges.Add(exchangeName))
                {
                    try
                    {
                        ArgumentNullException.ThrowIfNull(connector.Channel, nameof(connector.Channel));
                        connector.Channel.ExchangeDeclare(exchangeName, ExchangeType.Headers, config.DurableExchanges, config.AutoDeleteExchanges, null);

                        if (config.UseDeadLetterExchanges)
                        {
                            connector.Channel.ExchangeDeclare(deadLetterExchangeName, ExchangeType.Headers, config.DurableExchanges, config.AutoDeleteExchanges, null);
                            logger.LogDebug("Declared new exchange {ExchangeName} with dead letter {DeadLetterExchangeName}.", exchangeName, deadLetterExchangeName);
                        }
                        else
                        {
                            logger.LogDebug("Declared new exchange {ExchangeName} without dead letter.", exchangeName);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error while declaring new exchange {ExchangeName}: {ExceptionMessage}.", exchangeName, e.Message);
                        throw;
                    }
                }

                // If we haven't yet declared this queue (and so we're not listening to it) - do it now
                var queueRecord = createdQueues.GetOrAdd(queueName, _ =>
                {
                    try
                    {
                        var args = new Dictionary<string, object>();

                        if (config.UseDeadLetterExchanges)
                        {
                            args.Add("x-dead-letter-exchange", deadLetterExchangeName);
                        }

                        ArgumentNullException.ThrowIfNull(connector, nameof(connector));
                        ArgumentNullException.ThrowIfNull(connector.Channel, nameof(connector.Channel));

                        connector.Channel.QueueDeclare(queueName, config.DurableQueues, false, config.AutoDeleteQueues, args);
                        var output = new QueueInfo(connector.Channel.BasicConsume(queueName, false, consumer), new ConcurrentHashSet<string>());
                        logger.LogDebug("Declared new queue {QueueName}.", queueName);

                        return output;
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error while declaring new queue {QueueName} on exchange {ExchangeName}: {ExceptionMessage}", queueName, exchangeName, e.Message);
                        throw;
                    }
                });

                // bind the queue to the routing key if necessary.
                if (queueRecord.RoutingKeys.Add(routingHeader))
                {
                    try
                    {
                        var props = new Dictionary<string, object>
                    {
                        { "x-match", "any" },
                        { $"route-{routingHeader}", string.Empty }
                    };
                        ArgumentNullException.ThrowIfNull(connector.Channel, nameof(connector.Channel));
                        connector.Channel.QueueBind(queueName, exchangeName, string.Empty, props);
                        logger.LogDebug("Queue {QueueName} was bound to routing {Routing}.", queueName, routingHeader);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error while bounding queue {QueueName} on exchange {ExchangeName} to routing {Routing}: {ExceptionMessage}",
                            queueName, exchangeName, routingHeader, e.Message);
                    }
                }

                logger.LogInformation("Listener successfully subscribed for events of type {EventType} to exchange {ExchangeName} and queue {QueueName} with routing {Routing}.",
                    messageType.FullName, exchangeName, queueName, routingHeader);

                return Task.CompletedTask;
            }
        }

        protected override Task OnLastUnsubscription(Type messageType)
        {
            CheckConnection();

            var exchangeName = routerHelper.GetExchangeName(messageType);
            var routingHeader = routerHelper.GetRoutingForListener(messageType);
            
            ArgumentNullException.ThrowIfNull(config, nameof(config));
            var queueName = routerHelper.GetQueueName(messageType, config.UseSeparateQueuesPerEvent, listenerName);

            // Here we're leaving exchange and queue intact, as others can still be using it (it's up to server to remove any unnecessary objects).
            // We're just canceling the consumer listener for that queue if and only if there are no more routing keys we are listening for here

            if (createdQueues[queueName].RoutingKeys.TryRemove(routingHeader) && createdQueues[queueName].RoutingKeys.IsEmpty && createdQueues.TryRemove(queueName, out QueueInfo info))
            {
                try
                {
                    ArgumentNullException.ThrowIfNull(connector.Channel, nameof(connector.Channel));
                    connector.Channel.BasicCancel(info.ConsumerTag);
                    logger.LogInformation("Listener successfuly unsubscribed from events of type {EventType} in exchange {ExchangeName} and queue {QueueName} with routing key {RoutingKey}.",
                        messageType.FullName, exchangeName, queueName, routingHeader);
                }
                catch (Exception e)
                {
                    logger.LogInformation(e, "Error while unsubscribing from events of type {EventType} in exchange {ExchangeName} and queue {QueueName} with routing key {RoutingKey}: {ExceptionMessage}",
                        messageType.FullName, exchangeName, queueName, routingHeader, e.Message);
                    throw;
                }
            }

            return Task.CompletedTask;
        }

        protected virtual async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            logger.LogTrace("New message received. Exchange: {Exchange}; Routing key: {RoutingKey}; Delivery tag: {DeliveryTag}; Consumer tag: {ConsumerTag}; Body length: {BodyLength}.",
                @event.Exchange, @event.RoutingKey, @event.DeliveryTag, @event.ConsumerTag, @event.Body.Length);

            var model = (sender as AsyncEventingBasicConsumer)?.Model ?? (sender as IModel);

            if (model == null)
                return;

            try
            {
                await BroadcastMessage(@event.Body.ToArray(), decoder);
                model.BasicAck(@event.DeliveryTag, false);

                logger.LogTrace("Message consumed. Exchange: {Exchange}; Routing key: {RoutingKey}; Delivery tag: {DeliveryTag}; Consumer tag: {ConsumerTag}; Body length: {BodyLength}.", @event.Exchange, @event.RoutingKey, @event.DeliveryTag, @event.ConsumerTag, @event.Body.Length);
            }
            catch (Exception e) when (e is ThreadAbortException || (e is QueueMessageProcessingException qmpe && qmpe.IsRecoverable))
            {
                // If thread was aborted or this is recoverable exception, then let's requeue this message, so other workers can pick it up and try again.
                logger.LogWarning("Recoverable error while message was processed, requeuing. {ExceptionMessage}.", e.Message);
                model.BasicReject(@event.DeliveryTag, true);
            }
            catch (Exception e)
            {
                // In any other case, we assume that this error is not recoverable and we don't requeue this message. It should go to dead-letter queue.
                logger.LogError(e, "Exception while receiving message from queue: {ExceptionMessage}.", e.Message);
                model.BasicReject(@event.DeliveryTag, false);
            }
        }
    }
}
