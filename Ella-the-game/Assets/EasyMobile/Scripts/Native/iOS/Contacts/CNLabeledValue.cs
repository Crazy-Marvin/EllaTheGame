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
using EasyMobile.Internal.iOS.Contact;

namespace EasyMobile.iOS.Contacts
{
    /// <summary>
    /// A thread-safe class that defines an immutable value object that combines a 
    /// contact property value with a label, such as a contact phone number combined 
    /// with a label of Home, Work, or iPhone.
    /// </summary>
    internal class CNLabeledValue<T> : InternalCNLabeledValue where T : iOSObjectProxy
    {
        private NSString mIdentifier;
        private NSString mLabel;
        private T mValue;

        internal CNLabeledValue(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Returns a new labeled value identifier object with the specified label and value.
        /// </summary>
        /// <returns>The value with label.</returns>
        /// <param name="label">Label.</param>
        /// <param name="value">Value.</param>
        public static CNLabeledValue<T> LabeledValueWithLabel(NSString label, T value)
        {
            if (value == null)
                return null;

            var ptr = C.CNLabeledValue_labeledValueWithLabel(label == null ? IntPtr.Zero : label.ToPointer(), value.ToPointer());
            CNLabeledValue<T> instance = null;           
            if (PInvokeUtil.IsNotNull(ptr))
            {
                instance = new CNLabeledValue<T>(ptr);
                CFFunctions.CFRelease(ptr);
            }

            return instance;
        }

        /// <summary>
        /// A unique identifier for the labeled value object.
        /// </summary>
        /// <value>The identifier.</value>
        public NSString Identifier
        {
            get
            {
                if (mIdentifier == null)
                {
                    var ptr = C.CNLabeledValue_identifier(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mIdentifier = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }

                return mIdentifier;
            }
        }

        /// <summary>
        /// The label for a contact property value.
        /// </summary>
        /// <value>The label.</value>
        public NSString Label
        {
            get
            {
                if (mLabel == null)
                {
                    var ptr = C.CNLabeledValue_label(SelfPtr());

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
        /// Get the contact property value.
        /// </summary>
        /// <param name="constructor">Constructor to create object of type <see cref="T"/>.</param>
        public T GetValue(Func<IntPtr, T> constructor)
        {
            if (mValue == null)
            {
                var ptr = C.CNLabeledValue_value(SelfPtr());

                if (PInvokeUtil.IsNotNull(ptr))
                {
                    mValue = constructor(ptr);
                    CFFunctions.CFRelease(ptr);
                }
            }
            return mValue;
        }
    }
}
#endif