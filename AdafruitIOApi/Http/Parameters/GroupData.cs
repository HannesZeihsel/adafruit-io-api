using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdafruitIOApi.Http.Parameters
{
    //todo cleanup and comment
    [Serializable]
    public class GroupData<T>
    {
        [JsonProperty("feeds")]
        public List<Feed<T>> Feeds { get; set; } = new List<Feed<T>>();
        
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("location")]
        public Location? Location { get; set; }
    }

    [Serializable]
    public class Location
    {
        [JsonProperty("lat")]
        public float? Lat { get; set; }

        [JsonProperty("lon")]
        public float? Lon { get; set; }

        [JsonProperty("ele")]
        public float? Ele { get; set; }
    }

    [Serializable]
    public class Feed<T>
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonProperty("value")]
        public T Value { get; set; }

        public Feed(string key, T value)
        {
            Key = key;
            Value = value;
        }
    }

}
