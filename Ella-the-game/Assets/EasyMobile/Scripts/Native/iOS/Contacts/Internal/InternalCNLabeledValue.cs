#if UNITY_IOS && EM_CONTACTS
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS;
using System;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal.iOS.Contact
{
    internal abstract class InternalCNLabeledValue : iOSObjectProxy
    {
        /// <summary>
        /// The type of value of the CNLabelValue.
        /// </summary>
        public enum CNLabeledValueType
        {
            /// <summary>
            ///The value is a phonenumber. 
            /// </summary>
            TypePhoneNumber = 0,
            /// <summary>
            ///The value is a postalAddress. 
            /// </summary>
            TypePostalAddress = 1,
            /// <summary>
            ///The value is a string. 
            /// </summary>
            TypeString = 2,
        }

        internal InternalCNLabeledValue(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

#region C wrapper

        protected static class C
        {
            [DllImport("__Internal")]
            internal static extern /* CNLabelValue */IntPtr CNLabeledValue_labeledValueWithLabel(/* NSString */IntPtr label, /* ValueType */IntPtr value);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNLabeledValue_identifier(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNLabeledValue_label(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* ValueType */IntPtr CNLabeledValue_value(HandleRef self);

        }

#endregion
    }
}
#endif