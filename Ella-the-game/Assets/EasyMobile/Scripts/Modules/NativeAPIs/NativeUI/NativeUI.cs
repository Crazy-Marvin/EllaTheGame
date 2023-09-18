using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    public static partial class NativeUI
    {
        #region Alerts

        /// <summary>
        /// Shows an alert with 3 buttons.
        /// </summary>
        /// <returns>The three buttons alert.</returns>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="button1">Button1.</param>
        /// <param name="button2">Button2.</param>
        /// <param name="button3">Button3.</param>
        public static AlertPopup ShowThreeButtonAlert(string title, string message, string button1, string button2, string button3)
        {
            return AlertPopup.ShowThreeButtonAlert(title, message, button1, button2, button3);
        }

        /// <summary>
        /// Shows an alert with 2 buttons.
        /// </summary>
        /// <returns>The two buttons alert.</returns>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="button1">Button1.</param>
        /// <param name="button2">Button2.</param>
        public static AlertPopup ShowTwoButtonAlert(string title, string message, string button1, string button2)
        {
            return AlertPopup.ShowTwoButtonAlert(title, message, button1, button2);
        }

        /// <summary>
        /// Shows a one-button alert with a custom button label.
        /// </summary>
        /// <returns>The one button alert.</returns>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="button">Button.</param>
        public static AlertPopup Alert(string title, string message, string button)
        {
            return AlertPopup.ShowOneButtonAlert(title, message, button);
        }

        /// <summary>
        /// Shows a one-button alert with the default "OK" button.
        /// </summary>
        /// <returns>The alert.</returns>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        public static AlertPopup Alert(string title, string message)
        {
            return AlertPopup.Alert(title, message);
        }

        /// <summary>
        /// Determines if an alert popup is being shown.
        /// </summary>
        /// <returns><c>true</c> if is showing; otherwise, <c>false</c>.</returns>
        public static bool IsShowingAlert()
        {
            return (AlertPopup.Instance != null);
        }

        #endregion


        #region Android Toasts

#if UNITY_ANDROID
        /// <summary>
        /// Shows a toast message (Android only).
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="isLongToast">If set to <c>true</c> use long-length toast, otherwise use short-length toast.</param>
        public static void ShowToast(string message, bool isLongToast = false)
        {   
            AlertPopup.ShowToast(message, isLongToast);
        }
#endif

        #endregion

        #region UserInterfaceStyle check

        public enum UserInterfaceStyle
        {
            Unspecified = 0,
            Light = 1,
            Dark = 2
        }

        /// <summary>
        /// Gets the current user interface style defined by the <see cref="UserInterfaceStyle"/> enum.
        /// This method currently works on iOS only. On Android it always returns <see cref="UserInterfaceStyle.Unspecified"/>.
        /// </summary>
        /// <returns></returns>
        public static UserInterfaceStyle GetCurrentIOSUserInterfaceStyle()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return (UserInterfaceStyle)Internal.NativeAPIs.iOS.iOSNativeAlert.GetCurrentUserInterfaceStyle();
#else
            return UserInterfaceStyle.Unspecified;
#endif
        }

        #endregion
    }
}