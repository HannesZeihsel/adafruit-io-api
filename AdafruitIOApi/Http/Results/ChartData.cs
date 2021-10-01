using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using AdafruitIOApi.Http.Parameters;

namespace AdafruitIOApi.Http.Results
{
    [Serializable]
    public class ChartData:ISerializable
    {
        [JsonProperty("feed")]
        public Feed Feed { get; set; }
        
        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }
        
        [JsonProperty("columns")]
        public string[] Columns { get; set; }
        
        [JsonProperty("data")]
        public List<Datum<string>> Data { get; set; }

        [JsonProperty("storage")]
        public string Storage { get; set; }

        protected ChartData(SerializationInfo info, StreamingContext context)
        {
            Feed = this.Deserialize<Feed>(info, nameof(Feed));
            Parameters = this.Deserialize<Parameters>(info, nameof(Parameters));
            Columns = this.Deserialize<string[]>(info, nameof(Columns));
            string[][] dataString = this.Deserialize<string[][]>(info, nameof(Data));
            Data = new List<Datum<string>>(dataString.Length);
            foreach (var item in dataString)
            {
                Data.Add(new Datum<string>(item[1], createdAt: DateTime.Parse(item[0], CultureInfo.InvariantCulture)));
            }

            Storage = this.Deserialize<string>(info, nameof(Storage));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(this.GetSerializableName(nameof(Feed)), Feed);
            info.AddValue(this.GetSerializableName(nameof(Parameters)), Parameters);
            info.AddValue(this.GetSerializableName(nameof(Columns)), Columns);
            string[][] dataString = new string[Data.Count][];
            for (int i = 0; i < Data.Count; i++)
            {
                dataString[i] = new[] { Data[i].Value, Data[i].CreatedAt.Value.ToString("s") };
            }
            info.AddValue(this.GetSerializableName(nameof(Data)), Data);
            info.AddValue(this.GetSerializableName(nameof(Storage)), Storage);
        }
    }
}