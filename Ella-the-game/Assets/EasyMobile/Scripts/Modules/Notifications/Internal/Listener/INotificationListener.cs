using UnityEngine;
using System;
using System.Collections;

namespace EasyMobile.Internal.Notifications
{
    internal delegate void NativeNotificationHandler(string data);

    // Interface for internal notification event handlers.
    // If we need to support more push notification services, we'll need to update this.
    internal interface INotificationListener
    {
        event Action<LocalNotification> LocalNotificationOpened;

        event Action<RemoteNotification> RemoteNotificationOpened;

        string Name { get; }

        NativeNotificationHandler NativeNotificationFromForegroundHandler { get; }

        NativeNotificationHandler NativeNotificationFromBackgroundHandler { get; }

        #if EM_ONESIGNAL
        OneSignal.NotificationReceived OnOneSignalNotificationReceived { get; }

        OneSignal.NotificationOpened OnOneSignalNotificationOpened { get; }
        #endif

        #if EM_FIR_MESSAGING
        Action<Firebase.Messaging.MessageReceivedEventArgs> OnFirebaseNotificationReceived { get; }
        #endif
    }
}