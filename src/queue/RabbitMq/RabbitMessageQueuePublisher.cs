using ConcurrentCollections;
using Microsoft.Extensions.Logging;
using Postman.Codecs;
using Postman.Extensions;
using RabbitMQ.Client;

namespace Postman.RabbitMq
{
    public class RabbitMessageQueuePublisher : QueuePublisherBase<RabbitConfig>
    {
        private protected readonly RabbitMqConnector connector = new();
        protected readonly ConcurrentHashSet<string> createdExchanges = new();
        protected readonly IQueueMessageEncoder encoder;
        protected readonly ILogger logger;
        protected readonly IRabbitMessageRouterHelper routerHelper;
        protected RabbitConfig? config;

        public RabbitMessageQueuePublisher(ILogger<RabbitMessageQueuePublisher> logger, IRabbitMessageRouterHelper routerHelper, IQueueMessageEncoder messageEncoder)
        {
            this.logger = logger;
            this.routerHelper = routerHelper;
            this.encoder = messageEncoder;
        }

        public override Task Connect(RabbitConfig config)
        {
            try
            {
                this.config = config;
                connector.Connect(config);
                logger.LogInformation("Publisher successfully connected to the server.");
                isConnected = true;

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while trying to connect to publisher to server: {ExceptionMessage}.", e.Message);
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
                    connector.Disconnect();
                    logger.LogInformation("Publisher successfully disconnected from the server.");
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

        public override async Task Publish(IQueueMessage message)
        {
            CheckConnection();

            var messageType = message.GetType();
            var exchangeName = routerHelper.GetExchangeName(messageType);
            var deadLetterExchangeName = $"{exchangeName}.dead-letter";
            var routingHeaders = routerHelper.GetAllRoutingsForPublisher(messageType).ToList();
            var routingHeaderInfo = string.Join(", ", routingHeaders);
            using (logger.BeginScope())
            {

                logger.LogTrace("Got new message to send. Id: {MessageId}; Exchange: {ExchangeName}; Routing: {Routing}.", message.Id, exchangeName, routingHeaderInfo);

                // Call declare only if we didn't yet for this exchange.
                // It's ok that multiple services in cluster will do that as final thread-safety is up to RabbitMQ server, here we just try to avoid too many calls.
                if (createdExchanges.Add(exchangeName))
                {
                    try
                    {
                        ArgumentNullException.ThrowIfNull(connector.Channel, nameof(connector.Channel));
                        ArgumentNullException.ThrowIfNull(config, nameof(config));
                        connector.Channel.ExchangeDeclare(exchangeName, ExchangeType.Headers, config.DurableExchanges, config.AutoDeleteExchanges, null);

                        if (config.UseDeadLetterExchanges)
                        {
                            connector.Channel.ExchangeDeclare(deadLetterExchangeName, ExchangeType.Headers, config.DurableExchanges, config.AutoDeleteExchanges, null);
                            logger.LogDebug("Declared new exchange {ExchangeName} with dead letter {DeadLetterExchangeName}.", exchangeName, deadLetterExchangeName);
                        }
                        else
                        {
                            logger.LogDebug("Declared new exchange {ExchangeName} without dead letter one.", exchangeName);
                        }

                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error while declaring new exchange {ExchangeName}: {ExceptionMessage}.", exchangeName, e.Message);
                        throw;
                    }
                }

                try
                {
                    ArgumentNullException.ThrowIfNull(connector.Channel, nameof(connector.Channel));
                    var body = await encoder.Encode(message);
                    var props = connector.Channel.CreateBasicProperties();
                    props.Headers = routingHeaders.ToDictionary(k => $"route-{k}", v => (object)string.Empty);

                    connector.Channel.BasicPublish(exchangeName, string.Empty, props, body);
                    logger.LogTrace("Message {MessageId} was published to exchange {ExchangeName} with routing {Routing}. Body length: {}", message.Id, exchangeName, routingHeaderInfo, body?.Length ?? -1);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error while trying to publish message {MessageId}: {ExceptionMessage}", message.Id, e.Message);
                    throw;
                }
            }
        }
    }
}
