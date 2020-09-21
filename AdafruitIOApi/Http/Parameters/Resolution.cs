namespace AdafruitIOApi.Http.Parameters
{
    /// <summary>
    /// Enum to specify the Resolution (The size of aggregation slize [values encoded in minutes])
    /// </summary>
    public enum Resolution:int
    {
        //todo check if there is a better naming convention
        OneMinute = 1,
        FifeMinutes = 5,
        TenMinutes = 10,
        ThirtyMinutes = 30,
        OneHour = 60,
        TwoHours = 120,
        FourHours = 240,
        EightHours = 480,
        SixteenHours = 960,
    }
}