using System;

namespace AdafruitIOApi.Parameters
{
    /// <summary>
    /// Enum with Flags used to specify which (meta-)data schould be received from Adafruit IO.
    /// </summary>
    [Flags]
    public enum IncludeData
    {
        ///Include the value(s) of the DataPoint(s).
        Value=0,
        ///Include the latitude koordinate(s) of the DataPoint(s).
        Lat = 1,
        ///Include the longtitude koordinate(s) of the DataPoint(s).
        Lon = 2,
        ///Include the elevation koordinate(s) of the DataPoint(s).
        Ele = 4,
        ///Include the ID(s) of the DataPoint(s).
        ID = 8,
        ///Include the Time(s) of creation of the DataPoint(s).
        CreatedAt = 16
    }

    /// <summary>
    /// Defines useful extensions for the <see cref="IncludeData"/> enum.
    /// </summary>
    public static class IncludeDataExtensions
    {
        /// <summary>
        /// Converts the enum to the equivalent string that will be interpreted by Adafruit IO.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The converted string.</returns>
        public static string GetDataText(this IncludeData data)
        {
            //add all atributes to the string if present and seperate by ';'
            string value = "";
            if (data.HasFlag(IncludeData.Value))
                value += "value;";
            if (data.HasFlag(IncludeData.Lat))
                value += "lat;";
            if (data.HasFlag(IncludeData.Lon))
                value += "lon;";
            if (data.HasFlag(IncludeData.Ele))
                value += "ele;";
            if (data.HasFlag(IncludeData.ID))
                value += "id;";
            if (data.HasFlag(IncludeData.CreatedAt))
                value += "created_at;";
            //if any elements are present remove last ';' and return otherwise return empty string.
            return value.Length <= 0 ? "" : value.Substring(0, value.Length - 1);
        }
    }
}