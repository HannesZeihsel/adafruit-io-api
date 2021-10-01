using System;

namespace AdafruitIOApi.Http.Results
{
    [Serializable]
    public class Feed
    {
        public int? Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }
}