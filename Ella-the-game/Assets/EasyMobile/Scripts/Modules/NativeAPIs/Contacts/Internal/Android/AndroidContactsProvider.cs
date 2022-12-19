#if UNITY_ANDROID && EM_CONTACTS
using System;
using System.Linq;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal class AndroidContactsProvider : INativeContactsProvider
    {
        private const string NativeClassName = "com.sglib.easymobile.androidnative.contacts.ContactsProvider";
        private const string NativeGetContactsMethodName = "getContacts";
        private const string NativeAddContactMethodName = "addContact";
        private const string NativeDeleteContactMethodName = "deleteContact";
        private const string NativePickContactMethodName = "pickContact";
        private const string NullNativeProviderMessage = "The Android native contacts provider hasn't been initialized!!!";
        private const string FetchingContactsThreadRunningMessage = "Another fetching contacts progress is running.";

        private AndroidJavaObject nativeProvider;
        private bool isFetchingContacts = false;

        public bool IsFetchingContacts
        {
            get
            {
                return isFetchingContacts;
            }
        }

        public AndroidContactsProvider()
        {
            nativeProvider = new AndroidJavaObject(NativeClassName);
        }

        public void GetContacts(Action<string, Contact[]> callback)
        {
            if (nativeProvider == null)
            {
                callback(NullNativeProviderMessage, null);
                return;
            }

            if (isFetchingContacts)
            {
                callback(FetchingContactsThreadRunningMessage, null);
                return; 
            }

            isFetchingContacts = true;
            nativeProvider.Call(NativeGetContactsMethodName, new AndroidGetContactsProxy((error, contacts) =>
            {

                RuntimeHelper.RunOnMainThread(() => callback(error, contacts));
                isFetchingContacts = false;
            }));
        }

        public string AddContact(Contact contact)
        {
            if (nativeProvider == null)
                return NullNativeProviderMessage;

            string[] phoneNumbersLabels = null;
            string[] phoneNumbers = null;
            if (contact.PhoneNumbers != null)
            {
                phoneNumbersLabels = contact.PhoneNumbers.Select(p => p.Key).ToArray();
                phoneNumbers = contact.PhoneNumbers.Select(p => p.Value).ToArray();
            }

            string[] emailsLabels = null;
            string[] emails = null;
            if (contact.Emails != null)
            {
                emailsLabels = contact.Emails.Select(e => e.Key).ToArray();
                emails = contact.Emails.Select(e => e.Value).ToArray(); 
            }

            string birthdayString = contact.Birthday != null ? contact.Birthday.Value.ToString("yyyy-MM-dd") : null;

            return nativeProvider.Call<string>(NativeAddContactMethodName,
                contact.FirstName, contact.MiddleName, contact.LastName,
                contact.Company, birthdayString,
                phoneNumbersLabels, phoneNumbers,
                emailsLabels, emails, TextureUtilities.Encode(contact.Photo, ImageFormat.PNG));
        }

        public string DeleteContact(string id)
        {
            if (nativeProvider == null)
                return NullNativeProviderMessage;
           
            return nativeProvider.Call<string>(NativeDeleteContactMethodName, id);
        }

        public void PickContact(Action<string, Contact> callback)
        {
            if (nativeProvider == null)
            {
                if (callback != null)
                    callback(NullNativeProviderMessage, null);

                return;
            }

            nativeProvider.Call(NativePickContactMethodName, new AndroidPickContactProxy(callback));
        }
    }
}
#endif
