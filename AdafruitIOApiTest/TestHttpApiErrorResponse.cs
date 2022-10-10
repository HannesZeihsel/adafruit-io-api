using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using AdafruitIOApi.Http;
using AdafruitIOApi.Http.Exceptions;

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

            await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData");
            });

            await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
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

            await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData");
            });

            await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
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

            await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyInvalid, "testData");
            });

            await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
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

            try
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData");
                await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid);
            }
            catch(Exception e)
            {
                Assert.Fail($"Triggered error '{e.Message}'");   
            }
        }
    }
}