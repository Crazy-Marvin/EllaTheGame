#if UNITY_IOS && EM_CONTACTS
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.ContactsUI;

namespace EasyMobile.Internal.iOS.ContactsUI
{
    internal class CNContactPickerDelegateForwarder : iOSDelegateForwarder<CNContactPickerDelegate>
    {
        internal CNContactPickerDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static CNContactPickerDelegateForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<CNContactPickerDelegateForwarder>.FromPointer(
                pointer,
                ptr => new CNContactPickerDelegateForwarder(ptr));
        }

        internal CNContactPickerDelegateForwarder()
            : this(C.CNContactPickerDelegateForwarder_new(
                    InternalContactPickerDidCancelCallback,
                    InternalContactPickerDidSelectContactCallback,
                    InternalContactPickerDidSelectContactPropertyCallback,
                    InternalContactPickerDidSelectContactsCallback,
                    InternalContactPickerDidSelectContactPropertiesCallback))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalContactPickerDidCancel))]
        private static void InternalContactPickerDidCancelCallback(IntPtr delegatePtr, IntPtr pickerPtr)
        {
            var forwarder = FromPointer(delegatePtr);
        
            if (forwarder != null && forwarder.Listener != null)
            {
                // Picker.
                var picker = InteropObjectFactory<CNContactPickerViewController>.FromPointer(pickerPtr, ptr => new CNContactPickerViewController(ptr));

                // Invoke consumer delegates.
                forwarder.InvokeOnListener(l => l.ContactPickerDidCancel(picker));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalContactPickerDidSelectContact))]
        private static void InternalContactPickerDidSelectContactCallback(IntPtr delegatePtr, IntPtr pickerPtr, IntPtr contactPtr)
        {
            var forwarder = FromPointer(delegatePtr);
        
            if (forwarder != null && forwarder.Listener != null)
            {
                var picker = InteropObjectFactory<CNContactPickerViewController>.FromPointer(pickerPtr, ptr => new CNContactPickerViewController(ptr));
                var contact = InteropObjectFactory<CNContact>.FromPointer(contactPtr, ptr => new CNContact(ptr));
                forwarder.InvokeOnListener(l => l.ContactPickerDidSelectContact(picker, contact));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalContactPickerDidSelectContactProperty))]
        private static void InternalContactPickerDidSelectContactPropertyCallback(IntPtr delegatePtr, IntPtr pickerPtr, IntPtr contactPropertyPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var picker = InteropObjectFactory<CNContactPickerViewController>.FromPointer(pickerPtr, ptr => new CNContactPickerViewController(ptr));
                var contact = InteropObjectFactory<CNContactProperty>.FromPointer(contactPropertyPtr, ptr => new CNContactProperty(ptr));
                forwarder.InvokeOnListener(l => l.ContactPickerDidSelectContactProperty(picker, contact));
            }           
        }

        [MonoPInvokeCallback(typeof(C.InternalContactPickerDidSelectContacts))]
        private static void InternalContactPickerDidSelectContactsCallback(IntPtr delegatePtr, IntPtr pickerPtr, IntPtr contactsPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var picker = InteropObjectFactory<CNContactPickerViewController>.FromPointer(pickerPtr, ptr => new CNContactPickerViewController(ptr));
                var contacts = InteropObjectFactory<NSArray<CNContact>>.FromPointer(contactsPtr, ptr => new NSArray<CNContact>(ptr));
                forwarder.InvokeOnListener(l => l.ContactPickerDidSelectContacts(picker, contacts));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalContactPickerDidSelectContactProperties))]
        private static void InternalContactPickerDidSelectContactPropertiesCallback(IntPtr delegatePtr, IntPtr pickerPtr, IntPtr contactPropertiesPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var picker = InteropObjectFactory<CNContactPickerViewController>.FromPointer(pickerPtr, ptr => new CNContactPickerViewController(ptr));
                var contactProperties = InteropObjectFactory<NSArray<CNContactProperty>>.FromPointer(contactPropertiesPtr, ptr => new NSArray<CNContactProperty>(ptr));
                forwarder.InvokeOnListener(l => l.ContactPickerDidSelectContactProperties(picker, contactProperties));
            }
        }


#region C wrapper

        private static class C
        {
            internal delegate void InternalContactPickerDidCancel(
            /* CNContactPickerDelegateForwarder */IntPtr delegatePointer,
            /* CNContactPickerViewController */IntPtr pickerPointer);

            internal delegate void InternalContactPickerDidSelectContact(
            /* CNContactPickerDelegateForwarder */IntPtr delegatePointer,
            /* CNContactPickerViewController */IntPtr pickerPointer,
            /* CNContact */IntPtr contactPointer);

            internal delegate void InternalContactPickerDidSelectContactProperty(
            /* CNContactPickerDelegateForwarder */IntPtr delegatePointer,
            /* CNContactPickerViewController */IntPtr pickerPointer,
            /* CNContactProperty */IntPtr contactPropertyPointer);

            internal delegate void InternalContactPickerDidSelectContacts(
            /* CNContactPickerDelegateForwarder */IntPtr delegatePointer,
            /* CNContactPickerViewController */IntPtr pickerPointer,
            /* NSArray<CNContact *> */IntPtr contactsPointer);

            internal delegate void InternalContactPickerDidSelectContactProperties(
            /* CNContactPickerDelegateForwarder */IntPtr delegatePointer,
            /* CNContactPickerViewController */IntPtr pickerPointer,
            /* NSArray<CNContactProperty *> */IntPtr contactPropetiesPointer);

            [DllImport("__Internal")]
            internal static extern /* CNContactPickerDelegateForwarder */IntPtr CNContactPickerDelegateForwarder_new(
                InternalContactPickerDidCancel didCancel,
                InternalContactPickerDidSelectContact didSelectContact,
                InternalContactPickerDidSelectContactProperty didSelectContactProperty,
                InternalContactPickerDidSelectContacts didSelectContacts,
                InternalContactPickerDidSelectContactProperties didSelectContactProperties);
        }

#endregion
    }
}
#endif