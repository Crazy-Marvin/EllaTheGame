#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EasyMobile.Internal.Sharing
{
    internal class AndroidSharingClient : ISharingClient
    {
        private const string NATIVE_SHARING_CLASS = "com.sglib.easymobile.androidnative.sharing.Sharing";
        private const string NATIVE_SHARE_TEXT_OR_URL_METHOD = "shareTextOrURL";
        private const string NATIVE_SHARE_IMAGE_METHOD = "shareImage";

        #region ISharingClient implementation


        public void ShareImage(string imagePath, string message, string subject = "")
        {
            AndroidUtil.CallJavaStaticMethod<string>(NATIVE_SHARING_CLASS, NATIVE_SHARE_IMAGE_METHOD, imagePath, message, subject);
        }

        public void ShareText(string text, string subject = "")
        {
            AndroidUtil.CallJavaStaticMethod<string>(NATIVE_SHARING_CLASS, NATIVE_SHARE_TEXT_OR_URL_METHOD, text, subject);
        }

        public void ShareURL(string url, string subject = "")
        {
            AndroidUtil.CallJavaStaticMethod<string>(NATIVE_SHARING_CLASS, NATIVE_SHARE_TEXT_OR_URL_METHOD, url, subject);
        }

        #endregion
    }
}
#endif
