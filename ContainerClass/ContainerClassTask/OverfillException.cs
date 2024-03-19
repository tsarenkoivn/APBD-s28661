using System.Runtime.Serialization;

namespace ContainerClassTask
{
    [Serializable]
    internal class OverfillException : Exception
    {
        public OverfillException()
        {
        }

        public OverfillException(string? message) : base(message)
        {
        }

        public OverfillException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected OverfillException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}