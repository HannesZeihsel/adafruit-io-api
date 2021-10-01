using System;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AdafruitIOApi.Http.Parameters
{
    /// <summary>
    /// Represents a datum that schould be created and added to one of the feeds of Adafruit IO
    /// </summary>
    /// <typeparam name="T">The Type of Data that schould be added.</typeparam>
    [Serializable]
    public class Datum<T> : ISerializable
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

        protected Datum(SerializationInfo info, StreamingContext context)
        {
            if(typeof(T) == typeof(string))
            {
                Value = this.Deserialize<T>(info, nameof(Value));
            }
            else
            {
                string value = this.Deserialize<string>(info, nameof(Value));
                Value = JsonConvert.DeserializeObject<T>(value);
            }

            Lat = this.Deserialize<float?>(info, nameof(Lat));
            Lon = this.Deserialize<float?>(info, nameof(Lon));
            Ele = this.Deserialize<float?>(info, nameof(Ele));
            CreatedAt = this.Deserialize<DateTime?>(info, nameof(CreatedAt));
        }

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
        /// Used to serialize this object. The value is serialized as string to add the data as string to adafruit IO.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(typeof(T) == typeof(string))
            {
                info.AddValue(this.GetSerializableName(nameof(Value)), Value);
            }
            else
            {
                info.AddValue(this.GetSerializableName(nameof(Value)), JsonConvert.SerializeObject(Value));
            }

            info.AddValue(this.GetSerializableName(nameof(Lat)), Lat);
            info.AddValue(this.GetSerializableName(nameof(Lon)), Lon);
            info.AddValue(this.GetSerializableName(nameof(Ele)), Ele);
            info.AddValue(this.GetSerializableName(nameof(CreatedAt)), CreatedAt);
        }
    }

    //todo cleanup comment
    public static class SerializableExtension
    {
        public static string GetSerializableName(this ISerializable serializable, string propertyName)
        {
            string jsonName = serializable.GetType().GetProperty(propertyName).GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
            return string.IsNullOrEmpty(jsonName) ? propertyName : jsonName;
        }

        public static TU Deserialize<TU>(this ISerializable serializable, SerializationInfo info, string propertyName)
        {
            try
            {
                return (TU)info.GetValue(serializable.GetSerializableName(propertyName),typeof(TU));
            }
            catch(SerializationException)
            {
                return default;
            }
        }
    }
}