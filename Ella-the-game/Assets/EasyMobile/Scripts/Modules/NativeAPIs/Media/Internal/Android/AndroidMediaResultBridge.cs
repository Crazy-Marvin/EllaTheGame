#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    /// <summary>
    /// Class the convert from native MediaResult class to <see cref="MediaResult"/>.
    /// </summary>
    internal class AndroidMediaResultBridge
    {
        private const string NativeTypeName = "getType";
        private const string NativeContentUriName = "getContentUri";
        private const string NativeAbsoluteUriName = "getAbsoluteUri";

        public MediaType Type
        {
            get
            {
                if (nativeObject == null)
                    return MediaType.None;

                return (MediaType)nativeObject.Call<int>(NativeTypeName);
            }
        }

        public string ContentUri
        {
            get
            {
                if (nativeObject == null)
                    return null;

                return nativeObject.Call<string>(NativeContentUriName);
            }
        }

        public string AbsoluteUri
        {
            get
            {
                if (nativeObject == null)
                    return null;

                return nativeObject.Call<string>(NativeAbsoluteUriName);
            }
        }

        private AndroidJavaObject nativeObject = null;

        public AndroidMediaResultBridge(AndroidJavaObject nativeObject)
        {
            this.nativeObject = nativeObject;
        }

        public static explicit operator MediaResult(AndroidMediaResultBridge result)
        {
            if (result == null)
                return null;

            return new MediaResult(result.Type, result.ContentUri, result.AbsoluteUri);
        }
    }
}
#endif