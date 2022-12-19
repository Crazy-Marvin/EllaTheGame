#if UNITY_ANDROID
using UnityEngine;
using System.Collections;

namespace EasyMobile.Internal
{
    public class AndroidAppLifecycleHandler : IAppLifecycleHandler
    {
        private const string ANDROID_JAVA_CLASS = "com.sglib.easymobile.androidnative.AppUtil";

        public void OnApplicationFocus(bool isFocus)
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "OnApplicationFocus",
                isFocus
            );
        }

        public void OnApplicationPause(bool isPaused)
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "OnApplicationPause",
                isPaused
            );
        }

        public void OnApplicationQuit()
        {
            AndroidUtil.CallJavaStaticMethod(
                ANDROID_JAVA_CLASS,
                "OnApplicationQuit"
            );
        }
    }
}
#endif