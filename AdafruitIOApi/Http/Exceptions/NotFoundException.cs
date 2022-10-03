using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// Not Found -- The specified record could not be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class NotFoundException : InvalidActionException
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
        protected NotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
