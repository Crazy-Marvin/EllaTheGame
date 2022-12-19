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
    /// An object that represents a property of a contact.
    /// </summary>
    internal class CNContactProperty : iOSObjectProxy
    {
        protected CNContact mContact;
        protected NSString mKey;
        protected iOSObjectProxy mValue;
        protected NSString mLabel;
        protected NSString mIdentifier;

        internal CNContactProperty(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// <see cref="CNContact"/> property of the selected contact.
        /// </summary>
        /// <value>The contact.</value>
        public CNContact Contact
        {
            get
            {
                if (mContact == null)
                {
                    var ptr = C.CNContactProperty_contact(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mContact = new CNContact(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mContact;
            }
        }

        /// <summary>
        /// The key of the contact property.
        /// </summary>
        /// <value>The key.</value>
        public NSString Key
        {
            get
            {
                if (mKey == null)
                {
                    var ptr = C.CNContactProperty_key(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mKey = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mKey;
            }
        }

        /// <summary>
        /// The value of the property.
        /// </summary>
        /// <value>The value.</value>
        public iOSObjectProxy Value
        {
            get
            {
                if (mValue == null)
                {
                    var ptr = C.CNContactProperty_value(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mValue = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mValue;
            }
        }

        /// <summary>
        /// The label of the labeled value of the property array.
        /// </summary>
        /// <value>The label.</value>
        public NSString Label
        {
            get
            {
                if (mLabel == null)
                {
                    var ptr = C.CNContactProperty_label(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mLabel = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mLabel;
            }
        }

        /// <summary>
        /// The identifier of the labeled value in the array of labeled.
        /// </summary>
        /// <value>The identifier.</value>
        public NSString Identifier
        {
            get
            {
                if (mIdentifier == null)
                {
                    var ptr = C.CNContactProperty_identifier(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mIdentifier = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mIdentifier;
            }
        }

#region C wrapper

        private static class C
        {
            // Contact Properties
            [DllImport("__Internal")]
            internal static extern /* CNContact */IntPtr CNContactProperty_contact(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContactProperty_key(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern /* id */IntPtr CNContactProperty_value(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContactProperty_label(HandleRef selfPointer);

            // Contact Identifier
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContactProperty_identifier(HandleRef selfPointer);
        }

#endregion

    }
}
#endif