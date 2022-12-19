#if UNITY_IOS && EM_CONTACTS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.ContactsUI
{
    /// <summary>
    /// The CNContactPickerDelegate protocol describes the interface that CNContactPickerViewController 
    /// delegates must adopt to respond to contact-picker user events.
    /// </summary>
    internal interface CNContactPickerDelegate
    {
        /// <summary>
        /// In iOS, called when the user taps Cancel.
        /// </summary>
        /// <param name="picker">Picker.</param>
        void ContactPickerDidCancel(CNContactPickerViewController picker);

        /// <summary>
        /// Called after a contact has been selected by the user.
        /// </summary>
        /// <param name="picker">Picker.</param>
        /// <param name="contact">Contact.</param>
        void ContactPickerDidSelectContact(CNContactPickerViewController picker, CNContact contact);

        /// <summary>
        /// Called when a property of the contact has been selected by the user.
        /// </summary>
        /// <param name="picker">Picker.</param>
        /// <param name="contactProperty">Contact property.</param>
        void ContactPickerDidSelectContactProperty(CNContactPickerViewController picker, CNContactProperty contactProperty);

        /// <summary>
        /// Called after contacts have been selected by the user.
        /// </summary>
        /// <param name="picker">Picker.</param>
        /// <param name="contacts">Contacts.</param>
        void ContactPickerDidSelectContacts(CNContactPickerViewController picker, NSArray<CNContact> contacts);

        /// <summary>
        /// Called after contact properties have been selected by the user.
        /// </summary>
        /// <param name="picker">Picker.</param>
        /// <param name="contactProperties">Contact properties.</param>
        void ContactPickerDidSelectContactProperties(CNContactPickerViewController picker, NSArray<CNContactProperty> contactProperties);
    }
}
#endif
