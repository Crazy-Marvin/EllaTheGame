using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

namespace EasyMobile.Internal.Privacy
{
    internal class NativeConsentDialogListener : MonoBehaviour
    {
        private const string NATIVE_CONSENT_DIALOG_LISTENER_GO = "EM_NativeConsentDialogListener";

        // Singleton.
        private static NativeConsentDialogListener sInstance;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            if (sInstance == this)
                sInstance = null;
        }

        #region Public API

        /// <summary>
        /// Occurs when toggle state updated.
        /// This event is always called on main thread.
        /// </summary>
        public event Action<string, bool> ToggleStateUpdated;

        /// <summary>
        /// Occurs when the dialog is completed.
        /// This event is always called on main thread.
        /// </summary>
        public event Action<string> DialogCompleted;

        /// <summary>
        /// Occurs when the dialog is dismissed.
        /// This event is always called on main thread.
        /// </summary>
        public event Action DialogDismissed;

        public string ListenerName
        {
            get { return gameObject.name; }
        }

        public string ToggleBecameOnHandlerName
        {
            get { return ReflectionUtil.GetMethodName((Action<string>)(this._OnNativeToggleBecameOn)); }
        }

        public string ToggleBecameOffHandlerName
        {
            get { return ReflectionUtil.GetMethodName((Action<string>)(this._OnNativeToggleBecameOff)); }
        }

        public string DialogCompletedHandlerName
        {
            get { return ReflectionUtil.GetMethodName((Action<string>)(this._OnNativeDialogCompleted)); }
        }

        public string DialogDismissedHandlerName
        {
            get { return ReflectionUtil.GetMethodName((Action<string>)(this._OnNativeDialogDismissed)); }
        }

        /// <summary>
        /// Creates a gameobject for use with UnitySendMessage from native side.
        /// Must be called from Unity game thread.
        /// </summary>
        public static NativeConsentDialogListener GetListener()
        {
            if (sInstance == null)
            {
                var go = new GameObject(NATIVE_CONSENT_DIALOG_LISTENER_GO);
                go.hideFlags = HideFlags.HideAndDontSave;
                sInstance = go.AddComponent<NativeConsentDialogListener>();
                DontDestroyOnLoad(go);
            }
            return sInstance;
        }

        #endregion // Public API

        #region Native-Invoked Methods

        /// The following methods are to be invoked from native side using UnitySendMessage,
        /// which means they always run on main thread
        /// https://forum.unity.com/threads/calling-unitysendmessage-from-background-thread.487113/#post-3175895

        private void _OnNativeToggleBecameOn(string toggleId)
        {
            if (ToggleStateUpdated != null)
                ToggleStateUpdated(toggleId, true);
        }

        private void _OnNativeToggleBecameOff(string toggleId)
        {
            if (ToggleStateUpdated != null)
                ToggleStateUpdated(toggleId, false);
        }

        private void _OnNativeDialogCompleted(string jsonResults)
        {
            if (DialogCompleted != null)
                DialogCompleted(jsonResults);
        }

        private void _OnNativeDialogDismissed(string s)
        {
            if (DialogDismissed != null)
                DialogDismissed();
        }

        #endregion // Native-Invoked Methods
    }
}