#if EM_FIR_MESSAGING
using Firebase.Messaging;
using UnityEngine;

namespace EasyMobile.Internal.Notifications
{
    internal static class FirebaseMessagingHelper
    {
        internal static RemoteNotification ToCrossPlatformRemoteNotification(this MessageReceivedEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                Debug.LogError("ToCrossPlatformRemoteNotification, EventArgs: null");
                return null;
            }

            FirebaseMessage firebasePayload = eventArgs.Message.ToEasyMobileFirebaseMessage();
            string actionID = firebasePayload.Notification != null ? firebasePayload.Notification.ClickAction : null;

            return new RemoteNotification(
                actionID,
                firebasePayload,
                !firebasePayload.NotificationOpened,
                firebasePayload.NotificationOpened
            );
        }
    }
}
#endif
