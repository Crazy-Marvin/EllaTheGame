#if UNITY_ANDROID && EM_CONTACTS
using System;
using System.Collections.Generic;
using EasyMobile.Android;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal class StringStringPairBridge : EMAndroidJavaObject
    {
        private const string NativeClass = "com.sglib.easymobile.androidnative.contacts.StringStringPair";

        public StringStringPairBridge(AndroidJavaObject nativeObj) : base(nativeObj)
        {
        }

        public StringStringPairBridge(string first, string second) : base(NativeClass, first, second)
        {
        }

        public string First
        {
            get { return Call<string>("getFirst"); }
        }

        public string Second
        {
            get { return Call<string>("getSecond"); }
        }

        public KeyValuePair<string, string> ToKeyValuePair()
        {
            return new KeyValuePair<string, string>(First, Second);
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", First ?? "null", Second ?? "null");
        }
    }
}
#endif
