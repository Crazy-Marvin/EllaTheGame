using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal class UnsupportedContactProvider : INativeContactsProvider
    {
        private const string UnsupportedMessage = "Contacts APIs aren't supported on this platform.";

        public bool IsFetchingContacts
        {
            get
            {
                Debug.LogWarning(UnsupportedMessage);
                return false;
            }
        }

        public void GetContacts(Action<string, Contact[]> callback)
        {
            Debug.LogWarning(UnsupportedMessage);
            callback(UnsupportedMessage, null);
        }

        public string AddContact(Contact contact)
        {
            Debug.LogWarning(UnsupportedMessage);
            return UnsupportedMessage;
        }

        public string DeleteContact(string id)
        {
            Debug.LogWarning(UnsupportedMessage);
            return UnsupportedMessage;
        }

        public void PickContact(Action<string, Contact> callback)
        {
            Debug.LogWarning(UnsupportedMessage);
        }
    }
}
