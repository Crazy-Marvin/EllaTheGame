#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal
{
    internal static class AndroidUtil
    {
        internal static void CallJavaStaticMethod(string className, string method, params object[] args)
        {
            using (AndroidJavaObject jObj = new AndroidJavaObject(className))
            {
                jObj.CallStatic(method, args);
            }
        }

        internal static T CallJavaStaticMethod<T>(string className, string method, params object[] args)
        {
            using (AndroidJavaObject jObj = new AndroidJavaObject(className))
            {
                return jObj.CallStatic<T>(method, args);
            }
        }
    }
}
#endif
