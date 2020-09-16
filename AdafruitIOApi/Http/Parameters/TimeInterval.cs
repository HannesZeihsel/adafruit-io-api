using System;

namespace AdafruitIOApi.Parameters
{
    /// <summary>
    /// Represents an interval in time with a specific start and endpoint in time.
    /// </summary>
    public class TimeInterval
    {
        //todo maybe make Times optional. ie. only start time is used.

        ///The time representing the start time of this time interval.
        public DateTime StartTime { get; set; }
        
        ///The time representing the end time of this time interval.
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TimeInterval"/> class with the given start 
        /// and end times specifying the time interval.
        /// </summary>
        /// <param name="startTime">The point in time representing the begin of the time interval.</param>
        /// <param name="endTime">The point in time representing the end of the time interval.</param>
        public TimeInterval(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TimeInterval"/> class with the given start time and 
        /// the duration of the time interval.
        /// </summary>
        /// <param name="startTime">The point in time representing the begin of the time interval.</param>
        /// <param name="timeSpan">The timespan representing the length of the time interval.</param>
        public TimeInterval(DateTime startTime, TimeSpan timeSpan) : this(startTime, startTime + timeSpan) { }

        /// <summary>
        /// Initializes a new instance of <see cref="TimeInterval"/> class with the given duration and 
        /// end time of the time interval.
        /// </summary>
        /// <param name="timeSpan">The timespan representing the length of the time interval.</param>
        /// <param name="endTime">The point in time representing the end of the time interval.</param>
        public TimeInterval(TimeSpan timeSpan, DateTime endTime) : this(endTime - timeSpan, endTime) { }

        /// <summary>
        /// Checks if the given time is part of this time interval.
        /// </summary>
        /// <param name="moment">The moment in time that schould be checked.</param>
        /// <returns>True if the given moment in time lies within this time interval(both bounds inclusive).</returns>
        public bool Contains(DateTime moment)
        {
            return moment >= StartTime && moment <= EndTime;
        }

        /// <summary>
        /// Returns a time span representing the duration of this interval.
        /// </summary>
        /// <returns>The duration of this interval.</returns>
        public TimeSpan GetTimeSpan()
        {
            return EndTime - StartTime;
        }
    }
}