#if UNITY_ANDROID
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class AndroidPickFromGalleryProxy : AndroidJavaProxy
    {
        private const string NativeListenerName = "com.sglib.easymobile.androidnative.media.listeners.IPickFromGalleryListener";

        private Action<string, MediaResult[]> callback = null;

        public AndroidPickFromGalleryProxy(Action<string, MediaResult[]> callback)
            : base(NativeListenerName)
        {
            this.callback = callback;
        }

        public void OnNativeCallback(string error, AndroidJavaObject[] results)
        {
            if (callback == null)
                return;

            RuntimeHelper.RunOnMainThread(() =>
                {
                    if (error != null)
                    {
                        callback(error, null);
                        return;
                    }

                    if (results != null)
                    {
                        var mediaResults = new MediaResult[results.Length];
                        for (int i = 0; i < results.Length; i++)
                            mediaResults[i] = (MediaResult)new AndroidMediaResultBridge(results[i]);

                        callback(error, mediaResults);
                        return;
                    }

                    callback("Unknown error.", null);
                });
        }

        public override AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
        {
            var error = javaArgs[0] != null ? javaArgs[0].Call<string>("toString") : null;
            var results = javaArgs[1] != null ? AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(javaArgs[1].GetRawObject()) : null;
            OnNativeCallback(error, results);
            return null;
        }
    }
}
#endif
