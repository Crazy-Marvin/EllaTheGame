using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile.Internal.Notifications;

namespace EasyMobile
{
    public class NotificationContent
    {
        public const string DEFAULT_ANDROID_SMALL_ICON = "ic_stat_em_default";
        public const string DEFAULT_ANDROID_LARGE_ICON = "ic_large_em_default";

        /// <summary>
        /// The notification title.
        /// </summary>
        public string title;

        /// <summary>
        /// [iOS only] The notification subtitle.
        /// </summary>
        public string subtitle;

        /// <summary>
        /// The message displayed in the notification alert.
        /// </summary>
        public string body;

        /// <summary>
        /// [iOS only] The number to display as the app’s icon badge. 
        /// When the number in this property is 0 or smaller, the system does not display a badge. 
        /// When the number is greater than 0, the system displays the badge with the specified number.
        /// </summary>
        public int badge = 0;

        /// <summary>
        /// A dictionary to attach custom data to the notification. This is optional.
        /// </summary>
        public Dictionary<string, object> userInfo;

        /// <summary>
        /// The identifier of the category this notification belongs to.
        /// If no category is specified, the default one will be used.
        /// </summary>
        public string categoryId;

        /// <summary>
        /// [Android only] The small icon displayed on the notification.
        /// Give it a value only if you want to use a custom icon rather than the default one.
        /// </summary>
        public string smallIcon = DEFAULT_ANDROID_SMALL_ICON;

        /// <summary>
        /// [Android only] The large icon displayed on the notification.
        /// Give it a value only if you want to use a custom icon rather than the default one.
        /// </summary>
        public string largeIcon = DEFAULT_ANDROID_LARGE_ICON;
    }
}

