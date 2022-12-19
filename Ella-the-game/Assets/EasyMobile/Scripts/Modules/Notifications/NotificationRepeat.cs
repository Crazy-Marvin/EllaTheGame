using System;

namespace EasyMobile
{
    public enum NotificationRepeat
    {
        None = 0,
        EveryMinute = 1,
        EveryHour = 2,
        EveryDay = 3,
        EveryWeek = 4
    }

    public static class NotificationRepeatExtension
    {
        public static double ToSecondInterval(this NotificationRepeat t)
        {
            switch (t)
            {
                case NotificationRepeat.EveryMinute:
                    return TimeSpan.FromMinutes(1).TotalSeconds;
                case NotificationRepeat.EveryHour:
                    return TimeSpan.FromHours(1).TotalSeconds;
                case NotificationRepeat.EveryDay:
                    return TimeSpan.FromDays(1).TotalSeconds;
                case NotificationRepeat.EveryWeek:
                    return TimeSpan.FromDays(7).TotalSeconds;
                default:
                    return TimeSpan.Zero.TotalSeconds;
            }
        }

        public static NotificationRepeat FromExactSecondInterval(double interval)
        {          
            var timeSpan = TimeSpan.FromSeconds(interval);

            if (timeSpan.TotalDays == 7)
                return NotificationRepeat.EveryWeek;
            else if (timeSpan.TotalDays == 1)
                return NotificationRepeat.EveryDay;
            else if (timeSpan.TotalHours == 1)
                return NotificationRepeat.EveryHour;
            else if (timeSpan.TotalMinutes == 1)
                return NotificationRepeat.EveryMinute;
            else
                return NotificationRepeat.None;
            
        }
    }
}

