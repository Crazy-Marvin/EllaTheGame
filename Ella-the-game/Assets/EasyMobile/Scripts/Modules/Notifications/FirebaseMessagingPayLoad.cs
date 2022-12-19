using System;
using System.Collections.Generic;

namespace EasyMobile
{
    public class FirebaseMessage
    {
        public string Error { get; private set; }

        public string Priority { get; private set; }

        public string MessageType { get; private set; }

        public string MessageId { get; set; }

        public string RawData { get; private set; }

        public IDictionary<string, string> Data { get; set; }

        public string CollapseKey { get; private set; }

        public string To { get; set; }

        public string From { get; private set; }

        public Uri Link { get; private set; }

        public FirebaseNotification Notification { get; private set; }

        public TimeSpan TimeToLive { get; private set; }

        public string ErrorDescription { get; private set; }

        public bool NotificationOpened { get; private set; }

        #if EM_FIR_MESSAGING
        internal FirebaseMessage(Firebase.Messaging.FirebaseMessage firebaseMessage)
        {
            Error = firebaseMessage.Error;
            Priority = firebaseMessage.Priority;
            MessageType = firebaseMessage.MessageType;
            MessageId = firebaseMessage.MessageId;
            RawData = firebaseMessage.RawData;
            Data = firebaseMessage.Data;
            CollapseKey = firebaseMessage.CollapseKey;
            To = firebaseMessage.To;
            From = firebaseMessage.From;
            Link = firebaseMessage.Link;
            Notification = new FirebaseNotification(firebaseMessage.Notification);
            TimeToLive = firebaseMessage.TimeToLive;
            ErrorDescription = firebaseMessage.ErrorDescription;
            NotificationOpened = firebaseMessage.NotificationOpened;
        }
        #endif

        public NotificationContent ToNotificationContent()
        {
            NotificationContent notificationContent = new NotificationContent
            {
            };

            if (Notification != null)
            {
                notificationContent.title = Notification.Title;
                notificationContent.body = Notification.Body;
                notificationContent.badge = GetNotificationContentBadge();
                notificationContent.largeIcon = Notification.Icon;
                notificationContent.smallIcon = Notification.Icon;
            }

            if (Data != null)
            {
                Dictionary<string, object> userInfo = new Dictionary<string, object>();

                foreach (var data in Data)
                {
                    userInfo.Add(data.Key, data.Value);
                }

                notificationContent.userInfo = userInfo;
            }

            return notificationContent;
        }

        public int GetNotificationContentBadge()
        {
            if (Notification == null)
                return -1;

            int result = -1;
            if (int.TryParse(Notification.Badge, out result))
            {
                return result;
            }
            else
            {
                return -1;
            }
        }
    }

    public class FirebaseNotification
    {
        public string Title { get; private set; }

        public string Body { get; private set; }

        public string Icon { get; private set; }

        public string Sound { get; private set; }

        public string Badge { get; private set; }

        public string Tag { get; private set; }

        public string Color { get; private set; }

        public string ClickAction { get; private set; }

        public string BodyLocalizationKey { get; private set; }

        public IEnumerable<string> BodyLocalizationArgs { get; private set; }

        public string TitleLocalizationKey { get; private set; }

        public IEnumerable<string> TitleLocalizationArgs { get; private set; }

        #if EM_FIR_MESSAGING
        internal FirebaseNotification(Firebase.Messaging.FirebaseNotification firebaseNotification)
        {
            Title = firebaseNotification != null ? firebaseNotification.Title : string.Empty;
            Body = firebaseNotification != null ? firebaseNotification.Body : string.Empty;
            Icon = firebaseNotification != null ? firebaseNotification.Icon : string.Empty;
            Sound = firebaseNotification != null ? firebaseNotification.Sound : string.Empty;
            Badge = firebaseNotification != null ? firebaseNotification.Badge : string.Empty;
            Tag = firebaseNotification != null ? firebaseNotification.Tag : string.Empty;
            Color = firebaseNotification != null ? firebaseNotification.Color : string.Empty;
            ClickAction = firebaseNotification != null ? firebaseNotification.ClickAction : string.Empty;
            BodyLocalizationKey = firebaseNotification != null ? firebaseNotification.BodyLocalizationKey : string.Empty;
            BodyLocalizationArgs = firebaseNotification != null ? firebaseNotification.BodyLocalizationArgs : null;
            TitleLocalizationKey = firebaseNotification != null ? firebaseNotification.TitleLocalizationKey : string.Empty;
            TitleLocalizationArgs = firebaseNotification != null ? firebaseNotification.TitleLocalizationArgs : null;
        }
        #endif
    }

    #if EM_FIR_MESSAGING
    public static class FirebaseMessageExtension
    {
        public static FirebaseMessage ToEasyMobileFirebaseMessage(this Firebase.Messaging.FirebaseMessage firebaseMessage)
        {
            return new FirebaseMessage(firebaseMessage);
        }
    }
    #endif
}
