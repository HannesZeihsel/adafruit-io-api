using System;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// This exception is raised if a request to adafruit io will come back with an error stating the API key was invalid.
    /// </summary>
    [Serializable]
    public class InvalidApiKeyException :Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidApiKeyException"/> class.
        /// </summary>
        public InvalidApiKeyException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidApiKeyException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// 
        public InvalidApiKeyException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidApiKeyException"/> class with a specified 
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidApiKeyException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Returns a new instance of the <see cref="InvalidApiKeyException"/> class with a error message
        /// based upon the username used to attempt a connection with Adafruit IO.
        /// </summary>
        /// <param name="username">The username used to attempt a connection to Adafruit IO.</param>
        /// <returns>The generated new Instance of the <see cref="InvalidApiKeyException"/> class.</returns>
        public static InvalidApiKeyException GenerateInvalidKeyExceptionFromUsername(string username)
        {
            return new InvalidApiKeyException($"The passed API-key is invalid for the given username {username}, " +
            "please check if both are correct.");
        }

        /// <summary>
        /// Returns a new instance of the <see cref="InvalidApiKeyException"/> class with a error message
        /// based upon the username used to attempt a connection with Adafruit IO and a reference to the 
        /// inner exception that is the cause of the generated exception.
        /// </summary>
        /// <param name="username">The username used to attempt a connection to Adafruit IO.</param>
        /// /// <param name="inner">The exception that is the cause of the generated exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <returns>The generated new Instance of the <see cref="InvalidApiKeyException"/> class.</returns>
        public static InvalidApiKeyException GenerateInvalidKeyExceptionFromUsername(string username, Exception inner)
        {
            return new InvalidApiKeyException("The passed API-key is invalid for the given " +
            $"username {username}, please check if both are correct.", inner);
        }
    }
}