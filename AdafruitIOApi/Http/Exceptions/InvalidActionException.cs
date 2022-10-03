using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// General exception for errors caused by attempting an invalid action.
    /// </summary>
    /// <seealso cref="AdafruitIOApi.Http.Exceptions.BaseException" />
    [Serializable]
    public class InvalidActionException : BaseException
    {
        public InvalidActionException() { }
        public InvalidActionException(string message) : base(message) { }
        public InvalidActionException(string message, Exception inner) : base(message, inner) { }
        protected InvalidActionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
