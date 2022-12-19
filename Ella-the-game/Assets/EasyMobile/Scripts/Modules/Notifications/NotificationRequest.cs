using System;

namespace EasyMobile
{
    public class NotificationRequest
    {
        /// <summary>
        /// The notification identifier.
        /// </summary>
        public string id;

        /// <summary>
        /// The content of the notification.
        /// </summary>
        public NotificationContent content;

        /// <summary>
        /// Next trigger date of the notification.
        /// </summary>
        public DateTime nextTriggerDate;

        /// <summary>
        /// The repeat mode of the notification.
        /// </summary>
        public NotificationRepeat repeat;

        public NotificationRequest(string id, NotificationContent content, DateTime nextTriggerDate, NotificationRepeat repeat)
        {
            this.id = id;
            this.content = content;
            this.nextTriggerDate = nextTriggerDate;
            this.repeat = repeat;
        }
    }
}

