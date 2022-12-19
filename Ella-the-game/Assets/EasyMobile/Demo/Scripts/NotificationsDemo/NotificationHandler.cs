using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using EasyMobile.MiniJSON;

namespace EasyMobile.Demo
{
    public class NotificationHandler : MonoBehaviour
    {
        public static NotificationHandler Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            Notifications.PushTokenReceived += OnPushNotificationTokenReceived;
            Notifications.LocalNotificationOpened += OnLocalNotificationOpened;
            Notifications.RemoteNotificationOpened += OnPushNotificationOpened;
        }

        void OnDisable()
        {
            Notifications.PushTokenReceived -= OnPushNotificationTokenReceived;
            Notifications.LocalNotificationOpened -= OnLocalNotificationOpened;
            Notifications.RemoteNotificationOpened -= OnPushNotificationOpened;
        }

        void OnPushNotificationTokenReceived(string token)
        {
            Debug.Log("OnPushNotificationTokenReceived: " + token);
        }

        void OnLocalNotificationOpened(LocalNotification delivered)
        {
            // Reset app icon badge number (iOS only)
            if (delivered.isOpened)
                Notifications.SetAppIconBadgeNumber(0);

            DisplayNotification(delivered, false);
        }

        // Push notification opened handler
        void OnPushNotificationOpened(RemoteNotification delivered)
        {
            DisplayNotification(delivered, true);
        }

        void DisplayNotification(Notification delivered, bool isRemote)
        {
            var content = delivered.content;
            var sb = new StringBuilder();

            bool hasNewUpdate = content.userInfo != null ? content.userInfo.ContainsKey("newUpdate") : false;

            if (hasNewUpdate)
            {
                sb.Append("A new version is available. Do you want to update now?\n");
            }

            sb.AppendLine("----- NOTIFICATION DATA -----")
                .AppendLine("ActionID: " + delivered.actionId ?? "null")
                .AppendLine("isAppInForeground: " + delivered.isAppInForeground)
                .AppendLine("isOpened: " + delivered.isOpened)
                .AppendLine("Title: " + content.title ?? "null")
                .AppendLine("Body: " + content.body ?? "null")
                .AppendLine("Badge: " + content.badge.ToString())
                .AppendLine("CategoryID: " + content.categoryId ?? "null")
                .AppendLine("UserInfo: " + (content.userInfo != null ? Json.Serialize(content.userInfo) : "null"));

            string popupTitle;
            if (isRemote)
                popupTitle = Notifications.CurrentPushNotificationService.ToString() + " Push Notification Received";
            else
                popupTitle = "Local Notification Received";

            StartCoroutine(CRWaitAndShowPopup(hasNewUpdate, popupTitle, sb.ToString()));

            // Print original OneSignal payload for debug purpose.
            if (isRemote)
            {
                if (Notifications.CurrentPushNotificationService == PushNotificationProvider.OneSignal)
                {
                    var oneSignalPayload = ((RemoteNotification)delivered).oneSignalPayload;

                    if (oneSignalPayload == null)
                    {
                        Debug.Log("Something wrong: using OneSignal service but oneSignalPayload was not initialized.");
                    }
                    else
                    {
                        var oneSignalPayloadSb = new StringBuilder()
                            .AppendLine("----- START ONESIGNAL PAYLOAD -----")
                            .AppendLine("notificationID: " + oneSignalPayload.notificationID ?? "null")
                            .AppendLine("sound: " + oneSignalPayload.sound ?? "null")
                            .AppendLine("title: " + oneSignalPayload.title ?? "null")
                            .AppendLine("body: " + oneSignalPayload.body ?? "null")
                            .AppendLine("subtitle: " + oneSignalPayload.subtitle ?? "null")
                            .AppendLine("launchURL: " + oneSignalPayload.launchURL ?? "null")
                            .AppendLine("additionalData: " + oneSignalPayload.additionalData != null ? Json.Serialize(oneSignalPayload.additionalData) : "null")
                            .AppendLine("actionButtons: " + oneSignalPayload.actionButtons != null ? Json.Serialize(oneSignalPayload.actionButtons) : "null")
                            .AppendLine("contentAvailable: " + oneSignalPayload.contentAvailable.ToString())
                            .AppendLine("badge: " + oneSignalPayload.badge)
                            .AppendLine("smallIcon: " + oneSignalPayload.smallIcon ?? "null")
                            .AppendLine("largeIcon: " + oneSignalPayload.largeIcon ?? "null")
                            .AppendLine("bigPicture: " + oneSignalPayload.bigPicture ?? "null")
                            .AppendLine("smallIconAccentColor: " + oneSignalPayload.smallIconAccentColor ?? "null")
                            .AppendLine("ledColor: " + oneSignalPayload.ledColor ?? "null")
                            .AppendLine("lockScreenVisibility: " + oneSignalPayload.lockScreenVisibility)
                            .AppendLine("groupKey: " + oneSignalPayload.groupKey ?? "null")
                            .AppendLine("groupMessage: " + oneSignalPayload.groupMessage ?? "null")
                            .AppendLine("fromProjectNumber: " + oneSignalPayload.fromProjectNumber ?? "null")
                            .AppendLine("----- END ONESIGNAL PAYLOAD -----");

                        Debug.Log(oneSignalPayloadSb.ToString());
                    }
                }
                else if (Notifications.CurrentPushNotificationService == PushNotificationProvider.Firebase)
                {
                    var firebasePayload = ((RemoteNotification)delivered).firebasePayload;

                    if (firebasePayload == null)
                    {
                        Debug.Log("Something wrong: using Firebase service but firebasePayload was not initialized.");
                    }
                    else
                    {
                        var firebasePayloadSb = new StringBuilder()
                            .AppendLine("----- START FIREBASE PAYLOAD -----")
                            .AppendLine("Title: " + firebasePayload.Notification.Title)
                            .AppendLine("Body: " + firebasePayload.Notification.Body)
                            .AppendLine("Icon: " + firebasePayload.Notification.Icon)
                            .AppendLine("Sound: " + firebasePayload.Notification.Sound)
                            .AppendLine("Badge: " + firebasePayload.Notification.Badge)
                            .AppendLine("Tag: " + firebasePayload.Notification.Tag)
                            .AppendLine("ClickAction: " + firebasePayload.Notification.ClickAction)
                            .AppendLine("Data: " + (firebasePayload.Data != null ? Json.Serialize(firebasePayload.Data) : "null"))
                            .AppendLine("NotificationOpened: " + firebasePayload.NotificationOpened)
                            .AppendLine("MessageId: " + firebasePayload.MessageId)
                            .AppendLine("From: " + firebasePayload.From)
                            .AppendLine("To: " + firebasePayload.To)
                            .AppendLine("ErrorDescription: " + firebasePayload.ErrorDescription)
                            .AppendLine("----- END FIREBASE PAYLOAD -----");

                        Debug.Log(firebasePayloadSb.ToString());
                    }
                }
            }
        }

        IEnumerator CRWaitAndShowPopup(bool hasNewUpdate, string title, string message)
        {
            // Wait until no other alert is showing.
            while (NativeUI.IsShowingAlert())
                yield return new WaitForSeconds(0.1f);

            if (!hasNewUpdate)
                NativeUI.Alert(title, message);
            else
            {
                NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert(
                                                title,
                                                message,
                                                "Yes",
                                                "No");

                if (alert != null)
                {
                    alert.OnComplete += (int button) =>
                    {
                        if (button == 0)
                        {
                            NativeUI.Alert(
                                "Open App Store", 
                                "The user has opted to update! In a real app you would want to open the app store for them to download the new version.");
                        }
                    };
                }
            }
        }
    }
}