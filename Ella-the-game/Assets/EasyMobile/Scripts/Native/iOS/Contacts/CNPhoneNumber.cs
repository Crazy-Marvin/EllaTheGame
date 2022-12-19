#if UNITY_IOS && EM_CONTACTS
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.Contacts
{
    /// <summary>
    /// A thread-safe class that defines an immutable value object representing a 
    /// phone number for a contact.
    /// </summary>
    internal class CNPhoneNumber : iOSObjectProxy
    {
        private NSString mStringValue;

        internal CNPhoneNumber(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Returns a new phone number object initialized with the specified phone number string.
        /// </summary>
        /// <returns>The phone number</returns>
        /// <param name="value">A string value with which to initialize phone number object.</param>
        public static CNPhoneNumber PhoneNumberWithStringValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            
            var ptr = C.CNPhoneNumber_phoneNumberWithStringValue(value);

            if (PInvokeUtil.IsNotNull(ptr))
            {
                var cnPhoneNumber = new CNPhoneNumber(ptr);
                CFFunctions.CFRelease(ptr);   // release pointer returned by the native method.
                return cnPhoneNumber;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The string value of the phone number.
        /// </summary>
        /// <value>The string value.</value>
        public NSString StringValue
        {
            get
            {
                if (mStringValue == null)
                {
                    var ptr = C.CNPhoneNumber_stringValue(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mStringValue = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }

                return mStringValue;
            }
        }

#region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern  /* CNPhoneNumber */ IntPtr CNPhoneNumber_phoneNumberWithStringValue(string value);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPhoneNumber_stringValue(HandleRef self);
        }

#endregion
    }
}
#endif
