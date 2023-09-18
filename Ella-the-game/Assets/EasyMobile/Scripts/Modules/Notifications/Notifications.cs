using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.Internal;
using EasyMobile.Internal.Notifications;

namespace EasyMobile
{
    [AddComponentMenu("")]
    public class Notifications : MonoBehaviour
    {
        // Singleton pattern.
        public static Notifications Instance { get; private set; }

        // Fill this if we need a common prefix for local notification IDs.
        private const string LOCAL_NOTIF_ID_PREFIX = "";
        private const string LOCAL_NOTIF_CURRENT_ID_PPKEY = "EM_LOCAL_NOTIF_CURRENT_ID";

        /// <summary>
        /// Gets the current push notification service.
        /// </summary>
        /// <value>The current push notification service.</value>
        public static PushNotificationProvider CurrentPushNotificationService
        {
            get { return EM_Settings.Notifications.PushNotificationService; }
        }

        #pragma warning disable 0067
        /// <summary>
        /// Occurs when a token is received once the selected remote notification service
        /// has been initialized.
        /// </summary>
        public static event Action<string> PushTokenReceived;
        #pragma warning restore 0067

        /// <summary>
        /// Occurs when a remote notification is opened, either by the default open action,
        /// or by a custom notification action button.
        /// Note that if the notification arrives when the app is in foreground it won't be
        /// posted to the notification center. Instead, this event will be raised immediately
        /// as if the notification was opened by the user.
        /// </summary>
        public static event Action<RemoteNotification> RemoteNotificationOpened;

        /// <summary>
        /// Occurs when a local notification is opened, either by the default open action,
        /// or by a custom notification action button.
        /// Note that if the notification arrives when the app is in foreground it won't be
        /// posted to the notification center. Instead, this event will be raised immediately
        /// as if the notification was opened by the user.
        /// </summary>
        public static event Action<LocalNotification> LocalNotificationOpened;

        /// <summary>
        /// If a remote notification service is used, this contains the registration
        /// token for your app once it is received from the server.
        /// If no remote notification service was selected, or if it hasn't been initialized,
        /// this will be null.
        /// </summary>
        /// <value>The token.</value>
        public static string PushToken { get; private set; }

        private static ILocalNotificationClient LocalNotificationClient
        {
            get
            {
                if (sLocalNotificationClient == null)
                {
                    sLocalNotificationClient = GetLocalNotificationClient();
                }
                return sLocalNotificationClient;
            }
        }

        // Platform-dependent local notification client.
        private static ILocalNotificationClient sLocalNotificationClient;

        // Platform-dependent notification event listeners.
        private static INotificationListener sListener;

        // Whether initialization has been done.
        private static bool sIsInitialized = false;

        #region Public API

        /// <summary>
        /// Initializes local and remote notification services.
        /// </summary>
        public static void Init()
        {
            if (sIsInitialized)
            {
                Debug.Log("Notifications module has been initialized. Ignoring this call.");
                return;
            }

            // Get the listener.
            sListener = GetNotificationListener();

            // Subscibe to internal notification events.
            if (sListener != null)
            {
                sListener.LocalNotificationOpened += InternalOnLocalNotificationOpened;
                sListener.RemoteNotificationOpened += InternalOnRemoteNotificationOpened;
            }

            // Initialize OneSignal push notification service if it's used.
            if (EM_Settings.Notifications.PushNotificationService == PushNotificationProvider.OneSignal)
            {
#if EM_ONESIGNAL
                // Get the applicable data privacy consent for push notifications.
                var consent = GetApplicableDataPrivacyConsent();

                // Init OneSignal.
                var osBuilder = OneSignal.StartInit(EM_Settings.Notifications.OneSignalAppId);

                osBuilder.HandleNotificationReceived(sListener.OnOneSignalNotificationReceived)
                .HandleNotificationOpened(sListener.OnOneSignalNotificationOpened)
                .InFocusDisplaying(OneSignal.OSInFocusDisplayOption.None);

                // Set as requiring consent if one is specified.
                if (consent != ConsentStatus.Unknown)
                    osBuilder.SetRequiresUserPrivacyConsent(true);

                // Finalize init.
                osBuilder.EndInit();

                // Apply the provided consent. If the consent is Unknown it is ignored and the initialization
                // will proceed as normal. If the consent is Granted, the initialization will also complete.
                // If the consent is Revoked the initialization never completes and the device won't be registered
                // with OneSignal and won't receive notifications.
                // Note that if the device has been registered to OneSignal (during previous initialization) 
                // and then the consent is revoked, the notifications will still be delivered to the device
                // but won't be forwarded to the app, because the initialization is now not completed.
                if (consent != ConsentStatus.Unknown)
                    OneSignal.UserDidProvideConsent(consent == ConsentStatus.Granted);

                // Handle when the OneSignal token becomes available.
                // Must be called after StartInit.
                // If the consent is revoked we won't register this to avoid warning from OneSignal.
                if (consent != ConsentStatus.Revoked)
                {
                    OneSignal.IdsAvailable((playerId, pushToken) =>
                        {
                            PushToken = pushToken;

                            if (PushTokenReceived != null)
                                PushTokenReceived(pushToken);
                        });
                }
#else
                Debug.LogError("SDK missing. Please import OneSignal plugin for Unity.");
#endif
            }

            // Initialize Firebase push notification service if it's used.
            if (EM_Settings.Notifications.PushNotificationService == PushNotificationProvider.Firebase)
            {
#if EM_FIR_MESSAGING
                // To offer users a chance to opt-in to FCM, the dev must manually disable FCM's auto-initialization
                // https://firebase.google.com/docs/cloud-messaging/unity/client.
                // If the consent is granted, we'll help re-enable FCM automatically.
                var consent = GetApplicableDataPrivacyConsent();
                if (consent == ConsentStatus.Granted)
                    Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;

                // FirebaseMessaging will initialize once we subscribe to 
                // the TokenReceived or MessageReceived event.
                Firebase.Messaging.FirebaseMessaging.TokenReceived += (_, tokenArg) =>
                {
                    // Cache the registration token and fire event.
                    PushToken = tokenArg.Token;

                    if (PushTokenReceived != null)
                        PushTokenReceived(tokenArg.Token);

                    // Subscribe default Firebase topics if any.
                    // This must be done after the token has been received.
                    if (EM_Settings.Notifications.FirebaseTopics != null)
                    {
                        foreach (string topic in EM_Settings.Notifications.FirebaseTopics)
                        {
                            if (!string.IsNullOrEmpty(topic))
                                Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic);
                        }
                    }
                };

                // Register the event handler to be invoked once a Firebase message is received.
                Firebase.Messaging.FirebaseMessaging.MessageReceived += 
                    (_, param) => sListener.OnFirebaseNotificationReceived(param);
#else
                Debug.LogError("SDK missing. Please import FirebaseMessaging plugin for Unity.");
#endif
            }

            // Initialize local notification client.
            LocalNotificationClient.Init(EM_Settings.Notifications, sListener);

            // Done initializing.
            sIsInitialized = true;
        }

        /// <summary>
        /// Determines if the module has been initialized.
        /// </summary>
        /// <returns><c>true</c> if is initialized; otherwise, <c>false</c>.</returns>
        public static bool IsInitialized()
        {
            return sIsInitialized;
        }

        /// <summary>
        /// Schedules a local notification to be posted at the specified time with no repeat.
        /// Note that the scheduled notification persists even if the device reboots, and it
        /// will be fired immediately after the reboot if the triggerDate has passed.
        /// </summary>
        /// <returns>The ID of the scheduled notification.</returns>
        /// <param name="triggerDate">Trigger date.</param>
        /// <param name="content">Notification content.</param>
        public static string ScheduleLocalNotification(DateTime triggerDate, NotificationContent content)
        {
            var id = NextLocalNotificationId();
            LocalNotificationClient.ScheduleLocalNotification(id, triggerDate, content);
            return id;
        }

        /// <summary>
        /// Schedules a local notification to be posted after the specified delay time with no repeat.
        /// Note that the scheduled notification persists even if the device reboots, and it
        /// will be fired immediately after the reboot if the delay time has passed.
        /// </summary>
        /// <returns>The ID of the scheduled notification.</returns>
        /// <param name="delay">Delay.</param>
        /// <param name="content">Notification content.</param>
        public static string ScheduleLocalNotification(TimeSpan delay, NotificationContent content)
        {
            var id = NextLocalNotificationId();
            LocalNotificationClient.ScheduleLocalNotification(id, delay, content);
            return id;
        }

        /// <summary>
        /// Schedules a local notification to be posted after the specified delay time,
        /// and repeat automatically after the interval specified by the repeat mode.
        /// Note that the scheduled notification persists even if the device reboots, and it
        /// will be fired immediately after the reboot if its latest scheduled fire time has passed.
        /// </summary>
        /// <returns>The ID of the scheduled notification.</returns>
        /// <param name="delay">Delay.</param>
        /// <param name="content">Notification content.</param>
        /// <param name="repeat">Repeat.</param>
        public static string ScheduleLocalNotification(TimeSpan delay, NotificationContent content, NotificationRepeat repeat)
        {
            var id = NextLocalNotificationId();
            LocalNotificationClient.ScheduleLocalNotification(id, delay, content, repeat);
            return id;
        }

        /// <summary>
        /// Gets all scheduled local notifications that haven't been posted.
        /// </summary>
        /// <param name="callback">The callback that is invoked when this operation finishes.
        /// This callback receives an array of all pending notification requests.
        /// If there's no pending notifications, an empty array will be returned.
        /// This callback will always execute on the main thread (game thread)./></param>
        public static void GetPendingLocalNotifications(Action<NotificationRequest[]> callback)
        {
            LocalNotificationClient.GetPendingLocalNotifications(callback);
        }

        /// <summary>
        /// Cancels the pending local notification with the specified ID.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public static void CancelPendingLocalNotification(string id)
        {
            LocalNotificationClient.CancelPendingLocalNotification(id);
        }

        /// <summary>
        /// Cancels all pending local notifications.
        /// </summary>
        public static void CancelAllPendingLocalNotifications()
        {
            LocalNotificationClient.CancelAllPendingLocalNotifications();
        }

        /// <summary>
        /// Removes all previously shown notifications of this app from the notification center or status bar.
        /// </summary>
        public static void ClearAllDeliveredNotifications()
        {
            LocalNotificationClient.RemoveAllDeliveredNotifications();
        }

        /// <summary>
        /// Gets the app icon badge number. This methods is only effective on iOS.
        /// On other platforms it always returns 0.
        /// </summary>
        /// <returns>The app icon badge number.</returns>
        public static int GetAppIconBadgeNumber()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return iOS.UIKit.UIApplication.SharedApplication.GetApplicationIconBadgeNumber();
#else
            return 0;
#endif
        }

        /// <summary>
        /// Sets the app icon badge number. This methods is only effective on iOS.
        /// On other platforms it is a no-op.
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetAppIconBadgeNumber(int value)
        {
#if UNITY_IOS && !UNITY_EDITOR
            iOS.UIKit.UIApplication.SharedApplication.SetApplicationIconBadgeNumber(value);
#endif
        }

        #endregion

        #region Consent Management API

        /// <summary>
        /// Raised when the module-level data privacy consent is changed.
        /// </summary>
        public static event Action<ConsentStatus> DataPrivacyConsentUpdated
        {
            add { NotificationsConsentManager.Instance.DataPrivacyConsentUpdated += value; }
            remove { NotificationsConsentManager.Instance.DataPrivacyConsentUpdated -= value; }
        }

        /// <summary>
        /// The module-level data privacy consent status, 
        /// default to ConsentStatus.Unknown. 
        /// </summary>
        public static ConsentStatus DataPrivacyConsent
        {
            get
            {
                return NotificationsConsentManager.Instance.DataPrivacyConsent;
            }
        }

        /// <summary>
        /// Grants module-level data privacy consent.
        /// This consent persists across app launches.
        /// </summary>
        /// <remarks>
        /// This method is a wrapper of <c>NotificationsConsentManager.Instance.GrantDataPrivacyConsent</c>.
        /// </remarks>
        public static void GrantDataPrivacyConsent()
        {
            NotificationsConsentManager.Instance.GrantDataPrivacyConsent();
        }

        /// <summary>
        /// Revokes the module-level data privacy consent.
        /// This consent persists across app launches.
        /// </summary>
        /// <remarks>
        /// This method is a wrapper of <c>NotificationsConsentManager.Instance.RevokeDataPrivacyConsent</c>.
        /// </remarks>
        public static void RevokeDataPrivacyConsent()
        {
            NotificationsConsentManager.Instance.RevokeDataPrivacyConsent();
        }

        #endregion

        #region Internal stuff

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (EM_Settings.Notifications.IsAutoInit)
            {
                StartCoroutine(CRAutoInit(EM_Settings.Notifications.AutoInitDelay));
            }
        }

        IEnumerator CRAutoInit(float delay)
        {
            yield return new WaitForSeconds(delay);
            Init();
        }

        /// <summary>
        /// Finds the applicable data privacy consent for this module by searching
        /// upward from module-level consent to global-level consent (high priority -> low priority).
        /// </summary>
        /// <returns>The applicable data privacy consent.</returns>
        static ConsentStatus GetApplicableDataPrivacyConsent()
        {
            ConsentStatus localConsent = NotificationsConsentManager.Instance.DataPrivacyConsent;
            ConsentStatus globalConsent = GlobalConsentManager.Instance.DataPrivacyConsent;

            return localConsent != ConsentStatus.Unknown ? localConsent : globalConsent;
        }

        static INotificationListener GetNotificationListener()
        {
#if UNITY_EDITOR
            return EditorNotificationListener.GetListener();
#elif UNITY_IOS
            return iOSNotificationListener.GetListener();
#elif UNITY_ANDROID
            return AndroidNotificationListener.GetListener();
#else
            return UnsupportedNotificationListener.GetListener();
#endif
        }

        static ILocalNotificationClient GetLocalNotificationClient()
        {
#if UNITY_EDITOR
            return new EditorLocalNotificationClient();
#elif UNITY_IOS
            return new iOSLocalNotificationClient();
#elif UNITY_ANDROID
            return new AndroidLocalNotificationClient();
#else
            return new UnsupportedLocalNotificationClient();
#endif
        }

        static string NextLocalNotificationId()
        {
            int currentId = StorageUtil.GetInt(LOCAL_NOTIF_CURRENT_ID_PPKEY, 0);
            int nextId = currentId == int.MaxValue ? 1 : currentId + 1;
            StorageUtil.SetInt(LOCAL_NOTIF_CURRENT_ID_PPKEY, nextId);
            StorageUtil.Save();
            return LOCAL_NOTIF_ID_PREFIX + nextId.ToString();
        }

        static void InternalOnLocalNotificationOpened(LocalNotification delivered)
        {
            if (LocalNotificationOpened != null)
                LocalNotificationOpened(delivered);
        }

        static void InternalOnRemoteNotificationOpened(RemoteNotification delivered)
        {
            if (RemoteNotificationOpened != null)
                RemoteNotificationOpened(delivered);
        }

        #endregion
    }
}