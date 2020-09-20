using AdafruitIOApi.Parameters;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace AdafruitIOApi.Results
{
    /// <summary>
    /// Represents a datapoint from the Arduino IO with its value of type <typeparamref name="T"/> and assosiated metadata.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DataPoint<T> : Datum<T>, ISerializable
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


        //todo
        /// <summary>
        /// Constructor to construct new instance of <see cref="DataPoint{T}"/> from a serialized version. This deserializes 
        /// Value from string to type <typeparamref name="T"/> because it will be stored as string on adafruit io.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DataPoint(SerializationInfo info, StreamingContext context):base(info, context)
        {
            ID = this.Deserialize<string>(info, nameof(ID));
            FeedID = this.Deserialize<int>(info, nameof(FeedID));
            FeedKey = this.Deserialize<string>(info, nameof(FeedKey));
            CreatedEpoch = this.Deserialize<int>(info, nameof(CreatedEpoch));
            Expiration = this.Deserialize<DateTime>(info, nameof(Expiration));
        }

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
        /// Used to serialize this object. The value is serialized as string to add the data as string to adafruit IO.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(this.GetSerializableName(nameof(ID)), ID);
            info.AddValue(this.GetSerializableName(nameof(FeedID)), FeedID);
            info.AddValue(this.GetSerializableName(nameof(FeedKey)), FeedKey);
            info.AddValue(this.GetSerializableName(nameof(CreatedEpoch)), CreatedEpoch);
            info.AddValue(this.GetSerializableName(nameof(Expiration)), Expiration);
        }
    }
}