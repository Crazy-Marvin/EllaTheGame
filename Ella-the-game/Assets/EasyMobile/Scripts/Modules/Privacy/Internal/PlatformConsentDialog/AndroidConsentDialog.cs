#if UNITY_ANDROID
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.Privacy
{
    internal class AndroidConsentDialog : IPlatformConsentDialog
    {

        #region Inner Structures

        /// <summary>
        /// Used to deserialize parameter of consent dialog's closed event.
        /// </summary>
        [Serializable]
        internal struct ClosedEventParam
        {
            #pragma warning disable 0649 // Disable unused warning.
            public string clickedButtonId;
            public ToggleResult[] toggles;
            #pragma warning restore

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }

            public Dictionary<string, bool> GetTogglesAsDictionary()
            {
                if (toggles == null || toggles.Length < 1)
                    return null;

                return toggles.ToDictionary(toggle => toggle.id, toggle => toggle.state);
            }
        }

        [Serializable]
        internal struct ToggleResult
        {
            #pragma warning disable 0649 // Disable unused warning.
            public string id;
            public bool state;
            #pragma warning restore
        }

        #endregion // Inner Structures

        #region Singleton

        internal static AndroidConsentDialog Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new AndroidConsentDialog();

                return sInstance;
            }
        }

        private static AndroidConsentDialog sInstance;

        #endregion // Singleton

        /// <summary>
        /// Name of the class contains all Android's native APIs.
        /// </summary>
        internal static string FacadeClassName = "com.sglib.easymobile.androidnative.gdpr.ConsentDialogUnityFacade";

        /// <summary>
        /// Name of the method in the <see cref="FacadeClassName"/> class used to display consent dialog.
        /// </summary>
        internal static string ShowDialogMethodName = "Show";

        /// <summary>
        /// Name of the method in the <see cref="FacadeClassName"/> class used to update interactive state of an button.
        /// </summary>
        internal static string SetButtonInteractableMethodName = "SetButtonInteractable";

        /// <summary>
        /// Name of the method in the <see cref="FacadeClassName"/> class used to update the isOn property of a toggle.
        /// </summary>
        internal static string SetToggleIsOnMethodName = "SetToggleIsOn";

        /// <summary>
        /// Name of the method in the <see cref="FacadeClassName"/> class used to update interactive state of a toggle.
        /// </summary>
        internal static string SetToggleInteractableMethodName = "SetToggleInteractable";

        private event Action<IPlatformConsentDialog, string, Dictionary<string, bool>> mCompleted;
        private event Action<IPlatformConsentDialog> mDismissed;

        private AndroidJavaObject mAndroidJavaObject;
        private NativeConsentDialogListener mListener;
        private bool mIsShowing;

        private AndroidConsentDialog()
        {
            mAndroidJavaObject = new AndroidJavaObject(FacadeClassName);
            mIsShowing = false;

            mListener = NativeConsentDialogListener.GetListener();
            mListener.ToggleStateUpdated += OnNativeToggleStateUpdated;
            mListener.DialogCompleted += OnNativeDialogCompleted;
            mListener.DialogDismissed += OnNativeDialogDismissed;

            mCompleted += (_, __, ___) => mIsShowing = false;
            mDismissed += _ => mIsShowing = false;
        }

        #region Events Callback

        private void OnNativeToggleStateUpdated(string toggleId, bool isOn)
        {
            if (ToggleStateUpdated != null)
                ToggleStateUpdated(this, toggleId, isOn);
        }

        private void OnNativeDialogCompleted(string jsonResult)
        {
            if (mCompleted == null)
                return;

            try
            {
                ClosedEventParam result = JsonUtility.FromJson<ClosedEventParam>(jsonResult);
                mCompleted(this, result.clickedButtonId, result.GetTogglesAsDictionary());
            }
            catch (Exception e)
            {
                Debug.Log("[AndroidConsentDialog -> OnNativeDialogCompleted]. Error: " + e.Message);
            }
        }

        private void OnNativeDialogDismissed()
        {
            if (mDismissed != null)
                mDismissed(this);
        }

        #endregion // Events Callback

        #region IPlatformConsentDialog Implementation

        public event Action<IPlatformConsentDialog, string, bool> ToggleStateUpdated;

        public event Action<IPlatformConsentDialog, string, Dictionary<string, bool>> Completed
        {
            add { mCompleted += value; }
            remove { mCompleted -= value; }
        }

        public event Action<IPlatformConsentDialog> Dismissed
        {
            add { mDismissed += value; }
            remove { mDismissed -= value; }
        }

        public void Show(string title, string content, bool isDismissible)
        {
            try
            {
                mIsShowing = true;
                mAndroidJavaObject.Call(ShowDialogMethodName, title, content, isDismissible);
            }
            catch (Exception e)
            {
                Debug.Log("Error when showing consent dialog on Android. Message: " + e.Message);
                mIsShowing = false;
            }
        }

        public void SetButtonInteractable(string id, bool interactable)
        {
            mAndroidJavaObject.Call(SetButtonInteractableMethodName, id, interactable);
        }

        public void SetToggleInteractable(string toggleId, bool interactable)
        {
            mAndroidJavaObject.Call(SetToggleInteractableMethodName, toggleId, interactable);
        }

        public void SetToggleIsOn(string toggleId, bool isOn, bool animated)
        {
            mAndroidJavaObject.Call(SetToggleIsOnMethodName, toggleId, isOn, animated);
        }

        public bool IsShowing()
        {
            return mIsShowing;
        }

        #endregion // IPlatformConsentDialog Implementation

    }
}
#endif
