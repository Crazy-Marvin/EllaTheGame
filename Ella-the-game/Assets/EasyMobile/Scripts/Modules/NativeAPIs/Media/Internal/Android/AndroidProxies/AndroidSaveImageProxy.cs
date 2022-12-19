#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class AndroidSaveImageProxy : AndroidJavaProxy
    {
        private const string NativeListenerName = "com.sglib.easymobile.androidnative.media.listeners.ISaveImageListener";

        private Action<string> callback = null;

        public AndroidSaveImageProxy(Action<string> callback)
            : base(NativeListenerName)
        {
            this.callback = callback;
        }

        public void OnNativeCallback(string error)
        {
            if (callback == null)
                return;

            RuntimeHelper.RunOnMainThread(() => callback(error));
        }

        public override AndroidJavaObject Invoke(string methodName, object[] args)
        {
            var error = args[0] != null ? (string)args[0] : null;
            //var result = args[1] != null ? (string)args[1] : null; // The image url, still can be received...
            OnNativeCallback(error);
            return null;
        }
    }
}
#endif
