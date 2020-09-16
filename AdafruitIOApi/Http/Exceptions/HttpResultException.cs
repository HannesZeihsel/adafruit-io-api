using System;
using System.Net.Http;

namespace AdafruitIOApi.Exceptions
{
    /// <summary>
    /// This exception is raised if a request to adafruit io will come back with an http error code that was not expected.
    /// </summary>
    [Serializable]
    public class HttpResultException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResultException"/> class.
        /// </summary>
        public HttpResultException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResultException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpResultException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResultException"/> class with a specified 
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public HttpResultException(string message, Exception inner) : base(message, inner) { }
    }
}