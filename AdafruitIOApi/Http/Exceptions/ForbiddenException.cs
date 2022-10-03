using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// Forbidden -- This action is not permitted.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ForbiddenException : InvalidActionException
    {
        public ForbiddenException() { }
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, Exception inner) : base(message, inner) { }
        protected ForbiddenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
