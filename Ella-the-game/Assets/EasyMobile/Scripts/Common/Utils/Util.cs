using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile.Internal
{
    internal static class Util
    {
        public static readonly DateTime UnixEpoch =
            DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

        /// <summary>
        /// Determines if the current build is a development build.
        /// </summary>
        /// <returns><c>true</c> if is development build; otherwise, <c>false</c>.</returns>
        public static bool IsUnityDevelopmentBuild()
        {
            #if DEBUG || DEVELOPMENT_BUILD
            return true;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given value is null.
        /// </summary>
        /// <returns>The input value.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T NullArgumentTest<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(typeof(T).ToString());
            }

            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the indicated parameter name if the given value is null.
        /// </summary>
        /// <returns>The argument test.</returns>
        /// <param name="value">Value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T NullArgumentTest<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }

        /// <summary>
        /// Constructs a UTC <see cref="DateTime"/> from the milliseconds since Unix Epoch.
        /// </summary>
        /// <returns>The DateTime value in UTC.</returns>
        /// <param name="millisSinceEpoch">Milliseconds since Epoch.</param>
        public static DateTime FromMillisSinceUnixEpoch(long millisSinceEpoch)
        {
            return UnixEpoch.Add(TimeSpan.FromMilliseconds(millisSinceEpoch));
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to miliseconds.
        /// </summary>
        /// <returns>The milliseconds.</returns>
        /// <param name="span">Time span.</param>
        public static long ToMilliseconds(TimeSpan span)
        {
            double millis = span.TotalMilliseconds;

            if (millis > long.MaxValue)
            {
                return long.MaxValue;
            }

            if (millis < long.MinValue)
            {
                return long.MinValue;
            }

            return Convert.ToInt64(millis);
        }

        /// <summary>
        /// Removes all leading and trailing white-spaces from the input and returns.
        /// </summary>
        /// <returns>The trimmed identifier.</returns>
        /// <param name="id">Identifier.</param>
        public static string AutoTrimId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            return id.Trim();
        }
    }
}