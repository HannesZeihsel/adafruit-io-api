using System;

namespace AdafruitIOApi.Exceptions
{
    /// <summary>
    /// This exception is raised if a request to adofruit io will come back with an error stating not found.
    /// This might be due to wrong feed key or other exceptions.
    /// </summary>
    [Serializable]
    public class GeneralNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNotFoundException"/> class.
        /// </summary>
        public GeneralNotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNotFoundException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GeneralNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNotFoundException"/> class with a specified 
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public GeneralNotFoundException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNotFoundException"/> class with a generated 
        /// error message that is generated based upon the username and feedkey used to attempt the Http 
        /// connection to Adafruit IO.
        /// </summary>
        /// <param name="username">The username used to attempt a connection to Adafruit IO.</param>
        /// <param name="feedKey">The feed Key (name of feed) used to attempt a connection to Adafruit 
        /// IO.</param>
        public GeneralNotFoundException(string username, string feedKey) : base($"Cannot find the requested data. " +
            $"This might be caused by an invalid feed key, please check if the feed {feedKey} is available under " +
            $"the account with the username {username}") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNotFoundException"/> class with a generated 
        /// error message that is generated based upon the username and feedkey used to attempt the Http 
        /// connection to Adafruit IO and a reference to the inner exception that is the cause of this 
        /// exception.
        /// </summary>
        /// <param name="username">The username used to attempt a connection to Adafruit IO.</param>
        /// <param name="feedKey">The feed Key (name of feed) used to attempt a connection to Adafruit 
        /// IO.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public GeneralNotFoundException(string username, string feedKey, Exception inner) : base($"Cannot find the " +
            $"requested data. This might be caused by an invalid feed key, please check if the feed {feedKey} is " +
            $"available under the account with the username {username}", inner) { }
    }
}