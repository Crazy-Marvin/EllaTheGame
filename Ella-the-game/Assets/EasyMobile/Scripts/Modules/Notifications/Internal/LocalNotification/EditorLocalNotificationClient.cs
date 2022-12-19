#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EasyMobile.Internal.Notifications
{
    internal class EditorLocalNotificationClient : ILocalNotificationClient
    {
        private const string MESSAGE = "Mobile notifications are not available in Unity editor.";

        public void Init(NotificationsSettings settings, INotificationListener listener)
        {
            Debug.LogWarning(MESSAGE);
        }

        public bool IsInitialized()
        {
            return false;
        }

        public void ScheduleLocalNotification(string id, DateTime triggerDate, NotificationContent content)
        {
        }

        public void ScheduleLocalNotification(string id, TimeSpan delay, NotificationContent content)
        {
        }

        public void ScheduleLocalNotification(string id, TimeSpan delay, NotificationContent content, NotificationRepeat repeat)
        {
        }

        public void GetPendingLocalNotifications(Action<NotificationRequest[]> callback)
        {
        }

        public void CancelPendingLocalNotification(string id)
        {
        }

        public void CancelAllPendingLocalNotifications()
        {
        }

        public void RemoveAllDeliveredNotifications()
        {
        }
    }
}
#endif