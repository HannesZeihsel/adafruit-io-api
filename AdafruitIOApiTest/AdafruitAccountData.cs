using AdafruitIOApi;

namespace AdafruitIOApiTest
{
    /// <summary>
    /// Used to setup default data for the Adafruit IO Account details. 
    /// IMPORTANT:  Rename this File to AdafruitIOAccountData
    ///             And change the AccountKey and Usernames and FeedName to your Adafruit IO Account data.
    /// </summary>
    public static class AdafruitAccountData
    {
        //change the Username and Key to your Adafruit IO Account's Data.
        //See here to find out how to get them: https://io.adafruit.com/api/docs/#authentication
        public static readonly string AccountKey = "___Your KEY___";
        public static readonly string AccountUsername = "___Your Username___";
        public static readonly string FeedName = "___Your Feedname___";

        //sample Data used to test accounts and the response if account data is wrong
        public static AdafruitIOAccount AccountValid = new AdafruitIOAccount(AccountUsername, AccountKey);
        public static AdafruitIOAccount AccountInvalidUsername = new AdafruitIOAccount("SomeRandomFakeUsername398u7yP6peo", AccountKey);
        public static AdafruitIOAccount AccountInvalidKey = new AdafruitIOAccount(AccountUsername, "SomeRandomFakeKey2aoeuthi");
        public static AdafruitIOAccount AccountInvalidBoth = new AdafruitIOAccount("SomeRandomFakeUsername2ugea87eifdc", "SomeRandomFakeKey8aoeuggfigg");

        //sample data to test the feed (Note the feed 'test' has to be valid) and the response if the feed is incorrect
        public static readonly string FeedKeyValid = FeedName;
        public static readonly string FeedKeyInvalid = "nvfeed";
    }
}
