#if UNITY_ANDROID
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class AndroidLoadImageProxy : AndroidJavaProxy
    {
        private const string NativeListenerName = "com.sglib.easymobile.androidnative.media.listeners.ILoadImageListener";

        private Action<string, Texture2D> callback = null;

        public AndroidLoadImageProxy(Action<string, Texture2D> callback)
            : base(NativeListenerName)
        {
            this.callback = callback;
        }

        public void OnNativeCallback(string error, byte[] data)
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

                    try
                    {
                        var image = TextureUtilities.Decode(data);
                        callback(null, image);
                    }
                    catch (Exception e)
                    {
                        callback(e.Message, null);
                    }
                });
        }

        public override AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
        {
            var error = javaArgs[0] != null ? javaArgs[0].Call<string>("toString") : null;
            var data = javaArgs[1] != null ? (byte[])(Array)AndroidJNIHelper.ConvertFromJNIArray<sbyte[]>(javaArgs[1].GetRawObject()) : null;
            OnNativeCallback(error, data);
            return null;
        }
    }
}
#endif