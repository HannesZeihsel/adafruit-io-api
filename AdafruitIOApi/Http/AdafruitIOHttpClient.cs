using AdafruitIOApi.Exceptions;
using AdafruitIOApi.Parameters;
using AdafruitIOApi.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdafruitIOApi
{
    /// <summary>
    /// Provides methods to access to the HTTP API of Adafruit IO. This handeles the creation of 
    /// the HTTP requests and the interpretation and conversion of the received data.
    /// </summary>
    public class AdafruitIOHttpClient
    {
        private const string HeaderNameApiKey = "X-AIO-Key";
        private readonly AdafruitIOAccount account;
        
        //The http client managing the communiciton with Adafruit IO.
        private HttpClient Client { get; set; }

        /// <summary>
        /// Creates a new <see cref="AdafruitIOHttpClient"/> instance with the given account 
        /// and initializes the HttpClient.
        /// </summary>
        /// <param name="account">The Adafruit IO account that schould be accessed.</param>
        public AdafruitIOHttpClient(AdafruitIOAccount account) {
            this.account = account ?? throw new ArgumentNullException(nameof(account));
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Adds the <paramref name="data"/>P to the provided feed 
        /// <paramref name="feed"/> on Adafruit IO.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to wich the datum schould be added to.</param>
        /// <param name="data">The data which schould be added to the feed.</param>
        /// <returns>The added <see cref="DataPoint{T}"/> with <code>T = string</code> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<DataPoint<string>> CreateDataAsync(string feed, string data)
        {
            return await CreateDataAsync<string>(feed, new Datum<string>(data));
        }

        /// <summary>
        /// Adds the value <paramref name="data"/>P to the provided feed 
        /// <paramref name="feed"/> on Adafruit IO.
        /// </summary>
        /// <typeparam name="T">The type of the value of the datum to be added.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to wich the datum schould be added to.</param>
        /// <param name="data">The data which schould be added to the feed.</param>
        /// <returns>The added <see cref="DataPoint{T}"/> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<DataPoint<T>> CreateDataAsync<T>(string feed, T data)
        {
            return await CreateDataAsync<T>(feed, new Datum<T>(data));
        }

        /// <summary>
        /// Adds the datum <paramref name="datum"/> to the provided feed <paramref name="feed"/> on 
        /// Adafruit IO.
        /// </summary>
        /// <typeparam name="T">The type of the datums <see cref="Datum{T}.Value"/>.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to wich the datum schould be added to.</param>
        /// <param name="datum">The datum that schould be added to the feed.</param>
        /// <returns>The added <see cref="DataPoint{T}"/> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<DataPoint<T>> CreateDataAsync<T>(string feed, Datum<T> datum)
        {
            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://io.adafruit.com/api/v2/{account.Username}/feeds/{feed}/data"),
                    Content = new StringContent(datum.GetJson(), Encoding.UTF8, "application/json")
                };

                request.Content.Headers.Add(HeaderNameApiKey, account.Key);
                var response = await Client.PostAsync(request.RequestUri, request.Content);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);
                return DataPoint<T>.GenerateFromJson(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e) when (e is InvalidOperationException || e is HttpRequestException || e is TaskCanceledException)
            {
                throw new ConnectionException("Exception in http request.", e);
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Warning: Pagination not yet supported.
        /// Returns all the datapoints from Adafruit IO in the given feed <paramref name="feed"/> which
        /// adhere to the given additional constrains if any.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="timeInterval">The time interval that the data schould be created in.</param>
        /// <param name="limit">The limit of the number datapoints that schould be returned</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <param name="before">TODO: check meaning of this parameter.</param>
        /// <returns>A List of <see cref="DataPoint{T}"/> with <code>T=string</code>with the retrieved data.</returns>
        public async Task<List<DataPoint<string>>> GetDataAsync(string feed,
               TimeInterval timeInterval = null,
               int? limit = null,
               IncludeData? include = null,
               string before = null)
        {
            return await GetDataAsync<string>(feed, timeInterval, limit, include, before);
        }

        /// <summary>
        /// Warning: Pagination not yet supported.
        /// Returns all the datapoints from Adafruit IO in the given feed <paramref name="feed"/> which
        /// adhere to the given additional constrains if any.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="timeInterval">The time interval that the data schould be created in.</param>
        /// <param name="limit">The limit of the number datapoints that schould be returned</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <param name="before">TODO: check meaning of this parameter.</param>
        /// <returns>A List of <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<List<DataPoint<T>>> GetDataAsync<T>(string feed, 
            TimeInterval timeInterval = null, 
            int? limit = null, 
            IncludeData? include = null, 
            string before= null)
        {
            //setup query parameters if those are provided
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            if (!(timeInterval?.StartTime is null))
                queryParameters["start_time"] = timeInterval.StartTime.ToString("s"); //todo check if format is correct
            if (!(timeInterval?.EndTime is null))
                queryParameters["end_time"] = timeInterval.EndTime.ToString("s"); // todo check if format is correct
            if (!(limit is null))
                queryParameters["limit"] = limit.ToString();
            if (!(include is null))
                queryParameters["include"] = include.Value.GetDataText();
            if (!string.IsNullOrEmpty(before))
                queryParameters["before"] = before;
            string queryParameterString = queryParameters.Count <= 0 ? "" : "?" + queryParameters.ToString();

            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://io.adafruit.com/api/v2/{account.Username}/feeds/{feed}/data{queryParameterString}")
                };
                //request.Headers.Add("X-AIO-Key", feed.AdafruitIOAccount.Key);
                Client.DefaultRequestHeaders.Add(HeaderNameApiKey, account.Key);
                var response = await Client.GetAsync(request.RequestUri);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);
                if (typeof(T) == typeof(string))
                    return JsonConvert.DeserializeObject<List<DataPoint<T>>>(await response.Content.ReadAsStringAsync());
                else
                {
                    var retS = JsonConvert.DeserializeObject<List<DataPoint<string>>>(await response.Content.ReadAsStringAsync());
                    var retT = new List<DataPoint<T>>();
                    foreach (var dp in retS)
                    {
                        retT.Add(dp.ConvertTo<T>());
                    }
                    return retT;
                }
            }
            catch (Exception e) when (e is InvalidOperationException || e is HttpRequestException || e is TaskCanceledException)
            {
                throw new ConnectionException("Exception in http request.", e);
            }
            catch (Exception e) { throw e; }
            finally
            {
                Client.DefaultRequestHeaders.Remove(HeaderNameApiKey);
            }
        }

        /*public string ChartFeedData(string feed, 
            TimeInterval timeInterval = null,
            int? resolution = null,
            int? hours = null,
            string field = null,
            bool? raw = null)
        {
            throw new NotImplementedException();
        }

        public string CreateMultipleDataRecords(string feed, string[] data)
        {
            throw new NotImplementedException();
        }

        public string CreateMultipleDataRecords(string feed, string data)
        {
            throw new NotImplementedException();
        }

        public string GetPreviousData(string feed, IncludeData? includeData = null)
        {
            throw new NotImplementedException();
        }

        public string GetNextData(string feed, IncludeData? includeData = null)
        {
            throw new NotImplementedException();
        }

        public string GetLastData(string feed, IncludeData? includeData = null)
        {
            throw new NotImplementedException();
        }

        public string GetFirstsData(string feed, IncludeData? includeData = null)
        {
            throw new NotImplementedException();
        }*/

        /// <summary>
        /// Gets the most recent datapoint.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <returns>The most recent <see cref="DataPoint{T}"/> of the given feed.</returns>
        public async Task<DataPoint<T>> GetMostRecentDataPointAsync<T>(string feed)
        {
            return (await GetDataAsync<T>(feed, limit: 1))[0];
        }

        /*public string GetMostRecentData(string feed)
        {
            throw new NotImplementedException();
        }

        public string GetDataPoint(string feed, string id, IncludeData? includeData = null)
        {
            throw new NotImplementedException();
        }

        public string UpdateDataPoint(string feed, string id, string datum)
        {
            throw new NotImplementedException();
        }

        public string DeleteDataPoint(string feed, string id)
        {
            throw new NotImplementedException();
        }*/

        /// <summary>
        /// Used to check the result of an HTTP response and raise an appropriate error if such error is detected.
        /// </summary>
        /// <param name="response">The response to be checked for errors.</param>
        /// <param name="feed">The feeds name, to give additional information to raise a more concise error.</param>
        /// <returns></returns>
        private async Task RaiseErrorIfHttpResponseErrorAsync(HttpResponseMessage response, string feed)
        {
            if (response.IsSuccessStatusCode)
                return;
            else
            {
                HttpResultException inner = new HttpResultException($"Error Response Status-Code (HTTP): {response.StatusCode}. " +
                             $"Content of the Response is: {await response.Content.ReadAsStringAsync()}.");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    if ((await response.Content.ReadAsStringAsync()).
                                Equals("{\"error\":\"request failed - invalid API key provided\"}", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidApiKeyException(account.Username, inner);
                    else
                        throw inner;
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    if ((await response.Content.ReadAsStringAsync()).
                                Equals("{\"error\":\"not found - that username does not exist\"}", StringComparison.OrdinalIgnoreCase))
                        throw new UsernameNotFoundException(account.Username, inner);
                    else
                        throw new GeneralNotFoundException(account.Username, feed, inner);
                else
                    throw inner;
            }
        }
    }
}