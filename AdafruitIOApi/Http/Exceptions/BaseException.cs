using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// 
    /// BaseException for any HTTP request failes
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class BaseException: Exception
    {
        public BaseException() { }
        public BaseException(string message) : base(message) { }
        public BaseException(string message, Exception inner) : base(message, inner) { }
        protected BaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
