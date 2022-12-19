using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile.Internal
{
    public static class DateTimeExt
    {
        /// <summary>
        /// Converts both DateTime objects to local time and subtracts the subtrahend from the minuend.
        /// </summary>
        /// <returns>The timespan difference between two datetime objects.</returns>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        public static TimeSpan SameTimeZoneSubtract(this DateTime minuend, DateTime subtrahend)
        {
            return minuend.ToLocalTime().Subtract(subtrahend.ToLocalTime());
        }
    }
}