#if UNITY_IOS
using UnityEngine;
using System;
using System.Collections;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.Notifications.iOS;

namespace EasyMobile.Internal.Notifications
{
    // Dedicated MonoBehaviour for listening to notification events from iOS native.
    internal class iOSNotificationListener : MonoBehaviour, INotificationListener
    {
        private const string IOS_NOTIFICATION_LISTENER_GAMEOBJECT = "EM_iOSNotificationListener";

        // Singleton: we only need one listener object.
        private static iOSNotificationListener sInstance;

        /// <summary>
        /// Creates a gameobject for use with UnitySendMessage from native side.
        /// Must be called from Unity game thread.
        /// </summary>
        internal static iOSNotificationListener GetListener()
        {
            if (sInstance == null)
            {
                var go = new GameObject(IOS_NOTIFICATION_LISTENER_GAMEOBJECT);
                go.hideFlags = HideFlags.HideAndDontSave;
                sInstance = go.AddComponent<iOSNotificationListener>();
                DontDestroyOnLoad(go);
            }
            return sInstance;
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnDisable()
        {
            if (sInstance == this)
            {
                sInstance = null;
            }
        }

        #region INotificationListener Implementation

        public event Action<LocalNotification> LocalNotificationOpened;

        public event Action<RemoteNotification> RemoteNotificationOpened;

        public string Name
        {
            get { return gameObject.name; }
        }

        public NativeNotificationHandler NativeNotificationFromForegroundHandler
        { 
            get { return this._OnNativeNotificationFromForeground; }
        }

        public NativeNotificationHandler NativeNotificationFromBackgroundHandler
        { 
            get { return this._OnNativeNotificationFromBackground; }
        }

        #if EM_ONESIGNAL
        public OneSignal.NotificationReceived OnOneSignalNotificationReceived
        { 
            get { return this.HandleOneSignalNotificationReceived; }
        }

        public OneSignal.NotificationOpened OnOneSignalNotificationOpened
        { 
            get { return this.HandleOneSignalNotificationOpened; }
        }
        #endif

        #if EM_FIR_MESSAGING
        public Action<Firebase.Messaging.MessageReceivedEventArgs> OnFirebaseNotificationReceived
        {
            get { return this.HandleOnFirebaseNotificationReceived; }
        }
        #endif

        #endregion // INotificationListener Implementation

        #region Internal Notification Event Handlers

        //--------------------------------------------------------
        // Native Notification Event Handlers
        //--------------------------------------------------------

        private void _OnNativeNotificationFromForeground(string notifID)
        {
            InternalOnNativeNotificationEvent(true, notifID);
        }

        private void _OnNativeNotificationFromBackground(string notifID)
        {
            InternalOnNativeNotificationEvent(false, notifID);
        }

        //--------------------------------------------------------
        // OneSignal Event Handlers
        //--------------------------------------------------------
        #if EM_ONESIGNAL
        // Called when your app is in focus and a notification is recieved (no action taken by the user).
        private void HandleOneSignalNotificationReceived(OSNotification notification)
        {
            var delivered = OneSignalHelper.ToCrossPlatformRemoteNotification(null, notification);

            // Fire event
            RaiseRemoteNotificationEvent(delivered);
        }


        // Called when a notification is opened by the user.
        private void HandleOneSignalNotificationOpened(OSNotificationOpenedResult result)
        {
            var delivered = OneSignalHelper.ToCrossPlatformRemoteNotification(result);

            // Fire event
            RaiseRemoteNotificationEvent(delivered);

        }
        #endif

        //--------------------------------------------------------
        // FirebaseMessaging Event Handlers
        //--------------------------------------------------------
        #if EM_FIR_MESSAGING
        private void HandleOnFirebaseNotificationReceived(Firebase.Messaging.MessageReceivedEventArgs receivedMessage)
        {
            var delivered = receivedMessage.ToCrossPlatformRemoteNotification();

            if (!iOSNotificationHelper.IsEMLocalNotification(delivered.content.userInfo))
            {
                // Not EM's local notification
                RaiseRemoteNotificationEvent(delivered);
            }
        }
        #endif

        #endregion // Internal Notification Event Handlers

        #region Internal Stuff

        void RaiseRemoteNotificationEvent(RemoteNotification notification)
        {
            if (RemoteNotificationOpened != null)
                RemoteNotificationOpened(notification);
        }

        IEnumerator CRRaiseLocalNotificationEvent(string actionId, NotificationRequest request, bool isForeground)
        {
            // This could be called at app-launch-from-notification, so we'd better
            // check if the RuntimeHelper is ready before asking it to schedule a job on main thread.
            while (!RuntimeHelper.IsInitialized())
                yield return new WaitForSeconds(0.1f);
            
            RuntimeHelper.RunOnMainThread(() =>
                {
                    var delivered = new LocalNotification(
                                        request.id,
                                        actionId,
                                        request.content,
                                        isForeground,
                                        isForeground ? false : true // isOpened
                                    );

                    if (LocalNotificationOpened != null)
                        LocalNotificationOpened(delivered);
                });
        }

        void InternalOnNativeNotificationEvent(bool isForeground, string notifId)
        {
            InternalGetNotificationResponse(
                notifId,
                response =>
                {
                    if (response == null)
                    {
                        Debug.Log("Ignoring iOS notification due to invalid response data.");
                        return;
                    }

                    var actionID = response.ActionIdentifier;
                    var iOSRequest = response.GetRequest();

                    if (iOSRequest == null)
                    {
                        Debug.Log("Ignoring iOS notification due to NULL request data.");
                        return;
                    }

                    bool isRemote;
                    var request = iOSNotificationHelper.ToCrossPlatformNotificationRequest(iOSRequest, out isRemote);

                    // Only handle local notification events.
                    if (!isRemote)
                        StartCoroutine(CRRaiseLocalNotificationEvent(actionID, request, isForeground));
                });
        }

        private static void InternalGetNotificationResponse(string identifier, Action<iOSNotificationResponse> callback)
        {
            Util.NullArgumentTest(callback);

            iOSNotificationNative.EM_GetNotificationResponse(
                identifier,
                InternalGetNotificationResponseCallback,
                PInvokeCallbackUtil.ToIntPtr<iOSNotificationResponse>(
                    response =>
                    {
                        callback(response);
                    },
                    iOSNotificationResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSNotificationNative.GetNotificationResponseCallback))]
        private static void InternalGetNotificationResponseCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSNotificationListener#InternalGetNotificationResponseCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        #endregion // Internal Stuff
    }
}

#endif