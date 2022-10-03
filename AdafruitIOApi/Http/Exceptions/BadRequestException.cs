using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// Bad request -- Your request is invalid or was in the wrong format.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class BadRequestException : InvalidActionException
    {
        public BadRequestException() { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, Exception inner) : base(message, inner) { }
        protected BadRequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
