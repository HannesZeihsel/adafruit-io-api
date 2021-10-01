using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AdafruitIOApi.Http.Exceptions;
using AdafruitIOApi.Http.Parameters;
using AdafruitIOApi.Http.Results;
using Newtonsoft.Json;

namespace AdafruitIOApi.Http
{
    //todo add throw error to documentation
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
            Client.DefaultRequestHeaders.Add(HeaderNameApiKey, account.Key);
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
            return await CreateDataAsync(feed, new Datum<string>(data));
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
            return await CreateDataAsync(feed, new Datum<T>(data));
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
                    Content = new StringContent(JsonConvert.SerializeObject(datum), Encoding.UTF8, "application/json")
                };

                var response = await Client.PostAsync(request.RequestUri, request.Content);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);
                return JsonConvert.DeserializeObject<DataPoint<T>>(await response.Content.ReadAsStringAsync());
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
        /// <param name="startTime">The beginning of the interval that the data schould be in.</param>
        /// <param name="endTime">The end of the interval that the data schould be in.</param>
        /// <param name="limit">The limit of the number datapoints that schould be returned</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <param name="before">TODO: check meaning of this parameter.</param>
        /// <returns>A List of <see cref="DataPoint{T}"/> with <code>T=string</code>with the retrieved data.</returns>
        public async Task<List<DataPoint<string>>> GetDataAsync(string feed,
               DateTime? startTime = null,
               DateTime? endTime = null,
               int? limit = null,
               IncludeData? include = null,
               string before = null)
        {
            return await GetDataAsync<string>(feed, startTime, endTime, limit, include, before);
        }

        /// <summary>
        /// Warning: Pagination not yet supported.
        /// Returns all the datapoints from Adafruit IO in the given feed <paramref name="feed"/> which
        /// adhere to the given additional constrains if any.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="startTime">The beginning of the interval that the data schould be in.</param>
        /// <param name="endTime">The end of the interval that the data schould be in.</param>
        /// <param name="limit">The limit of the number datapoints that schould be returned</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <param name="before">TODO: check meaning of this parameter.</param>
        /// <returns>A List of <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<List<DataPoint<T>>> GetDataAsync<T>(string feed, 
            DateTime? startTime= null, 
            DateTime? endTime = null,
            int? limit = null, 
            IncludeData? include = null, 
            string before= null)
        {
            //setup query parameters if those are provided
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            if (!(startTime is null))
                queryParameters["start_time"] = startTime.Value.ToString("s");
            if (!(endTime is null))
                queryParameters["end_time"] = endTime.Value.ToString("s");
            if (!(limit is null))
                queryParameters["limit"] = limit.ToString();
            if (!(include is null))
                queryParameters["include"] = include.Value.GetDataText();
            if (!string.IsNullOrEmpty(before))
                queryParameters["before"] = before;
            string queryParameterString = queryParameters.Count <= 0 ? "" : "?" + queryParameters;

            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://io.adafruit.com/api/v2/{account.Username}/feeds/{feed}/data{queryParameterString}")
                };
                
                var response = await Client.GetAsync(request.RequestUri);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);
                return JsonConvert.DeserializeObject<List<DataPoint<T>>>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e) when (e is InvalidOperationException || e is HttpRequestException || e is TaskCanceledException)
            {
                throw new ConnectionException("Exception in http request.", e);
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Get chart data based upon the pased parameters to use as data to display a chart.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="startTime">The beginning of the interval that the data schould be in.</param>
        /// <param name="endTime">The end of the interval that the data schould be in.</param>
        /// <param name="resolution">The resolution representing the size of the aggreagete slice.</param>
        /// <param name="hours">The number of hours to include in the data (Will be ignored if 
        /// <paramref name="startTime"/> or <paramref name="endTime"/> are given).</param>
        /// <param name="field">The type of aggreagation to use to calculate the data in each slice.</param>
        /// <param name="raw">If true it forces the data to be returned raw. Not compatible with 
        /// <paramref name="field"/> or <paramref name="resolution"/>.</param>
        /// <returns>The ChartData as requested and defined by the parameters.</returns>
        public async Task<ChartData> ChartFeedDataAsync(string feed, 
            DateTime? startTime = null,
            DateTime? endTime = null,
            Resolution? resolution = null,
            int? hours = null,
            AggregateField? field = null,
            bool? raw = null)
        {
            //setup query parameters if those are provided
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            if (!(startTime is null))
                queryParameters["start_time"] = startTime.Value.ToString("s");
            if (!(endTime is null))
                queryParameters["end_time"] = endTime.Value.ToString("s");
            if (!(resolution is null))
                queryParameters["resolution"] = ((int)resolution).ToString();
            if (!(hours is null))
                queryParameters["hours"] = hours.Value.ToString();
            if (!(field is null))
                queryParameters["field"] = field.Value.GetDataText();
            if (!(raw is null))
                queryParameters["raw"] = raw.Value ? "true" : "false";
            string queryParameterString = queryParameters.Count <= 0 ? "" : "?" + queryParameters;

            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://io.adafruit.com/api/v2/{account.Username}/feeds/{feed}/data/chart{queryParameterString}")
                };
                
                var response = await Client.GetAsync(request.RequestUri);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);
                return JsonConvert.DeserializeObject<ChartData>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e) when (e is InvalidOperationException || e is HttpRequestException || e is TaskCanceledException)
            {
                throw new ConnectionException("Exception in http request.", e);
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Adds all the provided Data as multiple values to the given feed. 
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to wich the datum schould be added to.</param>
        /// <param name="data">The list of data which schould be added to the feed.</param>
        /// <returns>The added List of <see cref="DataPoint{T}"/> with <code>T = string</code> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<List<DataPoint<string>>> CreateMultipleDataRecordsAsync(string feed, List<Datum<string>> data)
        {
            return await CreateMultipleDataRecordsAsync<string>(feed, data);
        }
        
        /// <summary>
        /// Adds all the provided Data as multiple values to the given feed. 
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to wich the datum schould be added to.</param>
        /// <param name="data">The list of data which schould be added to the feed.</param>
        /// <returns>The added List of <see cref="DataPoint{T}"/> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<List<DataPoint<T>>> CreateMultipleDataRecordsAsync<T>(string feed, List<Datum<T>> data)
        {
            string dataString = JsonConvert.SerializeObject(data);

            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://io.adafruit.com/api/v2/{account.Username}/feeds/{feed}/data"),
                    Content = new StringContent(JsonConvert.SerializeObject(dataString), Encoding.UTF8, "application/json")
                };

                var response = await Client.PostAsync(request.RequestUri, request.Content);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);

                return JsonConvert.DeserializeObject<List<DataPoint<T>>>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e) when (e is InvalidOperationException || e is HttpRequestException || e is TaskCanceledException)
            {
                throw new ConnectionException("Exception in http request.", e);
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Helper methode to get one DataPoint back from Adafruit that requires only include parameters and a special path.
        /// </summary>
        /// <typeparam name="T">The type of the expected value.</typeparam>
        /// <param name="feed">The feed from which the data schould be retrieved.</param>
        /// <param name="special">The special path pointing to the wanted data.</param>
        /// <param name="include">The include parameters that schould be present in the result.</param>
        /// <returns>The DataPoint as returned by adafruit.</returns>
        private async Task<DataPoint<T>> GetSpecialDataAsync<T>(string feed, string special,  IncludeData? include = null)
        {
            //setup query parameters if those are provided
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            if (!(include is null))
                queryParameters["include"] = include.Value.GetDataText();
            string queryParameterString = queryParameters.Count <= 0 ? "" : "?" + queryParameters;

            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://io.adafruit.com/api/v2/{account.Username}/feeds/{feed}/data/{special}/{queryParameterString}")
                };

                var response = await Client.GetAsync(request.RequestUri);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);
                return JsonConvert.DeserializeObject<DataPoint<T>>(await response.Content.ReadAsStringAsync());    
            }
            catch (Exception e) when (e is InvalidOperationException || e is HttpRequestException || e is TaskCanceledException)
            {
                throw new ConnectionException("Exception in http request.", e);
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Returns the previous DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with <code>T=string</code> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetPreviousDataAsync(string feed, IncludeData? include = null)
        {
            return await GetPreviousDataAsync<string>(feed, include);
        }

        /// <summary>
        /// Returns the previous DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<T>> GetPreviousDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "previous", include);
        }

        /// <summary>
        /// Returns the next DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with <code>T=string</code> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetNextDataAsync(string feed, IncludeData? include = null)
        {
            return await GetNextDataAsync<string>(feed, include);
        }

        /// <summary>
        /// Returns the next DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<T>> GetNextDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "next", include);
        }

        /// <summary>
        /// Returns the last DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with <code>T=string</code> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetLastDataAsync(string feed, IncludeData? include = null)
        {
            return await GetLastDataAsync<string>(feed, include);
        }

        /// <summary>
        /// Returns the last DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<T>> GetLastDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "last", include);
        }

        /// <summary>
        /// Returns the first DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with <code>T=string</code> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetFirstDataAsync(string feed, IncludeData? include = null)
        {
            return await GetFirstDataAsync<string>(feed, include);
        }

        /// <summary>
        /// Returns the first DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<T>> GetFirstDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "first", include);
        }

        /// <summary>
        /// Returns the DataPoint from Adafruit IO in the given feed <paramref name="feed"/> as specified
        /// by the given id <paramref name="id"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <param name="id">The id of the DataPoint that schould be retreived.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with <code>T=string</code> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetDataPointAsync(string feed, string id, IncludeData? include = null)
        {
            return await GetDataPointAsync<string>(feed, id, include);
        }

        /// <summary>
        /// Returns the DataPoint from Adafruit IO in the given feed <paramref name="feed"/> as specified
        /// by the given id <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <param name="include">The metadata that schould be included in the datapoints.</param>
        /// <param name="id">The id of the DataPoint that schould be retreived.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<T>> GetDataPointAsync<T>(string feed, string id, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, id, include);
        }

        /// <summary>
        /// Gets the most recent datapoint.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <returns>The most recent <see cref="DataPoint{T}"/> with <code>T=string</code> of the given feed.</returns>
        public async Task<DataPoint<string>> GetMostRecentDataPointAsync(string feed)
        {
            return await GetMostRecentDataPointAsync<string>(feed);
        }

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

        /// <summary>
        /// Gets the most recent datum of the feed.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <returns>The most recent <see cref="Datum{T}"/> with <code>T=string</code> 
        /// of the given feed.</returns>
        public async Task<Datum<string>> GetMostRecentDataAsync(string feed)
        {
            return await GetMostRecentDataAsync<string>(feed);
        }

        /// <summary>
        /// Gets the most recent datum of the feed.
        /// </summary>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data schould be retrieved.</param>
        /// <returns>The most recent <see cref="Datum{T}"/> of the given feed.</returns>
        public async Task<Datum<T>> GetMostRecentDataAsync<T>(string feed)
        {
            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://io.adafruit.com/api/v2/{account.Username}/feeds/{feed}/data/retain")
                };

                var response = await Client.GetAsync(request.RequestUri);
                await RaiseErrorIfHttpResponseErrorAsync(response, feed);

                string res = await response.Content.ReadAsStringAsync();
                res = res.Trim('\n', '\r');
                int lastIndex1 = res.LastIndexOf(',');
                int lastIndex2 = res.LastIndexOf(',', lastIndex1-1);
                int lastIndex3 = res.LastIndexOf(',', lastIndex2-1);

                float? ele = null, lon = null, lat = null;
                if (float.TryParse(res.Substring(lastIndex1 + 1, res.Length - lastIndex1 - 1), out float eleF))
                    ele = eleF;
                if (float.TryParse(res.Substring(lastIndex2 + 1, lastIndex1 - lastIndex2 - 1), out float lonF))
                    lon = lonF;
                if (float.TryParse(res.Substring(lastIndex3 + 1, lastIndex2 - lastIndex3 - 1), out float latF))
                    lat = latF;
                T t;
                if (typeof(T) == typeof(string))
                    t = JsonConvert.DeserializeObject<T>("\"" + res.Substring(0, lastIndex3) + "\"");
                else
                    t = JsonConvert.DeserializeObject<T>(res.Substring(0, lastIndex3));
                return new Datum<T>(t, lat, lon, ele, null);
            }
            catch (Exception e) when (e is InvalidOperationException || e is HttpRequestException || e is TaskCanceledException)
            {
                throw new ConnectionException("Exception in http request.", e);
            }
            catch (Exception e) { throw e; }
        }

        /*public string UpdateDataPoint(string feed, string id, string datum)
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
            HttpResultException inner = new HttpResultException($"Error Response Status-Code (HTTP): {response.StatusCode}. " +
                                                                $"Content of the Response is: {await response.Content.ReadAsStringAsync()}.");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                if ((await response.Content.ReadAsStringAsync()).
                    Equals("{\"error\":\"request failed - invalid API key provided\"}", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidApiKeyException(account.Username, inner);
                else
                    throw inner;
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                if ((await response.Content.ReadAsStringAsync()).
                    Equals("{\"error\":\"not found - that username does not exist\"}", StringComparison.OrdinalIgnoreCase))
                    throw new UsernameNotFoundException(account.Username, inner);
                else
                    throw new GeneralNotFoundException(account.Username, feed, inner);
            throw inner;
        }
    }
}