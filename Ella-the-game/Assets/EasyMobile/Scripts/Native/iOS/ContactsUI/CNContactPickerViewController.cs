#if UNITY_IOS && EM_CONTACTS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.Internal.iOS.ContactsUI;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.ContactsUI
{
    /// <summary>
    /// The CNContactPickerViewController class creates a controller object that manages
    /// the contacts picker view. This class allows the user to select one or more contacts 
    /// (or their properties) from the list of contacts displayed in the contact view controller 
    /// (CNContactViewController). The picker supports both single selection and multiselection 
    /// of the contacts. The app using contact picker view does not need access to the user’s 
    /// contacts and the user will not be prompted for “grant permission” access. 
    /// The app has access only to the user’s final selection.
    /// </summary>
    internal class CNContactPickerViewController : UIViewController
    {
        private CNContactPickerDelegate mDelegate;
        private CNContactPickerDelegateForwarder mDelegateForwarder;

        internal CNContactPickerViewController(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        internal CNContactPickerViewController()
            : base(C.CNContactPickerViewController_new())
        {
            CFFunctions.CFRelease(this.ToPointer());
        }

        /// <summary>
        /// The <see cref="EasyMobile.iOS.Contacts.CNContact"/> property keys to display in the contact detail card.
        /// </summary>
        /// <value>The displayed property keys.</value>
        public NSArray<NSString> DisplayedPropertyKeys
        {
            get
            {
                var ptr = C.CNContactPickerViewController_displayedPropertyKeys(SelfPtr());
                var nsArray = InteropObjectFactory<NSArray<NSString>>.FromPointer(ptr, p => new NSArray<NSString>(p));
                CFFunctions.CFRelease(ptr);
                return nsArray;
            }
            set
            {
                C.CNContactPickerViewController_setDisplayedPropertyKeys(SelfPtr(), value != null ? value.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The delegate to be notified when the user selects a contact or a property.
        /// </summary>
        /// <value>The delegate.</value>
        public CNContactPickerDelegate Delegate
        {
            get
            {
                return mDelegate;
            }
            set
            {
                mDelegate = value;
        
                if (mDelegate == null)
                {
                    // Nil out the native delegate.
                    mDelegateForwarder = null;
                    C.CNContactPickerViewController_setDelegate(SelfPtr(), IntPtr.Zero);
                }
                else
                {
                    // Create a delegate forwarder if needed.
                    if (mDelegateForwarder == null)
                    {
                        mDelegateForwarder = InteropObjectFactory<CNContactPickerDelegateForwarder>.Create(
                            () => new CNContactPickerDelegateForwarder(),
                            fwd => fwd.ToPointer());
        
                        // Assign on native side.
                        C.CNContactPickerViewController_setDelegate(SelfPtr(), mDelegateForwarder.ToPointer());
                    }
        
                    // Set delegate.
                    mDelegateForwarder.Listener = mDelegate;
                }
            }
        }

        /// <summary>
        /// A predicate to determine the contact selectability in the list of contacts.
        /// </summary>
        /// <value>The predicate for enabling contact.</value>
        public NSPredicate PredicateForEnablingContact
        {
            get
            {
                var ptr = C.CNContactPickerViewController_predicateForEnablingContact(SelfPtr());
                var predicate = InteropObjectFactory<NSPredicate>.FromPointer(ptr, p => new NSPredicate(p));
                CFFunctions.CFRelease(ptr);
                return predicate;
            }
            set
            {
                C.CNContactPickerViewController_setPredicateForEnablingContact(SelfPtr(), value != null ? value.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// A predicate to control the return of the selected contact.
        /// </summary>
        /// <value>The predicate for selection of contact.</value>
        public NSPredicate PredicateForSelectionOfContact
        {
            get
            {
                var ptr = C.CNContactPickerViewController_predicateForSelectionOfContact(SelfPtr());
                var predicate = InteropObjectFactory<NSPredicate>.FromPointer(ptr, p => new NSPredicate(p));
                CFFunctions.CFRelease(ptr);
                return predicate;
            }
            set
            {
                C.CNContactPickerViewController_setPredicateForSelectionOfContact(SelfPtr(), value != null ? value.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// A predicate to control the properties of the selected contact.
        /// </summary>
        /// <value>The predicate for selection of property.</value>
        public NSPredicate PredicateForSelectionOfProperty
        {
            get
            {
                var ptr = C.CNContactPickerViewController_predicateForSelectionOfProperty(SelfPtr());
                var predicate = InteropObjectFactory<NSPredicate>.FromPointer(ptr, p => new NSPredicate(p));
                CFFunctions.CFRelease(ptr);
                return predicate;
            }
            set
            {
                C.CNContactPickerViewController_setPredicateForSelectionOfProperty(SelfPtr(), value != null ? value.ToPointer() : IntPtr.Zero);
            }
        }

#region C wrapper

        private static class C
        {
            // Constructors
            [DllImport("__Internal")]
            internal static extern /* CNContactPickerViewController */IntPtr CNContactPickerViewController_new();

            // Displaying Contacts Properties
            [DllImport("__Internal")]
            internal static extern /* NSArray<NSString *> */
            IntPtr CNContactPickerViewController_displayedPropertyKeys(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactPickerViewController_setDisplayedPropertyKeys(HandleRef selfPointer, /* NSArray<NSString *> */IntPtr displayedPropertyKeys);

            // Notifying Delegate
            [DllImport("__Internal")]
            internal static extern /* CNContactPickerDelegateForwarder */IntPtr CNContactPickerViewController_delegate(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactPickerViewController_setDelegate(HandleRef selfPointer, /* CNContactPickerDelegateForwarder */IntPtr delegatePointer);

            // Predicates For Selecting Contacts
            [DllImport("__Internal")]
            internal static extern /* NSPredicate */IntPtr CNContactPickerViewController_predicateForEnablingContact(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactPickerViewController_setPredicateForEnablingContact(HandleRef selfPointer, /* NSPredicate */IntPtr predicateForEnablingContact);

            [DllImport("__Internal")]
            internal static extern /* NSPredicate */IntPtr CNContactPickerViewController_predicateForSelectionOfContact(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactPickerViewController_setPredicateForSelectionOfContact(HandleRef selfPointer, /* NSPredicate */IntPtr predicateForSelectionOfContact);

            [DllImport("__Internal")]
            internal static extern /* NSPredicate */
            IntPtr CNContactPickerViewController_predicateForSelectionOfProperty(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactPickerViewController_setPredicateForSelectionOfProperty(HandleRef selfPointer, /* NSPredicate */IntPtr predicateForSelectionOfProperty);
        }

#endregion
    }
}
#endif
