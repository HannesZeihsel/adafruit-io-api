using System;

namespace AdafruitIOApi.Http.Exceptions
{
    /// <summary>
    /// This exception is raised if a request to adafruit io will come back with an error stating the username was not found.
    /// </summary>
    [Serializable]
    public class UsernameNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsernameNotFoundException"/> class.
        /// </summary>
        public UsernameNotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernameNotFoundException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        ///
        public UsernameNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernameNotFoundException"/> class with a specified 
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public UsernameNotFoundException(string message, Exception inner):base(message, inner) { }

        /// <summary>
        /// Returns a new instance of the <see cref="UsernameNotFoundException"/> class with a error message
        /// based upon the username used to attempt a connection with Adafruit IO.
        /// </summary>
        /// <param name="username">The username used to attempt a connection to Adafruit IO.</param>
        /// <returns>The generated new Instance of the <see cref="UsernameNotFoundException"/> class.</returns>
        public static UsernameNotFoundException GenerateUsernameNotFoundExceptionFromUsername(string username)
        {
            return new UsernameNotFoundException($"The username {username} was not found on adafruit.io. " +
            "Please check if the username does exits.");
        }

        /// <summary>
        /// Returns a new instance of the <see cref="UsernameNotFoundException"/> class with a error message
        /// based upon the username used to attempt a connection with Adafruit IO and a reference to the 
        /// inner exception that is the cause of the generated exception.
        /// </summary>
        /// <param name="username">The username used to attempt a connection to Adafruit IO.</param>
        /// /// <param name="inner">The exception that is the cause of the generated exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <returns>The generated new Instance of the <see cref="UsernameNotFoundException"/> class.</returns>
        public static UsernameNotFoundException GenerateUsernameNotFoundExceptionFromUsername(string username, Exception inner)
        {
            return new UsernameNotFoundException($"The username {username} was not found on adafruit.io. " +
            "Please check if the username does exits.", inner);
        }
    }
}