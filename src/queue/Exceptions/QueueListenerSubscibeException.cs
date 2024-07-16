using System.Runtime.Serialization;

namespace Postman.Exceptions;

[Serializable]
public class QueueListenerSubscibeException : Exception
{
    public QueueListenerSubscibeException() { }

    public QueueListenerSubscibeException(string message) : base(message) { }

    public QueueListenerSubscibeException(string message, Exception innerException) : base(message, innerException) { }

    protected QueueListenerSubscibeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}