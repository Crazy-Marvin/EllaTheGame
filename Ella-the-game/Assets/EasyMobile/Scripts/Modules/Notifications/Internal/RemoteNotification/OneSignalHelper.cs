#if EM_ONESIGNAL
using UnityEngine;
using System.Collections;
using EasyMobile.Internal;

namespace EasyMobile.Internal.Notifications
{
    internal static class OneSignalHelper
    {
        internal static RemoteNotification ToCrossPlatformRemoteNotification(
            string actionId, OSNotification notification)
        {
            return new RemoteNotification(
                actionId,
                ToCrossPlatformOSNotificationPayload(notification.payload),
                notification.isAppInFocus,
                notification.shown);
        }

        internal static RemoteNotification ToCrossPlatformRemoteNotification(OSNotificationOpenedResult result)
        {
            return ToCrossPlatformRemoteNotification(result.action.actionID, result.notification);
        }

        // Construct the crossplatform OneSignalNotificationPayload by copying the OneSignal original payload.
        internal static OneSignalNotificationPayload ToCrossPlatformOSNotificationPayload(OSNotificationPayload payload)
        {
            return ReflectionUtil.CopyObjectData<OSNotificationPayload, OneSignalNotificationPayload>(payload);
        }
    }
}
#endif