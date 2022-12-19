#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.Foundation
{
    /// <summary>
    /// An object that specifies a date or time in terms of units (such as year, month, 
    /// day, hour, and minute) to be evaluated in a calendar system and time zone.
    /// </summary>
    internal class NSDateComponents : iOSObjectProxy
    {
        private bool? mValidDate;
        private int? mEra;
        private int? mYear;
        private int? mYearForWeekOfYear;
        private int? mQuarter;
        private int? mMonth;
        private bool? mLeapMonth;
        private int? mWeekday;
        private int? mWeekdayOrdinal;
        private int? mWeekOfMonth;
        private int? mWeekOfYear;
        private int? mDay;
        private int? mHour;
        private int? mMinute;
        private int? mSecond;
        private int? mNanosecond;

        internal NSDateComponents(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        public NSDateComponents()
            : base(C.NSDateComponents_new())
        {
            CFFunctions.CFRelease(SelfPtr());
        }

        /// <summary>
        /// A Boolean value that indicates whether the current combination of properties represents a date which exists in the current calendar.
        /// </summary>
        /// <value><c>true</c> if valid date; otherwise, <c>false</c>.</value>
        public bool ValidDate
        {
            get
            {
                if (mValidDate == null)
                    mValidDate = C.NSDateComponents_validDate(SelfPtr());
                return mValidDate.Value;
            }
        }

        /// <summary>
        /// The number of eras.
        /// </summary>
        /// <value>The era.</value>
        public int Era
        {
            get
            {
                if (mEra == null)
                    mEra = C.NSDateComponents_era(SelfPtr());
                return mEra.Value;
            }
            set
            {
                mEra = value;
                C.NSDateComponents_setEra(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of years.
        /// </summary>
        /// <value>The year</value>
        public int Year
        {
            get
            {
                if (mYear == null)
                    mYear = C.NSDateComponents_year(SelfPtr());
                return mYear.Value;
            }
            set
            {
                mYear = value;
                C.NSDateComponents_setYear(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The ISO 8601 week-numbering year.
        /// </summary>
        /// <value>The year for week of year.</value>
        public int YearForWeekOfYear
        {
            get
            {
                if (mYearForWeekOfYear == null)
                    mYearForWeekOfYear = C.NSDateComponents_yearForWeekOfYear(SelfPtr());
                return mYearForWeekOfYear.Value;
            }
            set
            {
                mYearForWeekOfYear = value;
                C.NSDateComponents_setYearForWeekOfYear(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of quarters.
        /// </summary>
        /// <value>The quarter.</value>
        public int Quarter
        {
            get
            {
                if (mQuarter == null)
                    mQuarter = C.NSDateComponents_quarter(SelfPtr());
                return mQuarter.Value;
            }
            set
            {
                mQuarter = value;
                C.NSDateComponents_setQuarter(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of months.
        /// </summary>
        /// <value>The month</value>
        public int Month
        {
            get
            {
                if (mMonth == null)
                    mMonth = C.NSDateComponents_month(SelfPtr());
                return mMonth.Value;
            }
            set
            {
                mMonth = value;
                C.NSDateComponents_setMonth(SelfPtr(), value);
            }
        }

        /// <summary>
        /// A Boolean value that indicates whether the month is a leap month.
        /// </summary>
        /// <value><c>true</c> if leap month; otherwise, <c>false</c>.</value>
        public bool LeapMonth
        {
            get
            {
                if (mLeapMonth == null)
                    mLeapMonth = C.NSDateComponents_isLeapMonth(SelfPtr());
                return mLeapMonth.Value;
            }
            set
            {
                mLeapMonth = value;
                C.NSDateComponents_leapMonth(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of the weekdays.
        /// </summary>
        /// <value>The weekday.</value>
        public int Weekday
        {
            get
            {
                if (mWeekday == null)
                    mWeekday = C.NSDateComponents_weekday(SelfPtr());
                return mWeekday.Value;
            }
            set
            {
                mWeekday = value;
                C.NSDateComponents_setWeekday(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The ordinal number of weekdays.
        /// </summary>
        /// <value>The weekday ordinal.</value>
        public int WeekdayOrdinal
        {
            get
            {
                if (mWeekdayOrdinal == null)
                    mWeekdayOrdinal = C.NSDateComponents_weekdayOrdinal(SelfPtr());
                return mWeekdayOrdinal.Value;
            }
            set
            {
                mWeekdayOrdinal = value;
                C.NSDateComponents_setWeekdayOrdinal(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The week number of the months.
        /// </summary>
        /// <value>The week of month.</value>
        public int WeekOfMonth
        {
            get
            {
                if (mWeekOfMonth == null)
                    mWeekOfMonth = C.NSDateComponents_weekOfMonth(SelfPtr());
                return mWeekOfMonth.Value;
            }
            set
            {
                mWeekOfMonth = value;
                C.NSDateComponents_setWeekOfMonth(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The ISO 8601 week date of the year.
        /// </summary>
        /// <value>The week of year.</value>
        public int WeekOfYear
        {
            get
            {
                if (mWeekOfYear == null)
                    mWeekOfYear = C.NSDateComponents_weekOfYear(SelfPtr());
                return mWeekOfYear.Value;
            }
            set
            {
                mWeekOfYear = value;
                C.NSDateComponents_setWeekOfYear(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of days.
        /// </summary>
        /// <value>The day</value>
        public int Day
        {
            get
            {
                if (mDay == null)
                    mDay = C.NSDateComponents_day(SelfPtr());
                return mDay.Value;
            }
            set
            {
                mDay = value;
                C.NSDateComponents_setDay(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of hour units for the receiver.
        /// </summary>
        /// <value>The hour.</value>
        public int Hour
        {
            get
            {
                if (mHour == null)
                    mHour = C.NSDateComponents_hour(SelfPtr());
                return mHour.Value;
            }
            set
            {
                mHour = value;
                C.NSDateComponents_setHour(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of minute units for the receiver.
        /// </summary>
        /// <value>The minute.</value>
        public int Minute
        {
            get
            {
                if (mMinute == null)
                    mMinute = C.NSDateComponents_minute(SelfPtr());
                return mMinute.Value;
            }
            set
            {
                mMinute = value;
                C.NSDateComponents_setMinute(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of second units for the receiver.
        /// </summary>
        /// <value>The second.</value>
        public int Second
        {
            get
            {
                if (mSecond == null)
                    mSecond = C.NSDateComponents_second(SelfPtr());
                return mSecond.Value;
            }
            set
            {
                mSecond = value;
                C.NSDateComponents_setSecond(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The number of nanosecond units for the receiver.
        /// </summary>
        /// <value>The nanosecond.</value>
        public int Nanosecond
        {
            get
            {
                if (mNanosecond == null)
                    mNanosecond = C.NSDateComponents_nanosecond(SelfPtr());
                return mNanosecond.Value;
            }
            set
            {
                mNanosecond = value;
                C.NSDateComponents_setNanosecond(SelfPtr(), value);
            }
        }

        #region C wrapper

        private static class C
        {
            // Constructor
            [DllImport("__Internal")]
            internal static extern /* NSDateComponents */IntPtr NSDateComponents_new();

            // Validating a Date
            [DllImport("__Internal")]
            internal static extern bool NSDateComponents_validDate(HandleRef self);

            // Accessing Years and Months
            [DllImport("__Internal")]
            internal static extern int NSDateComponents_era(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setEra(HandleRef pointer, int era);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_year(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setYear(HandleRef pointer, int year);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_yearForWeekOfYear(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setYearForWeekOfYear(HandleRef pointer, int yearForWeekOfYear);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_quarter(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setQuarter(HandleRef pointer, int quarter);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_month(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setMonth(HandleRef pointer, int month);

            [DllImport("__Internal")]
            internal static extern bool NSDateComponents_isLeapMonth(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_leapMonth(HandleRef pointer, bool isLeapMonth);

            // Accessing Weeks and Days
            [DllImport("__Internal")]
            internal static extern int NSDateComponents_weekday(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setWeekday(HandleRef pointer, int weekday);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_weekdayOrdinal(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setWeekdayOrdinal(HandleRef pointer, int weekdayOrdinal);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_weekOfMonth(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setWeekOfMonth(HandleRef pointer, int weekOfMonth);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_weekOfYear(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setWeekOfYear(HandleRef pointer, int weekOfYear);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_day(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setDay(HandleRef pointer, int day);

            // Accessing Hours and Seconds
            [DllImport("__Internal")]
            internal static extern int NSDateComponents_hour(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setHour(HandleRef pointer, int hour);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_minute(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setMinute(HandleRef pointer, int minute);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_second(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setSecond(HandleRef pointer, int second);

            [DllImport("__Internal")]
            internal static extern int NSDateComponents_nanosecond(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void NSDateComponents_setNanosecond(HandleRef pointer, int nanosecond);
        }

        #endregion
    }
}
#endif