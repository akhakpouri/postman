using System.Runtime.Serialization;

namespace Sontiq.Queue.Exceptions
{
    [Serializable]
    public class QueueListenerBroadcastException : Exception
    {
        public QueueListenerBroadcastException() { }

        public QueueListenerBroadcastException(string message) : base(message) { }

        public QueueListenerBroadcastException(string message, Exception innerException) : base(message, innerException) { }

        protected QueueListenerBroadcastException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
