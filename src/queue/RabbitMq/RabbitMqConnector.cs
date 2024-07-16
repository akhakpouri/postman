using RabbitMQ.Client;

namespace Postman.RabbitMq
{
    /// <summary>
    /// Helper connector class to be used by both <seealso cref="RabbitMessageQueueListener"/> and <seealso cref="RabbitMessageQueuePublisher"/>.
    /// </summary>
    internal class RabbitMqConnector : IDisposable
    {
        protected bool isDisposed = false;
        public IConnection? Connection { get; set; }
        public IModel? Channel { get; set; }

        public virtual void Connect(RabbitConfig config)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(config.ConnectionString),
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };

            this.Connection = factory.CreateConnection();
            this.Channel = Connection.CreateModel();
        }

        public virtual void Disconnect()
        {
            ArgumentNullException.ThrowIfNull(Channel, nameof(Channel));
            ArgumentNullException.ThrowIfNull(Connection, nameof(Connection));
            Channel.Close();
            Connection.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    ArgumentNullException.ThrowIfNull(Channel, nameof(Channel));
                    ArgumentNullException.ThrowIfNull(Connection, nameof(Connection));

                    Channel.Dispose();
                    Connection.Dispose();
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
}