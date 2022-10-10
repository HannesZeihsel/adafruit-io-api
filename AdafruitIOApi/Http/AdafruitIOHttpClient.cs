using System;
using System.Collections.Generic;
using System.Net;
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
    /// Provides methods to access to the HTTP API of Adafruit IO. This handles the creation of 
    /// the HTTP requests and the interpretation and conversion of the received data.
    /// </summary>
    public class AdafruitIOHttpClient
    {
        private const string HeaderNameApiKey = "X-AIO-Key";
        private readonly AdafruitIOAccount _account;
        
        //The http client managing the communication with Adafruit IO.
        private HttpClient Client { get; set; }

        #region Helpers
        private async Task<string> ManageRequest(HttpMethod method, string requestUri, string? content = null)
        {
            requestUri = @"https://io.adafruit.com/api/v2/" + requestUri;
            var request = new HttpRequestMessage(method, requestUri);
            if (!(content is null))
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await Client.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return responseString;

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException($"server response was '{responseString}'");
                case HttpStatusCode.NotFound:
                    if (responseString.Equals("{\"error\":\"not found - that username does not exist\"}"))
                        throw new UnauthorizedException(
                            $"Username is incorrect response from server was '{responseString}'");
                    throw new NotFoundException($"server response was '{responseString}'");
                case (HttpStatusCode)429:
                    throw new TooManyRequestsException($"server response was '{responseString}'");
                default:
                    throw new HttpRequestException(
                        $"Request to server not successful. HTTP Response Code {response.StatusCode}. " +
                        $"Content was '{responseString}'");
            };
        }
        #endregion


        /// <summary>
        /// Creates a new <see cref="AdafruitIOHttpClient"/> instance with the given account 
        /// and initializes the HttpClient.
        /// </summary>
        /// <param name="account">The Adafruit IO account that should be accessed.</param>
        public AdafruitIOHttpClient(AdafruitIOAccount account) {
            this._account = account ?? throw new ArgumentNullException(nameof(account));
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Add(HeaderNameApiKey, account.Key);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region Data

        /// <summary>
        /// Adds the value <paramref name="data"/>P to the provided feed 
        /// <paramref name="feed"/> on Adafruit IO.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to which the datum should be added to.</param>
        /// <param name="data">The data which should be added to the feed.</param>
        /// <returns>The added <see cref="DataPoint{T}"/> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<DataPoint<string>> CreateDataAsync(string feed, string data)
        {
            return await CreateDataAsync(feed, new Datum<string>(data));
        }

        /// <inheritdoc cref="CreateDataAsync(string, string)"/>
        /// <typeparam name="T">The type of the data that that should be added.</typeparam>
        public async Task<DataPoint<T>> CreateDataAsync<T>(string feed, T data)
        {
            return await CreateDataAsync(feed, new Datum<T>(data));
        }

        /// <inheritdoc cref="CreateDataAsync{T}(string, T)"/>
        /// <param name="datum">The datum that should be added to the feed.</param>
        public async Task<DataPoint<T>> CreateDataAsync<T>(string feed, Datum<T> datum)
        {
            return await CreateDataAsync(feed, null, datum);
        }
        
        /// <summary>
        /// Warning: Pagination not yet supported.
        /// Returns all the DataPoints from Adafruit IO in the given feed <paramref name="feed"/> which
        /// adhere to the given additional constrains if any.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <param name="group">The name of the group of which the feed is part of or <code>null</code>
        /// if the feed is not part of a group.</param>
        /// <param name="startTime">The beginning of the interval that the data should be in.</param>
        /// <param name="endTime">The end of the interval that the data should be in.</param>
        /// <param name="limit">The limit of the number DataPoints that should be returned</param>
        /// <param name="include">The metadata that should be included in the DataPoints.</param>
        /// <param name="before">TODO: check meaning of this parameter.</param>
        /// <returns>A List of <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<List<DataPoint<string>>> GetDataAsync(string feed, string? group = null,
               DateTime? startTime = null,
               DateTime? endTime = null,
               int? limit = null,
               IncludeData? include = null,
               string? before = null)
        {
            return await GetDataAsync<string>(feed, group, startTime, endTime, limit, include, before);
        }

        /// <inheritdoc cref="GetDataAsync(string, string?, DateTime?, DateTime?, int?, IncludeData?, string)"/>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        public async Task<List<DataPoint<T>>> GetDataAsync<T>(string feed, string? group = null,
            DateTime? startTime= null, 
            DateTime? endTime = null,
            int? limit = null, 
            IncludeData? include = null, 
            string? before= null)
        {
            //todo use before internally for pagination? 
            //setup query parameters if those are provided
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            if (startTime.HasValue)
                queryParameters["start_time"] = startTime.Value.ToString("s");
            if (endTime.HasValue)
                queryParameters["end_time"] = endTime.Value.ToString("s");
            if (limit.HasValue)
                queryParameters["limit"] = limit.ToString();
            if (include.HasValue)
                queryParameters["include"] = include.Value.GetDataText();
            if (!(before is null))
                queryParameters["before"] = before;
            string queryParameterString = queryParameters.Count <= 0 ? "" : "?" + queryParameters;

            string response = await ManageRequest(HttpMethod.Get,
                $"{_account.Username}/{(group == null ? "" : $"groups/{group}/")}feeds/{feed}/data{queryParameterString}");
            return JsonConvert.DeserializeObject<List<DataPoint<T>>>(response);
        }


        /// <summary>
        /// Get chart data based upon the passed parameters to use as data to display a chart.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <param name="startTime">The beginning of the interval that the data should be in.</param>
        /// <param name="endTime">The end of the interval that the data should be in.</param>
        /// <param name="resolution">The resolution representing the size of the aggregate slice.</param>
        /// <param name="hours">The number of hours to include in the data (Will be ignored if 
        /// <paramref name="startTime"/> or <paramref name="endTime"/> are given).</param>
        /// <param name="field">The type of aggregation to use to calculate the data in each slice.</param>
        /// <param name="raw">If true it forces the data to be returned raw. Not compatible with 
        /// <paramref name="field"/> or <paramref name="resolution"/>.</param>
        /// <returns>The ChartData as requested and defined by the parameters.</returns>
        public async Task<ChartData> GetChartFeedDataAsync(string feed, 
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

            string response = await ManageRequest(HttpMethod.Get,
                $"{_account.Username}/feeds/{feed}/data/chart{queryParameterString}");
            return JsonConvert.DeserializeObject<ChartData>(response);
        }

        /// <summary>
        /// Adds all the provided Data as multiple values to the given feed. 
        /// </summary>
        /// <typeparam name="T">The typo of data to be added.</typeparam>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to which the datum should be added to.</param>
        /// <param name="data">The list of data which should be added to the feed.</param>
        /// <returns>The added List of <see cref="DataPoint{T}"/> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<List<DataPoint<T>>> CreateMultipleDataRecordsAsync<T>(string feed, List<Datum<T>> data)
        {
            return await CreateMultipleDataRecordsAsync(feed, null, data);
        }


        /// <summary>
        /// Helper methode to get one DataPoint back from Adafruit that requires only include parameters and a special path.
        /// </summary>
        /// <typeparam name="T">The type of the expected value.</typeparam>
        /// <param name="feed">The feed from which the data should be retrieved.</param>
        /// <param name="special">The special path pointing to the wanted data.</param>
        /// <param name="include">The include parameters that should be present in the result.</param>
        /// <returns>The DataPoint as returned by adafruit.</returns>
        private async Task<DataPoint<T>> GetSpecialDataAsync<T>(string feed, string special,  IncludeData? include = null)
        {
            //setup query parameters if those are provided
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            if (!(include is null))
                queryParameters["include"] = include.Value.GetDataText();
            string queryParameterString = queryParameters.Count <= 0 ? "" : "?" + queryParameters;

            string response = await ManageRequest(HttpMethod.Get,
                $"{_account.Username}/feeds/{feed}/data/{special}/{queryParameterString}");
            return JsonConvert.DeserializeObject<DataPoint<T>>(response);    
        }

        
        /// <summary>
        /// Returns the previous DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <param name="include">The metadata that should be included in the DataPoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with <code>T=string</code> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetPreviousDataAsync(string feed, IncludeData? include = null)
        {
            return await GetPreviousDataAsync<string>(feed, include);
        }

        /// <inheritdoc cref="GetPreviousDataAsync(string, IncludeData?)"/>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        public async Task<DataPoint<T>> GetPreviousDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "previous", include);
        }


        /// <summary>
        /// Returns the next DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <param name="include">The metadata that should be included in the DataPoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetNextDataAsync(string feed, IncludeData? include = null)
        {
            return await GetNextDataAsync<string>(feed, include);
        }

        /// <inheritdoc cref="GetNextDataAsync(string, IncludeData?)"/>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        public async Task<DataPoint<T>> GetNextDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "next", include);
        }

        
        /// <summary>
        /// Returns the last DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <param name="include">The metadata that should be included in the DataPoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetLastDataAsync(string feed, IncludeData? include = null)
        {
            return await GetLastDataAsync<string>(feed, include);
        }

        /// <inheritdoc cref="GetLastDataAsync(string, IncludeData?)"/>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        public async Task<DataPoint<T>> GetLastDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "last", include);
        }


        /// <summary>
        /// Returns the first DataPoint from Adafruit IO in the given feed <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <param name="include">The metadata that should be included in the DataPoints.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with <code>T=string</code> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetFirstDataAsync(string feed, IncludeData? include = null)
        {
            return await GetFirstDataAsync<string>(feed, include);
        }

        /// <inheritdoc cref="GetFirstDataAsync(string, IncludeData?)"/>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        public async Task<DataPoint<T>> GetFirstDataAsync<T>(string feed, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, "first", include);
        }


        /// <summary>
        /// Gets the most recent data of the feed.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <returns>The most recent <see cref="Datum{T}"/> of the given feed.</returns>
        public async Task<Datum<string>> GetMostRecentDataAsync(string feed)
        {
            return await GetMostRecentDataAsync<string>(feed);
        }

        /// <inheritdoc cref="GetMostRecentDataAsync(string)"/>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        public async Task<Datum<T>> GetMostRecentDataAsync<T>(string feed)
        {
            string response = await ManageRequest(HttpMethod.Get, $"{_account.Username}/feeds/{feed}/data/retain");
            response = response.Trim('\n', '\r');
            int lastIndex1 = response.LastIndexOf(',');
            int lastIndex2 = response.LastIndexOf(',', lastIndex1-1);
            int lastIndex3 = response.LastIndexOf(',', lastIndex2-1);

            float? ele = null, lon = null, lat = null;
            if (float.TryParse(response.Substring(lastIndex1 + 1, response.Length - lastIndex1 - 1), out float eleF))
                ele = eleF;
            if (float.TryParse(response.Substring(lastIndex2 + 1, lastIndex1 - lastIndex2 - 1), out float lonF))
                lon = lonF;
            if (float.TryParse(response.Substring(lastIndex3 + 1, lastIndex2 - lastIndex3 - 1), out float latF))
                lat = latF;
            T t = typeof(T) == typeof(string)
                ? JsonConvert.DeserializeObject<T>("\"" + response.Substring(0, lastIndex3) + "\"")
                : JsonConvert.DeserializeObject<T>(response.Substring(0, lastIndex3));
            return new Datum<T>(t, lat, lon, ele, null);
        }

        /// <summary>
        /// Returns the DataPoint from Adafruit IO in the given feed <paramref name="feed"/> as specified
        /// by the given id <paramref name="id"/>.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) from which the data should be retrieved.</param>
        /// <param name="include">The metadata that should be included in the DataPoints.</param>
        /// <param name="id">The id of the DataPoint that should be retrieved.</param>
        /// <returns>A <see cref="DataPoint{T}"/> with the retrieved data.</returns>
        public async Task<DataPoint<string>> GetDataByIdAsync(string feed, string id, IncludeData? include = null)
        {
            return await GetDataByIdAsync<string>(feed, id, include);
        }

        /// <inheritdoc cref="GetDataByIdAsync(string, string, IncludeData?)"/>
        /// <typeparam name="T">The expected type of the data's value.</typeparam>
        public async Task<DataPoint<T>> GetDataByIdAsync<T>(string feed, string id, IncludeData? include = null)
        {
            return await GetSpecialDataAsync<T>(feed, id, include);
        }


        /// <summary>
        /// Updates the value <paramref name="data"/> of the datum with the given id 
        /// <paramref name="id"/> on the provided feed <paramref name="feed"/> on Adafruit IO.
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) at which the update should be performed.</param>
        /// <param name="id">The id of the datum that should be updated.</param>
        /// <param name="data">The new data to which the datapoint should be updated to</param>
        /// <returns>The updated <see cref="DataPoint{T}"/> that was returned by Adafruit IO 
        /// after the update.</returns>
        public async Task<DataPoint<string>> UpdateDataAsync(string feed, string id, string data)
        {
            return await UpdateDataAsync(feed, id, new Datum<string>(data));
        }

        /// <inheritdoc cref="UpdateDataAsync(string, string, string)"/>
        /// <typeparam name="T">The type of the data that that should be updated.</typeparam>
        public async Task<DataPoint<T>> UpdateDataAsync<T>(string feed, string id, T data)
        {
            return await UpdateDataAsync(feed, id, new Datum<T>(data));
        }

        /// <inheritdoc cref="UpdateDataAsync{T}(string, string, T)"/>
        /// <param name="datum">The new datum which should be updated.</param>
        public async Task<DataPoint<T>> UpdateDataAsync<T>(string feed, string id, Datum<T> datum)
        {
            string response = await ManageRequest(HttpMethod.Put, $"{_account.Username}/feeds/{feed}/data/{id}",
                    JsonConvert.SerializeObject(datum));
            return JsonConvert.DeserializeObject<DataPoint<T>>(response);
        }

        /// <summary>
        /// Deletes the datum on the provided feed <paramref name="feed" /> with the given id
        /// <paramref name="id" />
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) at which the datum should be deleted.</param>
        /// <param name="id">The id of the datum that should be deleted.</param>
        public async Task DeleteDataAsync(string feed, string id)
        {
            await ManageRequest(HttpMethod.Delete, $"{_account.Username}/feeds/{feed}/data/{id}");
        }

        /// <summary>
        /// Create group data for the given group. (Multiple values on multiple feeds are possible)
        /// </summary>
        /// <param name="group">The group on which to add the data</param>
        /// <param name="data">The group data that should be added</param>
        /// <returns>The list of created DataPoints returned.</returns>
        public async Task<List<DataPoint<T>>> CreateGroupDataAsync<T>(string group, GroupData<T> data)
        {
            string response = await ManageRequest(HttpMethod.Post, $"{_account.Username}/groups/{group}/data",
                    JsonConvert.SerializeObject(data));
            return JsonConvert.DeserializeObject<List<DataPoint<T>>>(response);
        }

        /// <inheritdoc cref="CreateDataAsync(string, string)"/>
        /// <param name="group">The group to which the feed belongs.</param>
        public async Task<DataPoint<string>> CreateDataAsync(string feed, string group, string data)
        {
            return await CreateDataAsync(feed, group, new Datum<string>(data));
        }

        /// <inheritdoc cref="CreateDataAsync(string, string, string)"/>
        /// <typeparam name="T">The type of the data that that should be added.</typeparam>
        public async Task<DataPoint<T>> CreateDataAsync<T>(string feed, string group, T data)
        {
            return await CreateDataAsync(feed, group, new Datum<T>(data));
        }

        /// <inheritdoc cref="CreateDataAsync{T}(string, string, T)"/>
        /// <param name="datum">The datum that schould be added to the feed.</param>
        public async Task<DataPoint<T>> CreateDataAsync<T>(string feed, string? group, Datum<T> datum)
        {
            string response = await ManageRequest(HttpMethod.Post, 
                    $"{_account.Username}/{(group == null ? "" : $"groups/{group}/")}feeds/{feed}/data",
                    JsonConvert.SerializeObject(datum));
            return JsonConvert.DeserializeObject<DataPoint<T>>(response);
        }


        /// <summary>
        /// Adds all the provided Data as multiple values to the given feed. 
        /// </summary>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to which the datum should be added to.</param>
        /// <param name="group">The name of the group of which the feed is part of.</param>
        /// <param name="data">The list of data which should be added to the feed.</param>
        /// <returns>The added List of <see cref="DataPoint{T}"/> that was returned by Adafruit IO 
        /// after the creation.</returns>
        public async Task<List<DataPoint<string>>> CreateMultipleDataRecordsAsync(string feed, string? group, List<Datum<string>> data)
        {
            return await CreateMultipleDataRecordsAsync<string>(feed, group, data);
        }

        /// <inheritdoc cref="CreateMultipleDataRecordsAsync(string, string?, List{Datum{string}})"/>
        /// <param name="feed">The name of the feed (of the Adafruit IO account this Client is
        /// communicating with) to which the datum should be added to.</param>
        public async Task<List<DataPoint<T>>> CreateMultipleDataRecordsAsync<T>(string feed, string? group, List<Datum<T>> data)
        {
            var batchData = new {data = data};
            string request = await ManageRequest(HttpMethod.Post, 
                $"{_account.Username}/{(group == null ? "" : $"groups/{group}/")}feeds/{feed}/data/batch",
                JsonConvert.SerializeObject(batchData));
            return JsonConvert.DeserializeObject<List<DataPoint<T>>>(request);
        }

        #endregion Data
    }
}