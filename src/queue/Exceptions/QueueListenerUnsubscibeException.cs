using System.Runtime.Serialization;

namespace Postman.Exceptions;

[Serializable]
public class QueueListenerUnsubscibeException : Exception
{
    public QueueListenerUnsubscibeException() { }

    public QueueListenerUnsubscibeException(string message) : base(message) { }

    public QueueListenerUnsubscibeException(string message, Exception innerException) : base(message, innerException) { }

    protected QueueListenerUnsubscibeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}