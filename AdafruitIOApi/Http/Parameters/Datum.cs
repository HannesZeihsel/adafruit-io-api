using Newtonsoft.Json;
using System;

namespace AdafruitIOApi.Parameters
{
    /// <summary>
    /// Represents a datum that schould be created and added to one of the feeds of Adafruit IO
    /// </summary>
    /// <typeparam name="T">The Type of Data that schould be added.</typeparam>
    [Serializable]
    public class Datum<T>
    {
        /// The value of this datum (of Type T).
        [JsonProperty("value")]
        public T Value { get; set; }

        ///The latitude koordinate of the location as metadata of this datum.
        [JsonProperty("lat")]
        public float? Lat { get; set; }

        ///The longtitude koordinate of the location as metadata of this datum.
        [JsonProperty("lon")]
        public float? Lon { get; set; }

        ///The elevation koordinate of the location as metadata of this datum.
        [JsonProperty("ele")]
        public float? Ele { get; set; }

        ///The time of creation as metadata of this datum.
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        
        /// <summary>
        /// Initializes a new instance <see cref="Datum{T}"/> with the given value.
        /// </summary>
        /// <param name="value">The value of this datum (of type T)</param>
        public Datum(T value) : this(value, null) { }

        /// <summary>
        /// Initializes a new instance <see cref="Datum{T}"/> with the given value and Time of creation.
        /// </summary>
        /// <param name="value">The value of this datum (of type T)</param>
        /// <param name="createdAt">The time of creation as metadata of this datum. Will be omitted if <code>null</code>.</param>
        public Datum(T value, DateTime? createdAt) : this(value, null, null, createdAt) { }

        /// <summary>
        /// Initializes a new instance <see cref="Datum{T}"/> with the given value and location koordinates.
        /// </summary>
        /// <param name="value">The value of this datum (of type T)</param>
        /// <param name="lat">The latitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="lon">The longtitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        public Datum(T value, float? lat, float? lon) : this(value, lat, lon, (float?)null) { }

        /// <summary>
        /// Initializes a new instance <see cref="Datum{T}"/> with the given value and location koordinates.
        /// </summary>
        /// <param name="value">The value of this datum (of type T)</param>
        /// <param name="lat">The latitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="lon">The longtitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="ele">The elevation koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        public Datum(T value, float? lat, float? lon, float? ele) : this(value, lat, lon, ele, null) { }

        /// <summary>
        /// Initializes a new instance <see cref="Datum{T}"/> with the given value, location koordinates and Time of creation.
        /// </summary>
        /// <param name="value">The value of this datum (of type T)</param>
        /// <param name="lat">The latitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="lon">The longtitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="createdAt">The time of creation as metadata of this datum. Will be omitted if <code>null</code>.</param>
        public Datum(T value, float? lat, float? lon, DateTime? createdAt) : this(value, lat, lon, null, createdAt) { }

        /// <summary>
        /// Initializes a new instance <see cref="Datum{T}"/> with the given value, location koordinates and Time of creation.
        /// </summary>
        /// <param name="value">The value of this datum (of type T)</param>
        /// <param name="lat">The latitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="lon">The longtitude koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="ele">The elevation koordinate of the location as metadata of this datum. Will be omitted if <code>null</code>.</param>
        /// <param name="createdAt">The time of creation as metadata of this datum. Will be omitted if <code>null</code>.</param>
        public Datum(T value, float? lat, float? lon, float? ele, DateTime? createdAt)
        {
            Value = value;
            Lat = lat;
            Lon = lon;
            Ele = ele;
            CreatedAt = createdAt;
        }

        /// <summary>
        /// Returns the datum formatted as .Json in the way Adafruit IO can process it.
        /// The value is stored as a string to prevent errors from Adafruit IO.
        /// </summary>
        /// <returns>The datum formattet as Adafruit IO compliant .Json.</returns>
        public virtual string GetJson()
        {
            if (typeof(T) == typeof(string))            //if value is string already just serialize it
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            else                                        //if not create new Datum that's value is the serialized Version of this Datum's value
                return new Datum<string>(JsonConvert.SerializeObject(Value), Lat, Lon, Ele, CreatedAt).GetJson();
        }
    }
}