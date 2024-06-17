using System.Runtime.Serialization;

namespace Sontiq.Queue.Exceptions
{
    [Serializable]
    public class QueueMessageProcessingException : Exception
    {
        public bool IsRecoverable { get; set; }

        public QueueMessageProcessingException(bool isRecoverable)
        {
            this.IsRecoverable = isRecoverable;
        }

        public QueueMessageProcessingException(bool isRecoverable, string message) : base(message)
        {
            this.IsRecoverable = isRecoverable;
        }

        public QueueMessageProcessingException(bool isRecoverable, string message, Exception innerException) : base(message, innerException)
        {
            this.IsRecoverable = isRecoverable;
        }

        protected QueueMessageProcessingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
