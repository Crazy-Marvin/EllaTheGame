using System;

namespace EasyMobile
{
    public class Notification
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public readonly string id;

        /// <summary>
        /// The action identifier.
        /// </summary>
        public readonly string actionId;

        /// <summary>
        /// The notification content.
        /// </summary>
        public readonly NotificationContent content;

        /// <summary>
        /// Returns <c>true</c> if the notification arrives when the app is in foreground, <c>false</c> otherwise.
        /// </summary>
        public readonly bool isAppInForeground;

        /// <summary>
        /// Returns <c>true</c> if the notification was posted to the device's notification center and then opened by the user.
        /// This will be <c>false</c> if the notification was not posted to the notification center
        /// (e.g. because the app is in foreground) and its data was sent to your app directly.
        /// </summary>
        public readonly bool isOpened;

        public Notification(string notificationId, string actionId, NotificationContent content, bool isForeground, bool isOpened)
        {
            this.id = notificationId;
            this.actionId = actionId;
            this.content = content;
            this.isAppInForeground = isForeground;
            this.isOpened = isOpened;
        }
    }

    public class LocalNotification : Notification
    {
        public LocalNotification(string notificationId, string actionId, NotificationContent content, bool isForeground, bool isOpened)
            : base(notificationId, actionId, content, isForeground, isOpened)
        {
        }
    }

    public class RemoteNotification : Notification
    {
        /// <summary>
        /// The original payload object if OneSignal is used, <c>null</c> otherwise.
        /// </summary>
        public readonly OneSignalNotificationPayload oneSignalPayload;

        /// <summary>
        /// The original payload object if Firebase is used, <c>null</c> otherwise.
        /// </summary>
        public readonly FirebaseMessage firebasePayload;

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyMobile.RemoteDeliveredNotification"/> class.
        /// This constructor will automatically build the service-specific payload object based on 
        /// the user info dictionary (which was converted from the notification payload JSON string).
        /// This is intended to be used when handling generic (service-independent) remote notification events from native side.
        /// </summary>
        /// <param name="notificationId">Notification ID.</param>
        /// <param name="actionId">Action ID.</param>
        /// <param name="content">Content.</param>
        /// <param name="isForeground">If set to <c>true</c> is foreground.</param>
        /// <param name="isOpened">If set to <c>true</c> is opened.</param>
        public RemoteNotification(string notificationId, string actionId, NotificationContent content, bool isForeground, bool isOpened)
            : base(notificationId, actionId, content, isForeground, isOpened)
        {
            switch (EM_Settings.Notifications.PushNotificationService)
            {
                case PushNotificationProvider.None:
                    break;
                case PushNotificationProvider.OneSignal:
                    // Manually construct OneSignal payload from the JSON payload of the received notification.
                    oneSignalPayload = OneSignalNotificationPayload.FromJSONDict(notificationId, content.userInfo);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyMobile.RemoteDeliveredNotification"/> class.
        /// This is intended to be used when handling remote notification events raised by OneSignal API, 
        /// where <c>OneSignalNotificationPayload</c> can be constructed from the event arguments received.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <param name="payload">Payload.</param>
        /// <param name="isForeground">If set to <c>true</c> is foreground.</param>
        /// <param name="isOpened">If set to <c>true</c> is opened.</param>
        public RemoteNotification(string actionId, OneSignalNotificationPayload payload, bool isForeground, bool isOpened)
            : base(payload.notificationID, actionId, payload.ToNotificationContent(), isForeground, isOpened)
        {
            this.oneSignalPayload = payload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyMobile.RemoteDeliveredNotification"/> class.
        /// This is intended to be used when handling remote notification events raised by Firebase API, 
        /// where <c>FirebaseMessage</c> can be constructed from the event arguments received.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <param name="payload">Payload.</param>
        /// <param name="isForeground">If set to <c>true</c> is foreground.</param>
        /// <param name="isOpened">If set to <c>true</c> is opened.</param>
        public RemoteNotification(string actionId, FirebaseMessage payload, bool isForeground, bool isOpened)
            : base(payload.MessageId, actionId, payload.ToNotificationContent(), isForeground, isOpened)
        {
            this.firebasePayload = payload;
        }
    }
}

