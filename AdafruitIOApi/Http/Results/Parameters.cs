using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace AdafruitIOApi.Http.Results
{
    [Serializable]
    public class Parameters
    {
        [JsonProperty("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonProperty("end_time")]
        public DateTime? EndTime { get; set; }
        public int? Resolution { get; set; }
        public int? Hours { get; set; }
        public string Field { get; set; }
    }
}