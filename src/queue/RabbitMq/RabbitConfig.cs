namespace Postman.RabbitMq
{
    public class RabbitConfig
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = string.Empty;
        public bool UseSeparateQueuesPerEvent { get; set; } = false;
        public bool AutoDeleteExchanges { get; set; } = false;
        public bool DurableExchanges { get; set; } = true;
        public bool AutoDeleteQueues { get; set; } = false;
        public bool DurableQueues { get; set; } = true;
        public bool UseDeadLetterExchanges { get; set; } = false;
        public string ConnectionString => $"amqp://{Username}:{Password}@{Host}:{Port}/{VirtualHost}";
    }
}
