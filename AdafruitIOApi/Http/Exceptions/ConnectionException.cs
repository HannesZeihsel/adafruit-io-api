using System;

namespace AdafruitIOApi.Exceptions
{
    /// <summary>
    /// This exception is used for exceptions that are caused by the underlying http request.
    /// Look at the inner exception if present.
    /// </summary>
    [Serializable]
    class ConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        public ConnectionException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified 
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ConnectionException(string message, Exception inner) : base(message, inner) { }
    }
}