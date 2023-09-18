using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EasyMobile.Internal.Notifications
{
    internal interface ILocalNotificationClient
    {
        void Init(NotificationsSettings settings, INotificationListener listener);

        bool IsInitialized();

        void ScheduleLocalNotification(string id, DateTime triggerDate, NotificationContent content);

        void ScheduleLocalNotification(string id, TimeSpan delay, NotificationContent content);

        void ScheduleLocalNotification(string id, TimeSpan delay, NotificationContent content, NotificationRepeat repeat);

        void GetPendingLocalNotifications(Action<NotificationRequest[]> callback);

        void CancelPendingLocalNotification(string id);

        void CancelAllPendingLocalNotifications();

        void RemoveAllDeliveredNotifications();
    }
}
