using AdafruitIOApi.Http.Parameters;
using AdafruitIOApiHttpTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AdafruitIOApi.Http;
using AdafruitIOApi.Http.Exceptions;
using AdafruitIOApi.Http.Results;

namespace AdafruitIOApiTest
{
    [TestClass]
    public class TestHttpApiData
    {
        //todo add error {"error":"request failed - Record invalid. Failed to save data to Username/feeds/test, data missing required value"}.
        //todo check behaviour if the feed's value is null or empty.
        //todo check behaivoug if feed is null or empty

        [TestMethod]
        public async Task TestCreateDataAsync()
        {
            var client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);
            
            Datum<int?> datum = new Datum<int?>(12, 1, 2, 3, DateTime.Now);
            var dp = await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, datum);

            Assert.IsTrue(datum.Value == dp.Value);
            Assert.IsTrue(datum.Lat == dp.Lat);
            Assert.IsTrue(datum.Lon == dp.Lon);
            Assert.IsTrue(datum.Ele == dp.Ele);
            
            Assert.IsTrue((datum.CreatedAt!.Value.ToUniversalTime() - dp.CreatedAt!.Value.ToUniversalTime()) < new TimeSpan(0, 2, 0) &&
                    (datum.CreatedAt.Value.ToUniversalTime() - dp.CreatedAt.Value.ToUniversalTime()) > new TimeSpan(0, -2, 0));
            
            
            //todo check why i cant use DateTime.Now here but the above call works.
            Datum<string> datum2 = new Datum<string>("TestValue", 2, 3, 7, DateTime.Now - TimeSpan.FromSeconds(10));
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

            var resString = await client.CreateDataAsync(AdafruitIOAccountData.FeedOfGroup,
                AdafruitIOAccountData.GroupName, "test");
            
            Assert.IsTrue("test".Equals(resString.Value));
            
            var resPerson = await client.CreateDataAsync(AdafruitIOAccountData.FeedOfGroup, AdafruitIOAccountData.GroupName, p);
            Assert.IsTrue(p.Name == resPerson.Value.Name && p.Vorname == resPerson.Value.Vorname);
            
            datum = new Datum<int?>(12, 1, 2, 3, DateTime.Now - TimeSpan.FromSeconds(10));
            dp = await client.CreateDataAsync(AdafruitIOAccountData.FeedOfGroup, AdafruitIOAccountData.GroupName, datum);
            
            Assert.IsTrue(datum.Value == dp.Value);
            Assert.IsTrue(datum.Lat == dp.Lat);
            Assert.IsTrue(datum.Lon == dp.Lon);
            Assert.IsTrue(datum.Ele == dp.Ele);
            
            Assert.IsTrue((datum.CreatedAt!.Value.ToUniversalTime() - dp.CreatedAt!.Value.ToUniversalTime()) < new TimeSpan(0, 2, 0) &&
                          (datum.CreatedAt.Value.ToUniversalTime() - dp.CreatedAt.Value.ToUniversalTime()) > new TimeSpan(0, -2, 0));
        }

        [TestMethod]
        public async Task TestGetDataAsync()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);
            DateTime startTime = DateTime.Now - TimeSpan.FromHours(1);
            DateTime endTime = DateTime.Now;
            ans = await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid, null, startTime, endTime);
            Assert.IsTrue(ans != null);
            Assert.IsTrue(ans.TrueForAll((d) => d.CreatedAt.Value>=startTime && d.CreatedAt.Value<=endTime));

            IncludeData includeData = new IncludeData();
            includeData = IncludeData.CreatedAt | IncludeData.Value;
            ans = await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid, include: includeData);
            Assert.IsTrue(ans != null);
            Assert.IsTrue(ans.TrueForAll((d) => d.Lat == null && d.Lon == null && d.Ele == null && d.Id == null));

            ans = await client.GetDataAsync(AdafruitIOAccountData.FeedKeyValid, limit: 12);
            Assert.IsTrue(ans != null);
            Assert.IsTrue(ans.Count <= 12);

            //todo check result if no data is available.
        }

        [TestMethod]
        public async Task TestChartFeedData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetChartFeedDataAsync(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);

            ans = await client.GetChartFeedDataAsync(AdafruitIOAccountData.FeedKeyValid, DateTime.Now, DateTime.Now+ TimeSpan.FromHours(2), Resolution.OneMinute, null, AggregateField.Max, true);
            Assert.IsTrue(ans != null);

            ans = await client.GetChartFeedDataAsync(AdafruitIOAccountData.FeedKeyValid, DateTime.Now, DateTime.Now + TimeSpan.FromHours(2), Resolution.OneMinute, 2, AggregateField.Max, false);
            Assert.IsTrue(ans != null);
        }

        [TestMethod]
        public async Task TestGetLastData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetLastDataAsync(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);
        }

        [TestMethod]
        public async Task TestGetFirstData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetFirstDataAsync(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);
        }

        [TestMethod]
        public async Task TestGetNextData()
        {
            //await TestGetPreviousData();
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetNextDataAsync(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);
        }

        [TestMethod]
        public async Task TestGetPreviousData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetPreviousDataAsync(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);

            ans = await client.GetPreviousDataAsync(AdafruitIOAccountData.FeedKeyValid,
                IncludeData.Lon | IncludeData.CreatedAt);
            Assert.IsTrue(ans != null);
        }

        [TestMethod]
        public async Task TaskTestGetMostRecentData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            var ans = await client.GetMostRecentDataAsync(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(ans != null);

            await Assert.ThrowsExceptionAsync<NotFoundException>(async ()=>await client.GetMostRecentDataAsync(("invalid")));
        }
        
        [TestMethod]
        public async Task TaskTestGetDataById()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            Person p = new Person() {Name = "Smith", Vorname = "John"};
            var ans = await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, p);

            var retString = await client.GetDataByIdAsync(AdafruitIOAccountData.FeedKeyValid, ans.Id);
            Assert.IsTrue(JsonSerializer.Serialize(p).Equals(retString.Value));

            var ret = await client.GetDataByIdAsync<Person>(AdafruitIOAccountData.FeedKeyValid, ans.Id);
            Assert.IsTrue(p.Name == ret.Value.Name && p.Vorname == ret.Value.Vorname);
        }
        
        [TestMethod]
        public async Task TaskTestUpdateData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            Person p = new Person() {Name = "Smith", Vorname = "John"};
            var ans = await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, p);
            p.Name = "Doe";

            var ret = await client.UpdateDataAsync(AdafruitIOAccountData.FeedKeyValid, ans.Id, p);
            Assert.IsTrue(p.Name == ret.Value.Name && p.Vorname == ret.Value.Vorname);
            
            var retString = await client.UpdateDataAsync(AdafruitIOAccountData.FeedKeyValid, ans.Id, "test");
            Assert.IsTrue("test" == retString.Value);
        }
        
        [TestMethod]
        public async Task TaskTestDeleteData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            Person p = new Person() {Name = "Smith", Vorname = "John"};
            var ans = await client.CreateDataAsync(AdafruitIOAccountData.FeedKeyValid, p);
            
            await client.DeleteDataAsync(AdafruitIOAccountData.FeedKeyValid, ans.Id);

            await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
                await client.GetDataByIdAsync(AdafruitIOAccountData.FeedName, ans.Id));
        }
        
        [TestMethod]
        public async Task TaskTestCreateMultipleDataRecords()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);
            
            var ansInt = await client.CreateMultipleDataRecordsAsync(AdafruitIOAccountData.FeedKeyValid,
                new List<Datum<int>>() {new Datum<int>(1), new Datum<int>(2)});
            
            Assert.IsTrue(1 == ansInt[0].Value);
            Assert.IsTrue(2 == ansInt[1].Value);

            var retInt = await client.GetLastDataAsync<int>(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(2 == retInt.Value);
            
            
            
            
            Person p1 = new Person() {Name = "Smith", Vorname = "John"};
            Person p2 = new Person() {Name = "Doe", Vorname = "John"};
            var ans = await client.CreateMultipleDataRecordsAsync(AdafruitIOAccountData.FeedKeyValid,
                new List<Datum<Person>>() {new Datum<Person>(p1), new Datum<Person>(p2)});
            
            Assert.IsTrue(p1.Name == ans[0].Value.Name && p1.Vorname == ans[0].Value.Vorname);
            Assert.IsTrue(p2.Name == ans[1].Value.Name && p2.Vorname == ans[1].Value.Vorname);

            var ret = await client.GetLastDataAsync<Person>(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(p2.Name == ret.Value.Name && p2.Vorname == ret.Value.Vorname);
            
            
            
            
            var ansString = await client.CreateMultipleDataRecordsAsync(AdafruitIOAccountData.FeedOfGroup, AdafruitIOAccountData.GroupName,
                new List<Datum<string>>() {new Datum<string>("1"), new Datum<string>("two")});
            
            Assert.IsTrue("1".Equals(ansString[0].Value));
            Assert.IsTrue("two".Equals(ansString[1].Value));

            var retString =
                await client.GetLastDataAsync<string>(AdafruitIOAccountData.GroupName + "." +
                                                      AdafruitIOAccountData.FeedOfGroup);
            Assert.IsTrue("two" == retString.Value);
            
            
            ans = await client.CreateMultipleDataRecordsAsync(AdafruitIOAccountData.FeedOfGroup, AdafruitIOAccountData.GroupName,
                new List<Datum<Person>>() {new Datum<Person>(p1), new Datum<Person>(p2)});
            
            Assert.IsTrue(p1.Name == ans[0].Value.Name && p1.Vorname == ans[0].Value.Vorname);
            Assert.IsTrue(p2.Name == ans[1].Value.Name && p2.Vorname == ans[1].Value.Vorname);

            ret = await client.GetLastDataAsync<Person>(AdafruitIOAccountData.FeedKeyValid);
            Assert.IsTrue(p2.Name == ret.Value.Name && p2.Vorname == ret.Value.Vorname);
        }
        
        [TestMethod]
        public async Task TaskCreateGroupData()
        {
            AdafruitIOHttpClient client = new AdafruitIOHttpClient(AdafruitIOAccountData.AccountValid);

            GroupData<int> gd = new GroupData<int>();
            gd.Feeds.Add(new Feed<int>(AdafruitIOAccountData.FeedOfGroup, 12));
            gd.Feeds.Add(new Feed<int>(AdafruitIOAccountData.FeedOfGroup, 20));
            var ans = await client.CreateGroupDataAsync(AdafruitIOAccountData.GroupName, gd);
            
            //todo test different feeds
            Assert.IsTrue(ans.Count==2);
            Assert.IsTrue(ans[0].Value == 12);
            Assert.IsTrue(ans[1].Value == 20);
        }
    }
}