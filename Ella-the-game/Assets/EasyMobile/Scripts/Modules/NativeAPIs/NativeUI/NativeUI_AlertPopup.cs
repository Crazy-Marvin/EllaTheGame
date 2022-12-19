using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile
{
    #if UNITY_IOS
    using EasyMobile.Internal.NativeAPIs.iOS;
    #endif

    #if UNITY_ANDROID
    using EasyMobile.Internal.NativeAPIs.Android;
    #endif

    public partial class NativeUI
    {
        // We need a game object to receive and handle messages from native side,
        // therefore this class inherits MonoBehaviour so it can be attached to that game object.
        [AddComponentMenu("")]
        public class AlertPopup : MonoBehaviour
        {
            // The current active alert
            public static AlertPopup Instance { get; private set; }

            /// <summary>
            /// Occurs when the alert is closed. The returned int value can be 0,1 and 2
            /// corresponding to button1, button2 and button3 respectively.
            /// </summary>
            public event Action<int> OnComplete = delegate {};

            #if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            private static readonly string ALERT_GAMEOBJECT = "MobileNativeAlert";
            #endif

            /// <summary>
            /// Shows an alert dialog with 3 buttons.
            /// </summary>
            /// <returns>The three buttons alert.</returns>
            /// <param name="title">Title.</param>
            /// <param name="message">Message.</param>
            /// <param name="button1">Button1.</param>
            /// <param name="button2">Button2.</param>
            /// <param name="button3">Button3.</param>
            internal static AlertPopup ShowThreeButtonAlert(string title, string message, string button1, string button2, string button3)
            {
                #if UNITY_EDITOR
                Debug.Log("Show 3-button alert with message: " + message);
                return null; 
                #elif UNITY_IOS
                if (Instance != null)
                    return null;    // only allow one alert at a time

                // Create a Unity game object to receive messages from native side
                Instance = new GameObject(ALERT_GAMEOBJECT).AddComponent<AlertPopup>();

                // Show iOS 3-button alert
                iOSNativeAlert.ShowThreeButtonsAlert(title, message, button1, button2, button3);

                return Instance;
                #elif UNITY_ANDROID
                if (Instance != null)
                return null;    // only allow one alert at a time

                // Create a Unity game object to receive messages from native side
                Instance = new GameObject(ALERT_GAMEOBJECT).AddComponent<AlertPopup>();

                // Show Android 3-button alert
                AndroidNativeAlert.ShowThreeButtonsAlert(title, message, button1, button2, button3);

                return Instance;
                #else
                Debug.Log("Native alert is not supported on this platform.");
                return null;    
                #endif
            }

            /// <summary>
            /// Shows an alert dialog with 2 buttons.
            /// </summary>
            /// <returns>The two buttons alert.</returns>
            /// <param name="title">Title.</param>
            /// <param name="message">Message.</param>
            /// <param name="button1">Button1.</param>
            /// <param name="button2">Button2.</param>
            internal static AlertPopup ShowTwoButtonAlert(string title, string message, string button1, string button2)
            {
                #if UNITY_EDITOR
                Debug.Log("Show 2-button alert with message: " + message);
                return null; 
                #elif UNITY_IOS
                if (Instance != null)
                    return null;    // only allow one alert at a time

                // Create a Unity game object to receive messages from native side
                Instance = new GameObject(ALERT_GAMEOBJECT).AddComponent<AlertPopup>();

                // Show iOS 2-button alert
                iOSNativeAlert.ShowTwoButtonsAlert(title, message, button1, button2);

                return Instance;
                #elif UNITY_ANDROID
                if (Instance != null)
                return null;    // only allow one alert at a time

                // Create a Unity game object to receive messages from native side
                Instance = new GameObject(ALERT_GAMEOBJECT).AddComponent<AlertPopup>();

                // Show Android 2-button alert
                AndroidNativeAlert.ShowTwoButtonsAlert(title, message, button1, button2);

                return Instance;
                #else
                Debug.Log("Native alert is not supported on this platform.");
                return null;    
                #endif
            }

            /// <summary>
            /// Shows an alert dialog with 1 button.
            /// </summary>
            /// <returns>The one button alert.</returns>
            /// <param name="title">Title.</param>
            /// <param name="message">Message.</param>
            /// <param name="button">Button.</param>
            internal static AlertPopup ShowOneButtonAlert(string title, string message, string button)
            {
                #if UNITY_EDITOR
                Debug.Log("Show alert with message: " + message);
                return null;
                #elif UNITY_IOS
                if (Instance != null)
                    return null;    // only allow one alert at a time

                // Create a Unity game object to receive messages from native side
                Instance = new GameObject(ALERT_GAMEOBJECT).AddComponent<AlertPopup>();

                // Show iOS alert
                iOSNativeAlert.ShowOneButtonAlert(title, message, button);

                return Instance;
                #elif UNITY_ANDROID
                if (Instance != null)
                return null;    // only allow one alert at a time

                // Create a Unity game object to receive messages from native side
                Instance = new GameObject(ALERT_GAMEOBJECT).AddComponent<AlertPopup>();

                // Show Android alert
                AndroidNativeAlert.ShowOneButtonAlert(title, message, button);

                return Instance;
                #else
                Debug.Log("Native alert is not supported on this platform.");
                return null;    
                #endif
            }

            /// <summary>
            /// Shows an alert dialog with the default "OK" button.
            /// </summary>
            /// <returns>The alert.</returns>
            /// <param name="title">Title.</param>
            /// <param name="message">Message.</param>
            internal static AlertPopup Alert(string title, string message)
            {
                return ShowOneButtonAlert(title, message, "OK");
            }

            /// <summary>
            /// Shows a toast message (Android only).
            /// </summary>
            /// <param name="message">Message.</param>
            /// <param name="isLongToast">If set to <c>true</c> use long length toast, otherwise use short length toast.</param>
            internal static void ShowToast(string message, bool isLongToast = false)
            {   
                #if UNITY_ANDROID && !UNITY_EDITOR
                AndroidNativeAlert.ShowToast(message, isLongToast); 
                #else
                Debug.Log("Toasts are only available on Android devices.");
                #endif
            }

            // Alert callback to be called from native side with UnitySendMessage
            private void OnNativeAlertCallback(string buttonIndex)
            {
                // Release the singleton instance so a new alert can be created.
                Instance = null;

                // Fire event
                OnComplete(Convert.ToInt16(buttonIndex));

                // Destroy the used object
                Destroy(gameObject);
            }
        }
    }
}

