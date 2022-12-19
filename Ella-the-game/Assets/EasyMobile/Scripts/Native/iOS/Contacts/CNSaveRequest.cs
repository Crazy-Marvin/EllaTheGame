#if UNITY_IOS && EM_CONTACTS
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyMobile.iOS.Contacts
{
    /// <summary>
    /// A save request operation for contacts.
    /// </summary>
    internal class CNSaveRequest : iOSObjectProxy
    {
        internal CNSaveRequest(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        public CNSaveRequest()
            : base(C.CNSavedRequest_new())
        {
            CFFunctions.CFRelease(SelfPtr());
        }

        /// <summary>
        /// Adds the specified contact to the contact store.
        /// </summary>
        /// <param name="contact">Contact.</param>
        /// <param name="identifier">Identifier.</param>
        public void AddContactToContainerWithIdentifier(CNMutableContact contact, NSString identifier)
        {
            Util.NullArgumentTest(contact);
            C.CNSaveRequest_addContactToContainerWithIdentifier(
                SelfPtr(),
                contact.ToPointer(),
                identifier == null ? IntPtr.Zero : identifier.ToPointer());
        }

        /// <summary>
        /// Updates an existing contact in the contact store.
        /// </summary>
        /// <param name="contact">Contact.</param>
        public void UpdateContact(CNMutableContact contact)
        {
            Util.NullArgumentTest(contact);
            C.CNSaveRequest_updateContact(SelfPtr(), contact.ToPointer());
        }

        /// <summary>
        /// Deletes a contact from the contact store.
        /// </summary>
        /// <param name="contact">Contact.</param>
        public void DeleteContact(CNMutableContact contact)
        {
            if (contact != null)
                C.CNSaveRequest_deleteContact(SelfPtr(), contact.ToPointer());
        }

#region C wrapper

        private static class C
        {
            // Constructor
            [DllImport("__Internal")]
            internal static extern /* CNSaveRequest */IntPtr CNSavedRequest_new();

            // Saving a Contact Changes
            [DllImport("__Internal")]
            internal static extern void CNSaveRequest_addContactToContainerWithIdentifier(HandleRef selfPointer, /* CNMutableContact */IntPtr contact, /* NSString */IntPtr identifier);

            [DllImport("__Internal")]
            internal static extern void CNSaveRequest_updateContact(HandleRef selfPointer, /* CNMutableContact */IntPtr contact);

            [DllImport("__Internal")]
            internal static extern void CNSaveRequest_deleteContact(HandleRef selfPointer, /* CNMutableContact */IntPtr contact);
        }

#endregion

    }
}
#endif