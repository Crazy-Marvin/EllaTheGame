using System;
using UnityEngine;
using System.Collections.Generic;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal interface INativeContactsProvider
    {
        bool IsFetchingContacts { get; }

        /// <summary>
        /// Get all the contacts in the devices.
        /// </summary>
        /// <param name="callback">
        /// Param 1: Error, null means success.
        /// Param 2: All contacts on the device, null if there's an error.
        /// </param>
        void GetContacts(Action<string, Contact[]> callback);

        /// <summary>
        /// Add new contact into the device.
        /// </summary>
        /// <returns>Error, null if new contact has been added successfully.</returns>
        string AddContact(Contact contact);

        /// <summary>
        /// Delete a contact from the device.
        /// </summary>
        /// <param name="id">Contact's id.</param>
        /// <returns> Error, null if the contact has been deleted successfully.</returns>
        string DeleteContact(string id);

        /// <summary>
        /// Open native UI to pick up contacts.
        /// </summary>
        /// <param name="callback">
        /// Param 1: Error, null if success.
        /// Param 2: Selected contact, null if error orcurs.
        /// </param>
        void PickContact(Action<string, Contact> callback);
    }
}
