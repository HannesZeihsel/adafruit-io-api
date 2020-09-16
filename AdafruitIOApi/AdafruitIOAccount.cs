namespace AdafruitIOApi
{
    /// <summary>
    /// Represents an account at Adafruit IO.
    /// </summary>
    public class AdafruitIOAccount
    {
        //todo maybe the key needs to be stored differently to increase security.

        ///The username of this Adafruit IO account.
        public string Username { get; set; }

        ///The key of this Adafruit IO account.
        public string Key { get; set; }

        /// <summary>
        /// Initialize a new Adafruit IO account with the given username and key of the account.
        /// </summary>
        /// <param name="username">The username of this Adafruit IO account.</param>
        /// <param name="key">The key of this Adafruit IO account.</param>
        public AdafruitIOAccount(string username, string key)
        {
            Username = username;
            Key = key;
        }
    }
}