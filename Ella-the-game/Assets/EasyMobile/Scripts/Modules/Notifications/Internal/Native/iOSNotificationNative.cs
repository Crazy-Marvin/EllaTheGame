#if UNITY_IOS
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using EasyMobile.Internal;
using EasyMobile.MiniJSON;

namespace EasyMobile.Internal.Notifications.iOS
{
    #region iOS Native Types
    // ---------------------------------------------------------
    // These types match one-on-one with iOS native types.
    // ---------------------------------------------------------

    internal struct iOSNotificationListenerInfo
    {
        public string name;
        public string backgroundNotificationMethod;
        public string foregroundNotificationMethod;
    }

    internal struct iOSNotificationAuthOptions
    {
        // > 0 means true, otherwise false.
        public int isAlertAllowed;
        public int isBadgeAllowed;
        public int isSoundAllowed;
    }

    internal struct iOSNotificationContent
    {
        public string title;
        public string subtitle;
        public string body;
        public int badge;
        public bool enableSound;
        public string soundName;
        public string userInfoJson;
        public string categoryId;
    }

    internal struct iOSDateComponents
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;
    }

    internal struct iOSNotificationAction
    {
        public string id;
        public string title;
    }

    internal struct iOSNotificationCategory
    {
        public string id;
        public iOSNotificationAction[] actions;
    }

    #endregion // iOS Native Types

    #region iOSNotificationHelper

    internal static class iOSNotificationHelper
    {
        internal const string IOS_USERINFO_ORIGIN_KEY = "origin";
        internal const string IOS_USERINFO_EM_KEY = "easy-mobile";
        internal const string IOS_USERINFO_DATA_KEY = "data";

        internal static string ToIOSNotificationCategoriesJson(iOSNotificationCategory[] iOSCategories)
        {
            if (iOSCategories == null)
                return null;

            var dict = new Dictionary<string, object>();

            foreach (var cat in iOSCategories)
            {
                if (cat.id != null)
                {
                    var actionDict = new Dictionary<string, string>();

                    if (cat.actions != null)
                    {
                        foreach (var act in cat.actions)
                        {
                            if (!string.IsNullOrEmpty(act.id) && !string.IsNullOrEmpty(act.title))
                                actionDict.Add(act.id, act.title);
                        }
                    }

                    dict.Add(cat.id, actionDict);
                }
            }

            return Json.Serialize(dict);
        }

        internal static iOSNotificationCategory[] ToIOSNotificationCategories(NotificationCategory[] categories)
        {
            var iOSCategories = new List<iOSNotificationCategory>();

            if (categories != null)
            {
                foreach (var cat in categories)
                {
                    var iOSActions = new List<iOSNotificationAction>();

                    if (cat.actionButtons != null)
                    {
                        foreach (var btn in cat.actionButtons)
                        {
                            var iOSAct = new iOSNotificationAction()
                            {
                                id = btn.id,
                                title = btn.title
                            };

                            iOSActions.Add(iOSAct);
                        }
                    }

                    var iOSCat = new iOSNotificationCategory()
                    {
                        id = cat.id,
                        actions = iOSActions.ToArray()
                    };

                    iOSCategories.Add(iOSCat);
                }
            }

            return iOSCategories.ToArray();
        }

        internal static iOSNotificationContent ToIOSNotificationContent(NotificationContent content)
        {
            // Encode user info into JSON, attach our own private info for recognization.
            var dict = new Dictionary<string, object>();
            dict.Add(IOS_USERINFO_ORIGIN_KEY, IOS_USERINFO_EM_KEY);
            dict.Add(IOS_USERINFO_DATA_KEY, content.userInfo != null ? content.userInfo : new Dictionary<string, object>());

            var iOSContent = new iOSNotificationContent();
            iOSContent.title = content.title;
            iOSContent.subtitle = content.subtitle;
            iOSContent.body = content.body;
            iOSContent.badge = content.badge;
            iOSContent.userInfoJson = Json.Serialize(dict);

            // Determine the category for this notification.
            // If no valid category specified by the user, the default one is used.
            var category = EM_Settings.Notifications.GetCategoryWithId(content.categoryId);
            if (category == null)
                category = EM_Settings.Notifications.DefaultCategory;

            // Set the category ID.
            iOSContent.categoryId = category.id;

            // Extract sound settings from category.
            // Note that an empty sound name is seen as "using default sound" from native side.
            iOSContent.enableSound = category.sound != NotificationCategory.SoundOptions.Off;
            iOSContent.soundName = category.sound == NotificationCategory.SoundOptions.Custom ? category.soundName : null;

            return iOSContent;
        }

        internal static NotificationContent ToCrossPlatformNotificationContent(iOSNotificationContent iOSContent, out bool isRemote)
        {
            var content = new NotificationContent();
            content.title = iOSContent.title;
            content.subtitle = iOSContent.subtitle;
            content.body = iOSContent.body;
            content.badge = iOSContent.badge;
            content.categoryId = iOSContent.categoryId;

            // Decode user info JSON.
            var userInfo = Json.Deserialize(iOSContent.userInfoJson) as Dictionary<string, object>;
            isRemote = false;

            if (userInfo.ContainsKey(IOS_USERINFO_ORIGIN_KEY) && userInfo[IOS_USERINFO_ORIGIN_KEY].Equals(IOS_USERINFO_EM_KEY))
            {
                // This is a local notification created by us.
                // Extract the actual user info and assign it to the returned content.
                content.userInfo = userInfo[IOS_USERINFO_DATA_KEY] != null ? userInfo[IOS_USERINFO_DATA_KEY] as Dictionary<string, object> : new Dictionary<string, object>();
            }
            else
            {
                // This is probably a remote notification created by APNS.
                // We'll just return the original userInfo.
                isRemote = userInfo.ContainsKey("aps");
                content.userInfo = userInfo;
            }

            return content;
        }

        internal static NotificationRequest ToCrossPlatformNotificationRequest(iOSNotificationRequest iOSRequest, out bool isRemote)
        {
            var req = new NotificationRequest(
                          iOSRequest.Id,
                          iOSNotificationHelper.ToCrossPlatformNotificationContent(iOSRequest.Content, out isRemote),
                          iOSRequest.NextTriggerDate,
                          iOSRequest.Repeat
                      );

            return req;
        }

        internal static iOSNotificationAuthOptions ToIOSNotificationAuthOptions(NotificationAuthOptions opts)
        {
            var iOSAuthOptions = new iOSNotificationAuthOptions();
            iOSAuthOptions.isAlertAllowed = (opts & NotificationAuthOptions.Alert) == NotificationAuthOptions.Alert ? 1 : 0;
            iOSAuthOptions.isBadgeAllowed = (opts & NotificationAuthOptions.Badge) == NotificationAuthOptions.Badge ? 1 : 0;
            iOSAuthOptions.isSoundAllowed = (opts & NotificationAuthOptions.Sound) == NotificationAuthOptions.Sound ? 1 : 0;

            return iOSAuthOptions;
        }

        internal static iOSNotificationListenerInfo ToIOSNotificationListenerInfo(INotificationListener listener)
        {
            var iOSListenerInfo = new iOSNotificationListenerInfo();
            iOSListenerInfo.name = listener.Name;
            iOSListenerInfo.backgroundNotificationMethod = ReflectionUtil.GetMethodName(listener.NativeNotificationFromBackgroundHandler);
            iOSListenerInfo.foregroundNotificationMethod = ReflectionUtil.GetMethodName(listener.NativeNotificationFromForegroundHandler);

            return iOSListenerInfo;
        }

        internal static bool IsEMLocalNotification(Dictionary<string, object> userInfo)
        {
            return userInfo != null && userInfo.ContainsKey(IOS_USERINFO_ORIGIN_KEY) && userInfo[IOS_USERINFO_ORIGIN_KEY].Equals(IOS_USERINFO_EM_KEY);
        }
    }

    #endregion // iOSNotificationHelper

    #region iOSNotificationRequest

    // Wraps native iOS notification request
    internal class iOSNotificationRequest : InteropObject
    {
        private iOSNotificationContent? mContent = null;
        private string mId;
        private string mTitle;
        private string mSubtitle;
        private string mBody;
        private int? mBadge = null;
        private string mUserInfoJson;
        private string mCategoryId;

        internal iOSNotificationRequest(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            iOSNotificationNative.EM_iOSNotificationRequest_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            iOSNotificationNative.EM_iOSNotificationRequest_Unref(selfPointer);
        }

        internal static iOSNotificationRequest FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new iOSNotificationRequest(pointer);
        }

        internal string Id
        {
            get
            {
                if (mId == null)
                    mId = PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                        iOSNotificationNative.EM_iOSNotificationRequest_Id(SelfPtr(), strBuffer, strLen));

                return mId;
            }
        }

        internal iOSNotificationContent Content
        {
            get
            {
                if (mContent == null)
                {
                    var content = new iOSNotificationContent();
                    content.title = this.Title;
                    content.subtitle = this.Subtitle;
                    content.body = this.Body;
                    content.badge = this.Badge;
                    content.userInfoJson = this.UserInfoJson;
                    content.categoryId = this.CategoryId;

                    mContent = new Nullable<iOSNotificationContent>(content);
                }

                return mContent.Value;
            }
        }

        internal DateTime NextTriggerDate
        {
            get
            {
                return Util.FromMillisSinceUnixEpoch(
                    iOSNotificationNative.EM_iOSNotificationRequest_NextTriggerDateMillis(SelfPtr())).ToLocalTime();
            }
        }

        internal NotificationRepeat Repeat
        {
            get
            {
                return (NotificationRepeat)iOSNotificationNative.EM_iOSNotificationRequest_RepeatType(SelfPtr());
            }
        }

        private string Title
        {
            get
            {
                if (mTitle == null)
                    mTitle = PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                        iOSNotificationNative.EM_iOSNotificationRequest_Title(SelfPtr(), strBuffer, strLen));

                return mTitle;
            }
        }

        private string Subtitle
        {
            get
            {
                if (mSubtitle == null)
                    mSubtitle = PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                        iOSNotificationNative.EM_iOSNotificationRequest_Subtitle(SelfPtr(), strBuffer, strLen));

                return mSubtitle;
            }
        }

        private string Body
        {
            get
            {
                if (mBody == null)
                    mBody = PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                        iOSNotificationNative.EM_iOSNotificationRequest_Body(SelfPtr(), strBuffer, strLen));

                return mBody;
            }
        }

        private int Badge
        {
            get
            {
                if (mBadge == null)
                    mBadge = iOSNotificationNative.EM_iOSNotificationRequest_Badge(SelfPtr());

                return mBadge.Value;
            }
        }

        private string UserInfoJson
        {
            get
            {
                if (mUserInfoJson == null)
                    mUserInfoJson = PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                        iOSNotificationNative.EM_iOSNotificationRequest_UserInfoJson(SelfPtr(), strBuffer, strLen));

                return mUserInfoJson;
            }
        }

        private string CategoryId
        {
            get
            {
                if (mCategoryId == null)
                    mCategoryId = PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                        iOSNotificationNative.EM_iOSNotificationRequest_CategoryId(SelfPtr(), strBuffer, strLen));

                return mCategoryId;
            }
        }
    }

    #endregion

    #region iOSNotificationResponse

    // Wraps native iOS notification response.
    internal class iOSNotificationResponse : InteropObject
    {
        internal iOSNotificationResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            iOSNotificationNative.EM_iOSNotificationResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            iOSNotificationNative.EM_iOSNotificationResponse_Unref(selfPointer);
        }

        internal static iOSNotificationResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new iOSNotificationResponse(pointer);
        }

        internal string ActionIdentifier
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                    iOSNotificationNative.EM_iOSNotificationResponse_ActionIdentifier(SelfPtr(), strBuffer, strLen));
            }
        }

        internal DateTime DeliveryDate
        {
            get
            {
                return Util.FromMillisSinceUnixEpoch(
                    iOSNotificationNative.EM_iOSNotificationResponse_DeliveryDateMillis(SelfPtr())
                ).ToLocalTime();
            }
        }

        internal iOSNotificationRequest GetRequest()
        {
            return iOSNotificationRequest.FromPointer(
                iOSNotificationNative.EM_iOSNotificationResponse_Request(SelfPtr())
            );
        }
    }

    #endregion

    #region GetPendingNotificationRequestsResponse

    internal class GetPendingNotificationRequestsResponse : InteropObject
    {
        internal GetPendingNotificationRequestsResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            iOSNotificationNative.EM_GetPendingNotificationRequestsResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            iOSNotificationNative.EM_GetPendingNotificationRequestsResponse_Unref(selfPointer);
        }

        internal static GetPendingNotificationRequestsResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new GetPendingNotificationRequestsResponse(pointer);
        }

        internal iOSNotificationRequest[] GetNotificationRequests()
        {
            return PInvokeUtil.ToEnumerable<iOSNotificationRequest>(
                iOSNotificationNative.EM_GetPendingNotificationRequestsResponse_GetData_Length(SelfPtr()),
                index =>
                iOSNotificationRequest.FromPointer(
                    iOSNotificationNative.EM_GetPendingNotificationRequestsResponse_GetData_GetElement(SelfPtr(), index))
            ).Cast<iOSNotificationRequest>().ToArray();
        }
    }

    #endregion

    #region iOSNotificationNative

    internal static class iOSNotificationNative
    {
        internal delegate void GetPendingNotificationRequestsCallback(IntPtr response, IntPtr callbackPtr);

        internal delegate void GetNotificationResponseCallback(IntPtr response, IntPtr callbackPtr);

        // Local Notification API ==========================================================

        [DllImport("__Internal")]
        internal static extern void EM_InitNotifications(ref iOSNotificationAuthOptions authOptions, ref iOSNotificationListenerInfo listener, string jsonCategories);

        [DllImport("__Internal")]
        internal static extern void EM_ScheduleLocalNotification(string identifier, ref iOSNotificationContent content, long delaySeconds);

        [DllImport("__Internal")]
        internal static extern void EM_ScheduleRepeatLocalNotification(string identifier, ref iOSNotificationContent content, ref iOSDateComponents dateComponents, NotificationRepeat repeat);

        [DllImport("__Internal")]
        internal static extern void EM_GetPendingNotificationRequests(GetPendingNotificationRequestsCallback callback, IntPtr secondaryCallback);

        [DllImport("__Internal")]
        internal static extern void EM_GetNotificationResponse(string identifier, GetNotificationResponseCallback callback, IntPtr secondaryCallback);

        [DllImport("__Internal")]
        internal static extern void EM_RemovePendingNotificationRequestWithId(string identifier);

        [DllImport("__Internal")]
        internal static extern void EM_RemoveAllPendingNotificationRequests();

        [DllImport("__Internal")]
        internal static extern void EM_RemoveDeliveredNotificationWithId(string identifier);

        [DllImport("__Internal")]
        internal static extern void EM_RemoveAllDeliveredNotifications();

        // iOSNotificationRequest ===========================================================

        [DllImport("__Internal")]
        internal static extern void EM_iOSNotificationRequest_Ref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern void EM_iOSNotificationRequest_Unref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_Id(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_Title(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_Subtitle(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_Body(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_Badge(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_UserInfoJson(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_CategoryId(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern long EM_iOSNotificationRequest_NextTriggerDateMillis(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationRequest_RepeatType(HandleRef self);

        // iOSNotificationResponse ===========================================================
        [DllImport("__Internal")]
        internal static extern void EM_iOSNotificationResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern void EM_iOSNotificationResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_iOSNotificationResponse_ActionIdentifier(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern IntPtr EM_iOSNotificationResponse_Request(HandleRef self);

        [DllImport("__Internal")]
        internal static extern long EM_iOSNotificationResponse_DeliveryDateMillis(HandleRef self);

        // GetPendingNotificationRequestsResponse ===========================================

        [DllImport("__Internal")]
        internal static extern void EM_GetPendingNotificationRequestsResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern void EM_GetPendingNotificationRequestsResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_GetPendingNotificationRequestsResponse_GetData_Length(HandleRef self);

        [DllImport("__Internal")]
        internal static extern IntPtr EM_GetPendingNotificationRequestsResponse_GetData_GetElement(HandleRef self, int index);
    }

    #endregion // iOSNotificationNative
}
#endif