using System.Runtime.Serialization;

namespace Postman.Exceptions
{
    [Serializable]
    public class QueueConnectionException : Exception
    {
        public QueueConnectionException() { }

        public QueueConnectionException(string message) : base(message) { }

        public QueueConnectionException(string message, Exception innerException) : base(message, innerException) { }

        protected QueueConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
