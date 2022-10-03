using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// Internal Server Error -- We had a problem with our server. Try again later.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class InternalServerErrorException : ServerException
    {
        public InternalServerErrorException() { }
        public InternalServerErrorException(string message) : base(message) { }
        public InternalServerErrorException(string message, Exception inner) : base(message, inner) { }
        protected InternalServerErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
