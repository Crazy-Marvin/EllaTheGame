using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using EasyMobile.MiniJSON;

namespace EasyMobile.Demo
{
    public class NotificationsDemo : MonoBehaviour
    {
        [Header("One-Time Local Notification")]
        public string title = "Demo Notification";
        public string subtitle = "Demo Notification Subtitle";
        public string message = "Demo notification message";
        public string categoryId;
        public bool fakeNewUpdate = true;
        public int delayHours = 0;
        public int delayMinutes = 0;
        public int delaySeconds = 15;

        [Header("Repeat Local Notification")]
        public string repeatTitle = "Demo Repeat Notification";
        public string repeatSubtitle = "Demo Repeat Notification Subtitle";
        public string repeatMessage = "Demo repeat notification message";
        public string repeatCategoryId;
        public int repeatDelayHours = 0;
        public int repeatDelayMinutes = 0;
        public int repeatDelaySeconds = 25;
        public NotificationRepeat repeatType = NotificationRepeat.EveryMinute;

        [Header("Object References")]
        public GameObject curtain;
        public GameObject pushNotifService;
        public GameObject isAutoInitInfo;
        public GameObject isInitializedInfo;
        public Text pendingNotificationList;
        public InputField idInputField;
        public DemoUtils demoUtils;

        string orgNotificationListText;

        void Awake()
        {
            // Init EM runtime if needed (useful in case only this scene is built).
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        void Start()
        {
            curtain.SetActive(!EM_Settings.IsNotificationsModuleEnable);

            demoUtils.DisplayBool(
                pushNotifService,
                Notifications.CurrentPushNotificationService != PushNotificationProvider.None,
                "Remote Notification Service: " + Notifications.CurrentPushNotificationService.ToString());

            bool autoInit = EM_Settings.Notifications.IsAutoInit;
            demoUtils.DisplayBool(
                isAutoInitInfo,
                autoInit, 
                "Auto Initialization: " + autoInit.ToString().ToUpper()); 

            orgNotificationListText = pendingNotificationList.text;
            InvokeRepeating("UpdatePendingNotificationList", 1, 1);
        }

        void Update()
        {
            bool isInit = Notifications.IsInitialized();
            demoUtils.DisplayBool(
                isInitializedInfo,
                isInit, 
                "IsInitialized: " + isInit.ToString().ToUpper());  
        }

        public void Init()
        {
            if (Notifications.IsInitialized())
            {
                NativeUI.Alert("Already Initialized", "Notification module is already initalized.");
                return;
            }

            Notifications.Init();
        }

        public void ScheduleLocalNotification()
        {
            if (!InitCheck())
                return;

            var notif = new NotificationContent();
            notif.title = title;
            notif.subtitle = subtitle;
            notif.body = message;

            notif.userInfo = new Dictionary<string, object>();
            notif.userInfo.Add("string", "OK");
            notif.userInfo.Add("number", 3);

            if (fakeNewUpdate)
                notif.userInfo.Add("newUpdate", true);

            notif.categoryId = categoryId;

            // Increase badge number (iOS only)
            notif.badge = Notifications.GetAppIconBadgeNumber() + 1;

            DateTime triggerDate = DateTime.Now + new TimeSpan(delayHours, delayMinutes, delaySeconds);
            Notifications.ScheduleLocalNotification(triggerDate, notif);
        }

        public void ScheduleRepeatLocalNotification()
        {
            if (!InitCheck())
                return;

            var notif = new NotificationContent();
            notif.title = repeatTitle;
            notif.body = repeatMessage;
            notif.categoryId = repeatCategoryId;

            Notifications.ScheduleLocalNotification(new TimeSpan(repeatDelayHours, repeatDelayMinutes, repeatDelaySeconds), notif, repeatType);
        }

        public void CancelPendingLocalNotification()
        {
            if (!InitCheck())
                return;

            if (string.IsNullOrEmpty(idInputField.text))
            {
                NativeUI.Alert("Alert", "Please enter the ID of the notification to cancel.");
                return;
            }

            Notifications.CancelPendingLocalNotification(idInputField.text);
            NativeUI.Alert("Alert", "Canceled pending local notification with ID: " + idInputField.text);
        }

        public void CancelAllPendingLocalNotifications()
        {
            if (!InitCheck())
                return;

            Notifications.CancelAllPendingLocalNotifications();
            NativeUI.Alert("Alert", "Canceled all pending local notifications of this app.");
        }

        public void RemoveAllDeliveredNotifications()
        {
            Notifications.ClearAllDeliveredNotifications();
            NativeUI.Alert("Alert", "Cleared all shown notifications of this app.");
        }

        bool InitCheck()
        {
            bool isInit = Notifications.IsInitialized();

            if (!isInit)
            {
                NativeUI.Alert("Alert", "Please initialize first.");
            }

            return isInit;
        }

        void UpdatePendingNotificationList()
        {
            Notifications.GetPendingLocalNotifications(pendingNotifs =>
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var req in pendingNotifs)
                    {
                        NotificationContent content = req.content;

                        sb.Append("ID: " + req.id.ToString() + "\n")
                            .Append("Title: " + content.title + "\n")
                            .Append("Subtitle: " + content.subtitle + "\n")
                            .Append("Body: " + content.body + "\n")
                            .Append("Badge: " + content.badge.ToString() + "\n")
                            .Append("UserInfo: " + Json.Serialize(content.userInfo) + "\n")
                            .Append("CategoryID: " + content.categoryId + "\n")
                            .Append("NextTriggerDate: " + req.nextTriggerDate.ToShortDateString() + "\n")
                            .Append("Repeat: " + req.repeat.ToString() + "\n")
                            .Append("-------------------------\n");
                    }

                    var listText = sb.ToString();

                    // Display list of pending notifications
                    if (!pendingNotificationList.text.Equals(orgNotificationListText) || !string.IsNullOrEmpty(listText))
                        pendingNotificationList.text = sb.ToString();
                });
        }
    }
}