using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace EasyMobile.Demo
{
    public class NativeUIDemo : MonoBehaviour
    {
        public GameObject isIOSDarkModeBool;
        public GameObject isFirstButtonBool;
        public GameObject isSecondButtonBool;
        public GameObject isThirdButtonBool;

        public DemoUtils demoUtils;

        void Awake()
        {
            // Init EM runtime if needed (useful in case only this scene is built).
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        void Start()
        {
            UpdateIOSDarkModeBool();
        }

        IEnumerator OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                yield return null;
                UpdateIOSDarkModeBool();
            }
        }

        public void ShowThreeButtonsAlert()
        {
            NativeUI.AlertPopup alert = NativeUI.ShowThreeButtonAlert("Sample Alert", "This is a 3-button alert.", "Button 1", "Button 2", "Button 3");
            if (alert != null)
                alert.OnComplete += OnAlertComplete;
        }

        public void ShowTwoButtonsAlert()
        {
            NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert("Sample Alert", "This is a 2-button alert.", "Button 1", "Button 2");

            if (alert != null)
                alert.OnComplete += OnAlertComplete;
        }

        public void ShowOneButtonAlert()
        {
            NativeUI.AlertPopup alert = NativeUI.Alert("Sample Alert", "This is a simple (1-button) alert.");
            if (alert != null)
                alert.OnComplete += OnAlertComplete;
        }

        public void ShowToast()
        {
#if UNITY_ANDROID
            NativeUI.ShowToast("This is a sample Android toast");
#else
            NativeUI.Alert("Alert", "Toasts are available on Android only.");
#endif
        }

        void OnAlertComplete(int buttonIndex)
        {
            bool isFistButtonClicked = buttonIndex == 0;
            bool isSecondButtonClicked = buttonIndex == 1;
            bool isThirdButtonClicked = buttonIndex == 2;

            if (isFistButtonClicked)
                demoUtils.DisplayBool(isFirstButtonBool, true, "isFirstButtonClicked: TRUE");
            else
                demoUtils.DisplayBool(isFirstButtonBool, false, "isFirstButtonClicked: FALSE");

            if (isSecondButtonClicked)
                demoUtils.DisplayBool(isSecondButtonBool, true, "isSecondButtonClicked: TRUE");
            else
                demoUtils.DisplayBool(isSecondButtonBool, false, "isSecondButtonClicked: FALSE");

            if (isThirdButtonClicked)
                demoUtils.DisplayBool(isThirdButtonBool, true, "isThirdButtonClicked: TRUE");
            else
                demoUtils.DisplayBool(isThirdButtonBool, false, "isThirdButtonClicked: FALSE");
        }

        private void UpdateIOSDarkModeBool()
        {
            if (NativeUI.GetCurrentIOSUserInterfaceStyle() == NativeUI.UserInterfaceStyle.Dark)
                demoUtils.DisplayBool(isIOSDarkModeBool, true, "isIOSDarkMode: TRUE");
            else
                demoUtils.DisplayBool(isIOSDarkModeBool, false, "isIOSDarkMode: FALSE");
        }
    }
}
