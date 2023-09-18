#if UNITY_ANDROID && EM_CONTACTS
using System;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    public class AndroidPickContactProxy : AndroidJavaProxy
    {
        private const string NativeListenerName = "com.sglib.easymobile.androidnative.contacts.IPickContactListener";
        private Action<string, Contact> callback;

        public AndroidPickContactProxy(Action<string, Contact> callback) : base(NativeListenerName)
        {
            this.callback = callback;
        }

        public override AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
        {
            var error = javaArgs[0] != null ? javaArgs[0].Call<string>("toString") : null;
            var nativeContacts = javaArgs[1] != null ? javaArgs[1] : null;

            InvokeCallback(error, nativeContacts);
            return null;
        }

        private void InvokeCallback(string error, AndroidJavaObject nativeContacts)
        {
            if (callback == null)
                return;

            RuntimeHelper.RunOnMainThread(() =>
            {
                if (nativeContacts == null)
                {
                    callback.Invoke(error, null);
                    return;
                }

                Contact contact = (Contact)new AndroidContactBridge(nativeContacts);
                RuntimeHelper.RunOnMainThread(() => callback(error, contact));
            });
        }
    }
}
#endif
