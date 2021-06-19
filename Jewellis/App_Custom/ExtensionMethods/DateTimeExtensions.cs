using System;

namespace Jewellis
{
    /// <summary>
    /// Represents extension methods for the <see cref="DateTime"/> class.
    /// </summary>
    public static class DateTimeExtensions
    {

        /// <summary>
        /// Rounds up the datetime to the nearset specified time span.
        /// </summary>
        /// <param name="dateTime">The datetime to round</param>
        /// <param name="roundToNearset">The time span to round to.</param>
        /// <returns>Returns a new datetime rounded up to the nearset specified time span.</returns>
        public static DateTime RoundUp(this DateTime dateTime, TimeSpan roundToNearset)
        {
            return new DateTime((dateTime.Ticks + roundToNearset.Ticks - 1) / roundToNearset.Ticks * roundToNearset.Ticks, dateTime.Kind);
        }

    }
}
