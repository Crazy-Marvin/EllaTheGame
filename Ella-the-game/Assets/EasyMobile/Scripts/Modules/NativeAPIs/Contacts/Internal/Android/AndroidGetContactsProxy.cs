#if UNITY_ANDROID && EM_CONTACTS
using System;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal class AndroidGetContactsProxy : AndroidJavaProxy
    {
        private const string NativeListenerName = "com.sglib.easymobile.androidnative.contacts.IGetAllContactsListener";

        private Action<string, Contact[]> callback;

        public AndroidGetContactsProxy(Action<string, Contact[]> callback) : base(NativeListenerName)
        {
            this.callback = callback;
        }

        public void OnNativeCallback(string error, AndroidJavaObject[] nativeContacts)
        {
            if (callback == null)
                return;
                
            if (!string.IsNullOrEmpty(error))
            {
                callback(error, null);
                return;
            }

            if (nativeContacts != null)
            {
                Contact[] contacts = new Contact[nativeContacts.Length];
                for (int i = 0; i < nativeContacts.Length; i++)
                    contacts[i] = new AndroidContactBridge(nativeContacts[i]).ToContact();

                callback(null, contacts);
                return;
            }

            callback("Unknown error.", null);
        }

        public override AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
        {
            var error = javaArgs[0] != null ? javaArgs[0].Call<string>("toString") : null;
            var contacts = javaArgs[1] != null ? AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(javaArgs[1].GetRawObject()) : null;
            OnNativeCallback(error, contacts);

            return null;
        }
    }
}
#endif

