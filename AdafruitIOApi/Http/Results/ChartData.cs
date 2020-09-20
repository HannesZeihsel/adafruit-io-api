using System;

namespace AdafruitIOApi.Http.Results
{
    [Serializable]
    public class ChartData
    {
        public Feed Feed { get; set; }
        public Parameters Parameters { get; set; }
        public string[] Columns { get; set; }
        public object[][] Data { get; set; }

        public string Storage { get; set; }
    }
}