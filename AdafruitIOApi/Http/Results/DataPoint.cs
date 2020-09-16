using AdafruitIOApi.Parameters;
using Newtonsoft.Json;
using System;

namespace AdafruitIOApi.Results
{
    /// <summary>
    /// Represents a datapoint from the Arduino IO with its value of type <typeparamref name="T"/> and assosiated metadata.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DataPoint<T> : Datum<T>
    {
        ///The ID of this datapoint as metadata of this datum.
        [JsonProperty("id")]
        public string ID { get; set; }
        
        ///The feed ID of this datapoint as metadata of this datum.
        [JsonProperty("feed_id")]
        public int FeedID { get; set; }

        ///The feed key of this datapoint as metadata of this datum.
        [JsonProperty("feed_key")]
        public string FeedKey { get; set; }

        //todo check and implement content of location
        /*[JsonProperty("location")]
        public string Location { get; set; }*/

        ///The epoch of creation of the datapoint as metadata of this datum.
        [JsonProperty("created_epoch")]
        public int CreatedEpoch { get; set; }

        ///The expiration date of the datapoint as metadata of this datum.
        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Create a new instance of the <see cref="DataPoint{T}"/> class with the given value and metadata provided.
        /// </summary>
        /// <param name="value">The value of this datapoint (of type T)</param>
        /// <param name="lat">The latitude koordinate of the location as metadata of this datapoint. Will be omitted if <code>null</code>.</param>
        /// <param name="lon">The longtitude koordinate of the location as metadata of this datapoint. Will be omitted if <code>null</code>.</param>
        /// <param name="ele">The elevation koordinate of the location as metadata of this datapoint. Will be omitted if <code>null</code>.</param>
        /// <param name="createdAt">The time of creation as metadata of this datapoint. Will be omitted if <code>null</code>.</param>
        /// <param name="iD">The ID as metadata of this datapoint.</param>
        /// <param name="feedId">The feed ID as metadata of this datapoint.</param>
        /// <param name="feedKey">The feed key as metadata of this datapoint.</param>
        /// <param name="createdEpoch">The epoch of creation as metadata of this datapoint.</param>
        /// <param name="expiration">The expiration data as metadata of this datapoint.</param>
        public DataPoint(T value, float? lat, float? lon, float? ele,
                DateTime? createdAt, string iD, int feedId, string feedKey, 
                int createdEpoch, DateTime expiration):base(value, lat, lon, ele, createdAt)
        {
            ID = iD;
            FeedID = feedId;
            FeedKey = feedKey;
            CreatedEpoch = createdEpoch;
            Expiration = expiration;
        }

        /// <summary>
        /// Warning: Can only convert this DataPoint if T is of type string.
        /// Convert this DataPoint from <see cref="DataPoint{T}"/> with <code>T=string</code> to <see cref="DataPoint{U}"/>.
        /// </summary>
        /// <typeparam name="U">The type of the value of the returned <see cref="DataPoint{U}"/> object.</typeparam>
        /// <returns>The new <see cref="DataPoint{U}"/> that this <see cref="DataPoint{T}"/> was converted to.</returns>
        public DataPoint<U> ConvertTo<U>()
        {
            //todo maybe return null if convertion failed?
            if (typeof(T) == typeof(string))
                return new DataPoint<U>(JsonConvert.DeserializeObject<U>(Value.ToString()), Lat, Lon, Ele,
                            CreatedAt, ID, FeedID, FeedKey, CreatedEpoch, Expiration);
            else//todo find better way of handeling this.
                throw new Exception("Not possible to convert from non string type");
        }

        /// <summary>
        /// Generate a new instance of <see cref="DataPoint{T}"/> from the given .Json string, that is 
        /// formatted according to the Adafruit IO (The value is of type string and if T is of another 
        /// Type it will be encoded once more).
        /// </summary>
        /// <param name="json">The .Json string to be converted to the DataPoint. With the Value of 
        /// type T encoded as type string.</param>
        /// <returns>The generated <see cref="DataPoint{T}"/> from the passed .Json formatted string.</returns>
        static public DataPoint<T> GenerateFromJson(string json)
        {
            if (typeof(T) == typeof(string))   //if type string just deserialize and return
               return JsonConvert.DeserializeObject<DataPoint<T>>(json);
            else                               //else convert the value of string to T and construct and return the new DataPoint.
            {
                DataPoint<string> dat = JsonConvert.DeserializeObject<DataPoint<string>>(json);
                return new DataPoint<T>(JsonConvert.DeserializeObject<T>(dat.Value), dat.Lat, dat.Lon, dat.Ele, 
                            dat.CreatedAt, dat.ID, dat.FeedID, dat.FeedKey, dat.CreatedEpoch, dat.Expiration);
            }
        }
    }
}