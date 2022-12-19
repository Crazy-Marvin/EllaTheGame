using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal.NativeAPIs.Contacts;

namespace EasyMobile
{
    /// <summary>
    /// Entry class for contact APIs.
    /// </summary>
    public static class DeviceContacts
    {
        internal static INativeContactsProvider sNativeProvider = null;
        private const string InvalidNameMessage = "Name can't be null or empty.";
        private const string InvalidPhonenumber = "Phone number can't be null or empty.";
        private const string InvalidContactMessage = "Contact can't be null.";
        private const string InvalidContactIdMessage = "Can't delete a contact with null id.";
        private const string NullContactsProviderMessage = "The contacts provider hasn't been initialized.";

        private static INativeContactsProvider NativeProvider
        {
            get
            {
                if (sNativeProvider == null)
                    sNativeProvider = GetNativeContactsProvider();

                return sNativeProvider;
            }
        }

        /// <summary>
        /// Check if the previous <see cref="GetContacts(Action{string, Contact[]})"/> method is still running.
        /// </summary>
        public static bool IsFetchingContacts
        {
            get { return NativeProvider != null && NativeProvider.IsFetchingContacts; }
        }

        /// <summary>
        /// Get all the contacts in the devices.
        /// </summary>
        /// <param name="callback">
        /// Param 1: Error, null means success.
        /// Param 2: All contacts on the device, null if there's an error.
        /// </param>
        public static void GetContacts(Action<string, Contact[]> callback)
        {
            if (callback == null)
                return;

            if (NativeProvider == null)
            {
                callback(NullContactsProviderMessage, null);
                return;
            }

            NativeProvider.GetContacts(callback);
        }

        /// <summary>
        /// Add new contact into the device.
        /// </summary>
        /// <param name="contact">Contact's info. Note that the "id" field will be ignored.</param>
        /// <returns>Error, null if the contact has been added successfully.</returns>
        public static string AddContact(Contact contact)
        {
            if (contact == null)
                return InvalidContactMessage;

            return NativeProvider.AddContact(contact);
        }

        /// <summary>
        /// Delete a contact from the device.
        /// </summary>
        /// <param name="id">Contact's id.</param>
        /// <returns>Error, null if the contact has been deleted successfully.</returns>
        public static string DeleteContact(string id)
        {
            if (id == null)
                return InvalidContactIdMessage;

            return NativeProvider.DeleteContact(id);
        }

        /// <summary>
        /// Delete a contact from the device.
        /// </summary>
        /// <param name="contact">Contact's info.</param>
        /// <returns>Error, null if the contact has been deleted successfully.</returns>
        public static string DeleteContact(Contact contact)
        {
            if (contact == null)
                return NullContactsProviderMessage;

            return DeleteContact(contact.Id);
        }

        /// <summary>
        /// Open native UI to pick up contacts.
        /// </summary>        /// <param name="callback">
        /// Param 1: Error, null if success.
        /// Param 2: All selected contacts.
        ///</param>
        public static void PickContact(Action<string, Contact> callback)
        {
            NativeProvider.PickContact(callback);
        }

        private static INativeContactsProvider GetNativeContactsProvider()
        {
#if !EM_CONTACTS
            Debug.LogError("Contacts submodule is currently disable. Please enable it to use DeviceContacts API.");
            return null;
#elif UNITY_EDITOR
            return new UnsupportedContactProvider();
#elif UNITY_ANDROID
            return new AndroidContactsProvider();
#elif UNITY_IOS
            return new iOSContactsProvider();
#else
            return new UnsupportedContactProvider();
#endif
        }
    }
}
