#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.MiniJSON;
using EasyMobile.Internal;
using EasyMobile.Internal.Notifications.Android;

namespace EasyMobile.Internal.Notifications
{
    internal class AndroidLocalNotificationClient : ILocalNotificationClient
    {
        private bool mIsInitialized;
        private NotificationsSettings mSettings;

        #region ILocalNotificationClient Implementation

        public void Init(NotificationsSettings settings, INotificationListener listener)
        {
            // Store the settings for later references.
            mSettings = settings;

            // Convert category groups to JSON. Invalid groups (empty name or ID) will be
            // automatically ignored in native side.
            var categoryGroupJson = AndroidNotificationHelper.ToJson(settings.CategoryGroups);

            // Convert categories to JSON.
            var categories = new List<NotificationCategory>();
            categories.Add(settings.DefaultCategory);

            if (settings.UserCategories != null)
                categories.AddRange(settings.UserCategories);

            var categoriesJson = AndroidNotificationHelper.ToJson(categories.ToArray());

            // Listener info
            var name = listener.Name;
            var backgroundNotificationMethod = ReflectionUtil.GetMethodName(listener.NativeNotificationFromBackgroundHandler);
            var foregroundNotificationMethod = ReflectionUtil.GetMethodName(listener.NativeNotificationFromForegroundHandler);

            // Initialize native Android client, which may send some launch notification data during the process.
            AndroidNotificationNative._InitNativeClient(categoryGroupJson, categoriesJson, name, backgroundNotificationMethod, foregroundNotificationMethod);

            mIsInitialized = true;
        }

        public bool IsInitialized()
        {
            return mIsInitialized;
        }

        public void ScheduleLocalNotification(string id, DateTime fireDate, NotificationContent content)
        {
            fireDate = fireDate.ToLocalTime();
            TimeSpan delay = fireDate <= DateTime.Now ? TimeSpan.Zero : fireDate - DateTime.Now;
            ScheduleLocalNotification(id, delay, content);
        }

        public void ScheduleLocalNotification(string id, TimeSpan delay, NotificationContent content)
        {
            ScheduleLocalNotification(id, delay, content, NotificationRepeat.None);
        }

        public void ScheduleLocalNotification(string id, TimeSpan delay, NotificationContent content, NotificationRepeat repeat)
        {
            if (!mIsInitialized)
            {
                Debug.Log("Please initialize first.");
                return;
            }

            AndroidNotificationNative._ScheduleLocalNotification(
                id,
                (long)delay.TotalSeconds,
                repeat == NotificationRepeat.None ? -1 : (long)repeat.ToSecondInterval(),
                content.title,
                content.body,
                content.userInfo != null ? Json.Serialize(content.userInfo) : "",
                string.IsNullOrEmpty(content.categoryId) ? mSettings.DefaultCategory.id : content.categoryId,    // use Default category if none is specified
                content.smallIcon,
                content.largeIcon
            );
        }

        public void GetPendingLocalNotifications(Action<NotificationRequest[]> callback)
        {
            Util.NullArgumentTest(callback);
            callback = RuntimeHelper.ToMainThread<NotificationRequest[]>(callback);

            AndroidNotificationNative._GetPendingLocalNotifications(
                androidRequests =>
                {
                    var requests = new NotificationRequest[androidRequests.Length];
                    for (int i = 0; i < androidRequests.Length; i++)
                        requests[i] = androidRequests[i].ToCrossPlatformNotificationRequest();

                    callback(requests);
                }
            );
        }

        public void CancelPendingLocalNotification(string id)
        {
            AndroidNotificationNative._CancelPendingLocalNotificationRequest(id);
        }

        public void CancelAllPendingLocalNotifications()
        {
            AndroidNotificationNative._CancelAllPendingLocalNotificationRequests();
        }

        public void RemoveAllDeliveredNotifications()
        {
            AndroidNotificationNative._CancelAllShownNotifications();
        }

        #endregion // ILocalNotificationClient Implementation
    }
}
#endif
