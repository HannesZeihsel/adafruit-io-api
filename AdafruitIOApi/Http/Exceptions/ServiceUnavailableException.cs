using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{

    [Serializable]
    public class ServiceUnavailableException : ServerException
    {
        public ServiceUnavailableException() { }
        public ServiceUnavailableException(string message) : base(message) { }
        public ServiceUnavailableException(string message, Exception inner) : base(message, inner) { }
        protected ServiceUnavailableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
