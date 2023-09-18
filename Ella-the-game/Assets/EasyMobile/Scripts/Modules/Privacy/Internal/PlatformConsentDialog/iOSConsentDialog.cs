#if UNITY_IOS
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyMobile.MiniJSON;
using System.Linq;

namespace EasyMobile.Internal.Privacy
{
    internal class iOSConsentDialog : IPlatformConsentDialog
    {
        private const string JSON_RESULTS_BUTTON_ID_KEY = "button_id";
        private const string JSON_RESULTS_TOGGLES_KEY = "toggles";

        [DllImport("__Internal")]
        private static extern void EM_ConsentDialog_Show(string title,
                                                       string contentElements,
                                                       bool isDismissible,
                                                       ref iOSConsentDialogListenerInfo listenerInfo);

        [DllImport("__Internal")]
        private static extern bool EM_ConsentDialog_IsShowing();

        [DllImport("__Internal")]
        private static extern void EM_ConsentDialog_SetButtonEnabled(string buttonId, bool enabled);

        [DllImport("__Internal")]
        private static extern void EM_ConsentDialog_SetToggleEnabled(string toggleId, bool enabled);

        [DllImport("__Internal")]
        private static extern void EM_ConsentDialog_SetToggleIsOn(string toggleId, bool isOn, bool animated);

        internal static iOSConsentDialog Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new iOSConsentDialog();

                return sInstance;
            }
        }

        private static iOSConsentDialog sInstance;

        private NativeConsentDialogListener mListener;

        private iOSConsentDialog()
        {
            mListener = NativeConsentDialogListener.GetListener();
            mListener.ToggleStateUpdated += OnNativeToggleStateUpdated;
            mListener.DialogCompleted += OnNativeDialogCompleted;
            mListener.DialogDismissed += OnNativeDialogDismissed;
        }

        //--------------------------------------------------------
        // IPlatformConsentDialog implementation
        //--------------------------------------------------------

        public event Action<IPlatformConsentDialog, string, bool> ToggleStateUpdated;

        public event Action<IPlatformConsentDialog, string, Dictionary<string, bool>> Completed;

        public event Action<IPlatformConsentDialog> Dismissed;

        public bool IsShowing()
        {
            return EM_ConsentDialog_IsShowing();
        }

        public void Show(string title, string content, bool isDimissible)
        {
            var listenerInfo = GetListenerInfo(mListener);
            EM_ConsentDialog_Show(title, content, isDimissible, ref listenerInfo);
        }

        public void SetButtonInteractable(string buttonId, bool interactable)
        {
            EM_ConsentDialog_SetButtonEnabled(buttonId, interactable);
        }

        public void SetToggleInteractable(string toggleId, bool interactable)
        {
            EM_ConsentDialog_SetToggleEnabled(toggleId, interactable);
        }

        public void SetToggleIsOn(string toggleId, bool isOn, bool animated)
        {
            EM_ConsentDialog_SetToggleIsOn(toggleId, isOn, animated);
        }

        private void OnNativeToggleStateUpdated(string toogleId, bool isOn)
        {
            if (ToggleStateUpdated != null)
                ToggleStateUpdated(this, toogleId, isOn);
        }

        private void OnNativeDialogCompleted(string jsonResults)
        {
            var results = Json.Deserialize(jsonResults) as Dictionary<string, object>;
            var buttonId = results[JSON_RESULTS_BUTTON_ID_KEY] as string;
            var toggles = results[JSON_RESULTS_TOGGLES_KEY] as Dictionary<string, object>;

            if (Completed != null)
                Completed(this, buttonId, toggles != null ? toggles.ToDictionary(pair => pair.Key, pair => (bool)pair.Value) : null);
        }

        private void OnNativeDialogDismissed()
        {
            if (Dismissed != null)
                Dismissed(this);
        }

        #region Internal Stuff

        internal struct iOSConsentDialogListenerInfo
        {
            public string name;
            public string onToggleBecameOnHandler;
            public string onToggleBecameOffHandler;
            public string onDialogCompletedHandler;
            public string onDialogDismissedHandler;
        }

        private iOSConsentDialogListenerInfo GetListenerInfo(NativeConsentDialogListener listener)
        {
            var info = new iOSConsentDialogListenerInfo();
            info.name = listener.ListenerName;
            info.onToggleBecameOnHandler = listener.ToggleBecameOnHandlerName;
            info.onToggleBecameOffHandler = listener.ToggleBecameOffHandlerName;
            info.onDialogCompletedHandler = listener.DialogCompletedHandlerName;
            info.onDialogDismissedHandler = listener.DialogDismissedHandlerName;

            return info;
        }

        #endregion  // Internal Types
    }
}
#endif