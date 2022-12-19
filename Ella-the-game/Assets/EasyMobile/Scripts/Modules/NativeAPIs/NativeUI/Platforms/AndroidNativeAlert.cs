#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using EasyMobile.Internal;

namespace EasyMobile.Internal.NativeAPIs.Android
{
    internal static class AndroidNativeAlert
    {
        private static readonly string ANDROID_JAVA_UI_CLASS = "com.sglib.easymobile.androidnative.EMNativeUI";

        internal static void ShowThreeButtonsAlert(string title, string message, string button1, string button2, string button3)
        {
            AndroidUtil.CallJavaStaticMethod(ANDROID_JAVA_UI_CLASS, "ShowThreeButtonsAlert", title, message, button1, button2, button3);
        }

        internal static void ShowTwoButtonsAlert(string title, string message, string button1, string button2)
        {
            AndroidUtil.CallJavaStaticMethod(ANDROID_JAVA_UI_CLASS, "ShowTwoButtonsAlert", title, message, button1, button2);
        }

        internal static void ShowOneButtonAlert(string title, string message, string button)
        {
            AndroidUtil.CallJavaStaticMethod(ANDROID_JAVA_UI_CLASS, "ShowOneButtonAlert", title, message, button);
        }

        internal static void ShowToast(string message, bool longToast = false)
        {
            AndroidUtil.CallJavaStaticMethod(ANDROID_JAVA_UI_CLASS, "ShowToast", message, longToast);
        }
    }
}
#endif

