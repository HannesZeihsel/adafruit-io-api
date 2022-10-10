namespace AdafruitIOApi.Http.Parameters
{
    /// <summary>
    /// Enum to specify the aggregate field to be used for the chart feed Data function.
    /// </summary>
    public enum AggregateField
    {
        Avg,
        Sum,
        Val,
        Min,
        Max,
        ValCount
    }

    /// <summary>
    /// Defines useful extensions for the <see cref="AggregateField"/> enum.
    /// </summary>
    public static class AggregateFieldExtensions
    {
        /// <summary>
        /// Converts the enum to the equivalent string that will be interpreted by Adafruit IO.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The converted string.</returns>
        public static string GetDataText(this AggregateField data)
        {
            return data switch
            {
                AggregateField.Avg => "avg",
                AggregateField.Sum => "sum",
                AggregateField.Val => "val",
                AggregateField.Min => "min",
                AggregateField.Max => "max",
                AggregateField.ValCount => "val_count",
                _ => string.Empty,
            };
        }
    }
}