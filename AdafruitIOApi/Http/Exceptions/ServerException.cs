using System;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// General Exception caused by the adafruit IO server not working as expected.
    /// </summary>
    /// <seealso cref="AdafruitIOApi.Http.Exceptions.BaseException" />
    [Serializable]
    public class ServerException : BaseException
    {
        public ServerException() { }
        public ServerException(string message) : base(message) { }
        public ServerException(string message, Exception inner) : base(message, inner) { }
        protected ServerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
