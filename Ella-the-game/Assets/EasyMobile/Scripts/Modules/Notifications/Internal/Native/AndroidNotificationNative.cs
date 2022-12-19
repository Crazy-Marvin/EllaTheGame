#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyMobile.Internal;
using EasyMobile.MiniJSON;

namespace EasyMobile.Internal.Notifications.Android
{
    #region AndroidNotificationHelper
    internal class AndroidNotificationHelper
    {
        internal static string ToJson(NotificationCategoryGroup[] categoryGroups)
        {
            if (categoryGroups == null)
                return Json.Serialize(new string[0]);
            
            // Convert to array of JSON.
            var jsonStrArray = new string[categoryGroups.Length];
            for (int i = 0; i < categoryGroups.Length; i++)
                jsonStrArray[i] = AndroidNotificationCategoryGroup.FromCrossPlatformCategoryGroup(categoryGroups[i]).ToJson();

            // Encode to single JSON string.
            return Json.Serialize(jsonStrArray);
        }

        internal static string ToJson(NotificationCategory[] categories)
        {
            if (categories == null)
                return Json.Serialize(new string[0]);
            
            var jsonStrArray = new string[categories.Length];
            for (int i = 0; i < categories.Length; i++)
                jsonStrArray[i] = AndroidNotificationCategory.FromCrossPlatformCategory(categories[i]).ToJson();

            return Json.Serialize(jsonStrArray);
        }
    }

    #endregion

    #region AndroidNotificationCategoryGroup

    [System.Serializable]
    internal class AndroidNotificationCategoryGroup
    {
        public string id;
        public string name;

        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        internal static AndroidNotificationCategoryGroup FromCrossPlatformCategoryGroup(NotificationCategoryGroup categoryGroup)
        {
            if (categoryGroup == null)
                return null;

            var g = new AndroidNotificationCategoryGroup();
            g.id = categoryGroup.id;
            g.name = categoryGroup.name;

            return g;
        }
    }

    #endregion

    #region AndroidNotificationCategory

    [System.Serializable]
    internal class AndroidNotificationCategory
    {
        [System.Serializable]
        internal class AndroidActionButton
        {
            public string id;
            public string title;
            public string icon = string.Empty;
        }

        public string id;
        public string groupId;
        public string name;
        public string description;
        public int importance;
        public bool enableBadge;
        public int lights;
        public int lightColor;
        public int vibration;
        public int[] vibrationPattern;
        public int lockScreenVisibility;
        public int sound;
        public string soundName;
        public AndroidActionButton[] actionButtons;

        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        internal static AndroidNotificationCategory FromCrossPlatformCategory(NotificationCategory category)
        {
            if (category == null)
                return null;

            var androidCat = new AndroidNotificationCategory();

            androidCat.id = category.id;
            androidCat.groupId = category.groupId;
            androidCat.name = category.name;
            androidCat.description = category.description;
            androidCat.importance = (int)category.importance;
            androidCat.enableBadge = category.enableBadge;
            androidCat.lights = (int)category.lights;
            androidCat.lightColor = category.lightColor.ToARGBHex();
            androidCat.vibration = (int)category.vibration;
            androidCat.vibrationPattern = category.vibrationPattern;
            androidCat.lockScreenVisibility = (int)category.lockScreenVisibility;
            androidCat.sound = (int)category.sound;
            androidCat.soundName = System.IO.Path.GetFileNameWithoutExtension(category.soundName);  // Android doesn't need file extension

            var actionList = new List<AndroidActionButton>();
            if (category.actionButtons != null)
            {
                for (int i = 0; i < category.actionButtons.Length; i++)
                    actionList.Add(new AndroidActionButton()
                        {
                            id = category.actionButtons[i].id,
                            title = category.actionButtons[i].title
                        });
            }
            androidCat.actionButtons = actionList.ToArray();

            return androidCat;
        }
    }

    #endregion

    #region AndroidNotificationResponse

    // Represents an native Android notification response.
    [System.Serializable]
    internal class AndroidNotificationResponse
    {
        // *** IMPORTANT: these member names must match the corresponding keys used when serializing JSON strings from Android native.
        public string actionId;
        public AndroidNotificationRequest request;

        internal AndroidNotificationResponse(string actionId, AndroidNotificationRequest request)
        {
            this.actionId = actionId;
            this.request = request;
        }

        internal static AndroidNotificationResponse FromJson(string jsonData)
        {
            var dict = Json.Deserialize(jsonData) as Dictionary<string, object>;
            string actionId = null;
            AndroidNotificationRequest request = null;

            if (dict.ContainsKey("actionId"))
                actionId = dict["actionId"] as string;

            if (dict.ContainsKey("request"))
                request = AndroidNotificationRequest.FromJson((string)dict["request"]);

            return new AndroidNotificationResponse(actionId, request);
        }
    }

    #endregion

    #region AndroidNotificationRequest

    // Represents an native Android notification request.
    [System.Serializable]
    internal class AndroidNotificationRequest
    {
        // *** IMPORTANT: these member names must match the corresponding keys used when serializing JSON strings from Android native.
        public string id;
        public string title;
        public string message;
        public string userInfo;
        public string categoryId;
        public string smallIcon;
        public string largeIcon;
        public long fireTimeMillis;
        public long repeatSecs;
        public int requestCode;

        internal AndroidNotificationRequest(
            string id,
            string title,
            string message,
            string infoJson,
            string categoryId,
            string smallIcon,
            string largeIcon,
            long fireTimeMillis,
            long repeatSecs,
            int requestCode)
        {
            this.id = id;
            this.title = title;
            this.message = message;
            this.userInfo = infoJson;
            this.categoryId = categoryId;
            this.smallIcon = smallIcon;
            this.largeIcon = largeIcon;
            this.fireTimeMillis = fireTimeMillis;
            this.repeatSecs = repeatSecs;
            this.requestCode = requestCode;
        }

        internal NotificationRequest ToCrossPlatformNotificationRequest()
        {
            var content = new NotificationContent();
            content.title = title;
            content.body = message;
            content.userInfo = string.IsNullOrEmpty(userInfo) ? new Dictionary<string, object>() : Json.Deserialize(userInfo) as Dictionary<string, object>;
            content.categoryId = categoryId;
            content.smallIcon = smallIcon;
            content.largeIcon = largeIcon;
        
            var req = new NotificationRequest(
                          id,
                          content,
                          Util.FromMillisSinceUnixEpoch(fireTimeMillis).ToLocalTime(),
                          NotificationRepeatExtension.FromExactSecondInterval(repeatSecs)
                      );
        
            return req;
        }

        internal static AndroidNotificationRequest FromJson(String jsonData)
        {
            var req = JsonUtility.FromJson<AndroidNotificationRequest>(jsonData);

            if (req == null)
                Debug.Log("Failed to construct AndroidNotificationRequest: invalid JSON data.");

            return req;
        }
    }

    #endregion  // AndroidNotificationRequest

    #region AndroidNotificationNative

    internal static class AndroidNotificationNative
    {
        private const string ANDROID_JAVA_CLASS = "com.sglib.easymobile.androidnative.notification.NotificationUnityInterface";

        internal static void _InitNativeClient(string categoryGroupsJson, string categoriesJson, string listenerName, string backgroundNotificationMethodName, string foregroundNotificationMethodName)
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "_Init",
                categoryGroupsJson,
                categoriesJson,
                listenerName,
                backgroundNotificationMethodName,
                foregroundNotificationMethodName
            );
        }

        internal static void _ScheduleLocalNotification(
            string id, 
            long delaySecs, 
            long repeatSecs, 
            string title, 
            string body, 
            string userInfoJson,
            string categoryId,
            string smallIcon, 
            string largeIcon)
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "_ScheduleLocalNotification",
                id,
                delaySecs,
                repeatSecs,
                title,
                body,
                userInfoJson,
                categoryId,
                smallIcon,
                largeIcon
            );
        }

        internal static void _GetPendingLocalNotifications(Action<AndroidNotificationRequest[]> callback)
        {
            Util.NullArgumentTest(callback);

            var requests = new List<AndroidNotificationRequest>();
            var jObj = AndroidUtil.CallJavaStaticMethod<AndroidJavaObject>(
                           ANDROID_JAVA_CLASS,
                           "_GetPendingNotificationRequestsJson");

            // The JNI array conversion method will crash if the raw object value is 0.
            if (jObj.GetRawObject().ToInt32() != 0)
            {
                var jsonStrs = AndroidJNIHelper.ConvertFromJNIArray<string[]>(jObj.GetRawObject());
                foreach (string s in jsonStrs)
                {
                    var req = AndroidNotificationRequest.FromJson(s);
                    if (req != null)
                        requests.Add(req);
                }
            }

            jObj.Dispose();
            callback(requests.ToArray());
        }

        internal static void _CancelPendingLocalNotificationRequest(string id)
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "_CancelPendingLocalNotificationRequest",
                id
            );
        }

        internal static void _CancelAllPendingLocalNotificationRequests()
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "_CancelAllPendingLocalNotificationRequests"
            );
        }

        internal static void _CancelAllShownNotifications()
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "_CancelAllShownNotifications"
            );
        }
    }

    #endregion // AndroidNotificationNative
}
#endif