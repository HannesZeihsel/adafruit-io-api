using AdafruitIOApi;
using AdafruitIOApi.Exceptions;
using AdafruitIOApi.Parameters;
using AdafruitIOApi.Results;
using AdafruitIOApiHttpTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AdafruitIOApiTest
{
    [TestClass]
    public class TestHttpApiData
    {
        //todo add error {"error":"request failed - Record invalid. Failed to save data to Username/feeds/test, data missing required value"}.
        //todo check behaviour if the feed's value is null or empty.
        //todo check behaivoug if feed is null or empty

        [TestMethod]
        public async System.Threading.Tasks.Task TestCreateDataAsync()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountInvalidKey);

            //wrong account key
            //401
            //{"error": "request failed - invalid API key provided"}
            await Assert.ThrowsExceptionAsync<InvalidApiKeyException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData");
            });

            client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountInvalidUsername);
            //wrong username
            //404
            //{"error": "not found - that username does not exist"}
            await Assert.ThrowsExceptionAsync<UsernameNotFoundException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, "testData");
            });

            client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);
            //wrong feed key
            //404
            //{"error":not found - API documentation can be found at https://io.adafruit.com/api/docs"}
            await Assert.ThrowsExceptionAsync<GeneralNotFoundException>(async () =>
            {
                await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyInvalid, "testData");
            });

            //correct Data
            Datum<int?> datum = new Datum<int?>(12, 1, 2, 3, DateTime.Now);
            var dp = await client.CreateDataAsync<int?>(AdafruitIOAccountData.FeedKeyValid, datum);

            Assert.IsTrue(datum.Value == dp.Value);
            Assert.IsTrue(datum.Lat == dp.Lat);
            Assert.IsTrue(datum.Lon == dp.Lon);
            Assert.IsTrue(datum.Ele == dp.Ele);
            //todo check if datetime conversion is correct
            Assert.IsTrue((datum.CreatedAt.Value.ToUniversalTime() - dp.CreatedAt.Value.ToUniversalTime())
                        < new TimeSpan(0, 2, 0) &&
                    (datum.CreatedAt.Value.ToUniversalTime() - dp.CreatedAt.Value.ToUniversalTime()) >
                        new TimeSpan(0, -2, 0));

            Datum<string> datum2 = new Datum<string>("TestValue", 2, 3, 7, DateTime.Now);
            DataPoint<string> dp2 = await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, datum2);

            Assert.IsTrue(datum2.Value == dp2.Value);
            Assert.IsTrue(datum2.Lat == dp2.Lat);
            Assert.IsTrue(datum2.Lon == dp2.Lon);
            Assert.IsTrue(datum2.Ele == dp2.Ele);
            //todo check if datetime conversion is correct
            Assert.IsTrue((datum2.CreatedAt.Value.ToUniversalTime() - dp2.CreatedAt.Value.ToUniversalTime()) < new TimeSpan(0, 2, 0) &&
                        (datum2.CreatedAt.Value.ToUniversalTime() - dp2.CreatedAt.Value.ToUniversalTime()) > new TimeSpan(0, -2, 0));

            Person p = new Person() { Name = "Doe", Vorname = "John" };
            DataPoint<Person> dp3 = await client.CreateDataAsync<Person>(AdafruitIOAccountData.FeedKeyValid, p);

            Assert.IsTrue(p.Name == dp3.Value.Name);
            Assert.IsTrue(p.Vorname == dp3.Value.Vorname);

            string s = "TeststringData";

            var res = await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, s);
            Assert.IsTrue(s == res.Value);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestGetDataAsync()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetDataAsync<string>(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);

            TimeInterval timeInterval = new TimeInterval(DateTime.Now - TimeSpan.FromHours(1), DateTime.Now);
            ans = await client.GetDataAsync<string>(AdafruitIOAccountData.FeedKeyValid, timeInterval);
            Assert.IsTrue(ans != null);
            Assert.IsTrue(ans.TrueForAll((d) => timeInterval.Contains(d.CreatedAt.Value)));

            //todo check json deserialisation if value is not supplied
            IncludeData includeData = new IncludeData();
            includeData = IncludeData.CreatedAt | IncludeData.Value;
            ans = await client.GetDataAsync<string>(AdafruitIOAccountData.FeedKeyValid, include: includeData);
            Assert.IsTrue(ans != null);
            Assert.IsTrue(ans.TrueForAll((d) => d.Lat == null && d.Lon == null && d.Ele == null && d.ID == null));

            ans = await client.GetDataAsync<string>(AdafruitIOAccountData.FeedKeyValid, limit: 12);
            Assert.IsTrue(ans != null);
            Assert.IsTrue(ans.Count <= 12);

            //todo check result if no data is available.
        }
    }
}