using System;
namespace HMSD.EncryptionService.Exceptions
{
    [Serializable()]
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException() : base() { }
        public InvalidKeyException(string message) : base(message) { }
        public InvalidKeyException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected InvalidKeyException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
