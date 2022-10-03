using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// You requested a format that we don't serve.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class NotAcceptableException : InvalidActionException
    {
        public NotAcceptableException() { }
        public NotAcceptableException(string message) : base(message) { }
        public NotAcceptableException(string message, Exception inner) : base(message, inner) { }
        protected NotAcceptableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
