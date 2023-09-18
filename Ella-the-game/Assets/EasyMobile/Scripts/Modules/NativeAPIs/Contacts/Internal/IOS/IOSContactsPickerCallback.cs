#if UNITY_IOS && EM_CONTACTS
using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.ContactsUI;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal class iOSContactsPickerCallback : CNContactPickerDelegate
    {
        private readonly Action<string, Contact> callback;
        private readonly CNContactPickerViewController picker;

        private const string CancelMessage = "Progress has been canceled by user.";

        public iOSContactsPickerCallback(Action<string, Contact> callback, CNContactPickerViewController picker)
        {
            this.callback = callback;
            this.picker = picker;
        }

        public void ContactPickerDidCancel(CNContactPickerViewController picker)
        {
            InvokeCallback(CancelMessage, null);
            DismissPicker();
        }

        public void ContactPickerDidSelectContact(CNContactPickerViewController picker, CNContact contact)
        {
            if (contact == null)
            {
                InvokeCallback("Received a null contact data.", null);
                DismissPicker();
                return;
            }

            try
            {
                InvokeCallback(null, contact.ToContact());
            }
            catch (Exception e)
            {
                InvokeCallback(e.Message, null);
                Debug.Log(e.StackTrace);
            }
            finally
            {
                DismissPicker();
            }
        }

#region Redundant Callbacks

        public void ContactPickerDidSelectContactProperties(CNContactPickerViewController picker, NSArray<CNContactProperty> contactProperties)
        {

        }

        public void ContactPickerDidSelectContactProperty(CNContactPickerViewController picker, CNContactProperty contactProperty)
        {

        }

        public void ContactPickerDidSelectContacts(CNContactPickerViewController picker, NSArray<CNContact> contacts)
        {

        }

#endregion

        private  void InvokeCallback(string errorMessage, Contact contact)
        {
            if (callback == null)
                return;

            RuntimeHelper.RunOnMainThread(() => callback(errorMessage, contact));
        }

        private void DismissPicker()
        {
            if (picker != null)
                picker.DismissViewController(true, null);
        }
    }
}
#endif