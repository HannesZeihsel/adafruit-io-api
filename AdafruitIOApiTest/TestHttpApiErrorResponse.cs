using AdafruitIOApi;
using AdafruitIOApi.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AdafruitIOApiTest
{
    [TestClass]
    public class TestHttpApiErrorResponse
    {
        [TestMethod]
        public async Task TestWrongUsernameAsync()
        {
            //setup client with wrong username
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountInvalidUsername);


            //test on various methods 
            //todo add more and more as implemented

            await Assert.ThrowsExceptionAsync<UsernameNotFoundException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData");
            });

            await Assert.ThrowsExceptionAsync<UsernameNotFoundException>(async () =>
            {
                await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid);
            });
        }

        [TestMethod]
        public async Task TestWrongAccountKey()
        {
            //setup client with wrong username
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountInvalidKey);


            //test on various methods 
            //todo add more and more as implemented

            await Assert.ThrowsExceptionAsync<InvalidApiKeyException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData");
            });

            await Assert.ThrowsExceptionAsync<InvalidApiKeyException>(async () =>
            {
                await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid);
            });
        }

        [TestMethod]
        public async Task TestWrongFeedName()
        {
            //setup client with correct data (and later supply wrong feed name
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            //test on various methods 
            //todo add more and more as implemented

            await Assert.ThrowsExceptionAsync<GeneralNotFoundException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyInvalid, "testData");
            });

            await Assert.ThrowsExceptionAsync<GeneralNotFoundException>(async () =>
            {
                await client.GetDataAsync(AdafruitIOAccountData.FeedKeyInvalid);
            });

        }

        [TestMethod]
        public async Task TestCorrectAccount()
        {
            //setup client with correct data
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            //test on various methods 
            //todo add more and more as implemented

            Assert.IsTrue(await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData") != null);

            Assert.IsTrue(await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid) != null);
        }
    }
}