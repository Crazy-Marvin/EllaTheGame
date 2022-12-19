#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class AndroidMediaCollectedProxy : AndroidJavaProxy
    {
        private const string NativeListenerName = "com.sglib.easymobile.androidnative.media.listeners.IMediaCollectedListener";

        private Action<string, MediaResult> callback = null;

        public AndroidMediaCollectedProxy(Action<string, MediaResult> callback)
            : base(NativeListenerName)
        {
            this.callback = callback;
        }

        public void OnNativeCallback(string error, AndroidJavaObject result)
        {
            if (callback == null)
                return;

            RuntimeHelper.RunOnMainThread(() => callback(error, (MediaResult)new AndroidMediaResultBridge(result)));
        }

        public override AndroidJavaObject Invoke(string methodName, object[] args)
        {
            var error = args[0] != null ? (string)args[0] : null;
            var result = args[1] != null ? (AndroidJavaObject)args[1] : null;
            OnNativeCallback(error, result);
            return null; 
        }
    }
}
#endif