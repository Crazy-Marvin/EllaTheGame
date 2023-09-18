using UnityEngine;
using System.Collections;

#if UNITY_IOS
using System.Runtime.InteropServices;

namespace EasyMobile.Internal.NativeAPIs.iOS
{
    internal static class iOSNativeAlert
    {
        [DllImport("__Internal")]
        private static extern void EM_AlertWithThreeButtons(string title, string message, string button1, string button2, string button3);

        [DllImport("__Internal")]
        private static extern void EM_AlertWithTwoButtons(string title, string message, string button1, string button2);

        [DllImport("__Internal")]
        private static extern void EM_Alert(string title, string message, string button);

        [DllImport("__Internal")]
        private static extern int EM_GetCurrentUserInterfaceStyle();

        internal static void ShowThreeButtonsAlert(string title, string message, string button1, string button2, string button3)
        {
            EM_AlertWithThreeButtons(title, message, button1, button2, button3);
        }

        internal static void ShowTwoButtonsAlert(string title, string message, string button1, string button2)
        {
            EM_AlertWithTwoButtons(title, message, button1, button2);
        }

        internal static void ShowOneButtonAlert(string title, string message, string button)
        {
            EM_Alert(title, message, button);
        }

        internal static int GetCurrentUserInterfaceStyle()
        {
            return EM_GetCurrentUserInterfaceStyle();
        }
    }
}
#endif
