#if UNITY_IOS && EM_CONTACTS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.Internal.iOS.Contact;

namespace EasyMobile.iOS.Contacts
{
    internal class CNContactFormatter : iOSObjectProxy
    {
        /// <summary>
        /// The formatting styles for contact names.
        /// </summary>
        public enum CNContactFormatterStyle
        {
            /// <summary>
            /// Combines the contact name components into a full name.
            /// </summary>
            CNContactFormatterStyleFullName = 0,
            /// <summary>
            /// Combines the contact phonetic name components into a phonetic full name.
            /// </summary>
            CNContactFormatterStylePhoneticFullName
        }

        /// <summary>
        /// The formatting orders for contact names component.
        /// </summary>
        public enum CNContactDisplayNameOrder
        {
            /// <summary>
            /// Display name order by user default.
            /// </summary>
            CNContactDisplayNameOrderUserDefault = 0,
            /// <summary>
            /// Display name order by given name first.
            /// </summary>
            CNContactDisplayNameOrderGivenNameFirst,
            /// <summary>
            /// Display name order by family name first.
            /// </summary>
            CNContactDisplayNameOrderFamilyNameFirst
        }

        internal CNContactFormatter(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Returns the required key descriptor for the specified formatting style of the contact.
        /// </summary>
        /// <returns>The for required keys for style.</returns>
        /// <param name="style">Style.</param>
        public static CNKeyDescriptor DescriptorForRequiredKeysForStyle(CNContactFormatterStyle style)
        {
            InternalCNKeyDescriptor keyDesc = null;
            var ptr = C.CNContactFormatter_descriptorForRequiredKeysForStyle(style);
            if (PInvokeUtil.IsNotNull(ptr))
            {
                keyDesc = new InternalCNKeyDescriptor(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return keyDesc;
        }

        /// <summary>
        /// Returns the contact name, formatted with the specified formatter.
        /// </summary>
        /// <returns>The from contact.</returns>
        /// <param name="contact">Contact.</param>
        /// <param name="style">Style.</param>
        public static NSString StringFromContact(CNContact contact, CNContactFormatterStyle style)
        {
            if (contact == null)
                return null;

            NSString str = null;    
            var ptr = C.CNContactFormatter_stringFromContact(contact.ToPointer(), style);
            if (PInvokeUtil.IsNotNull(ptr))
            {
                str = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return str;
        }

        /// <summary>
        /// Returns the delimiter to use between name components.
        /// </summary>
        /// <returns>The for contact.</returns>
        /// <param name="contact">Contact.</param>
        public static NSString DelimiterForContact(CNContact contact)
        {
            if (contact == null)
                return null;

            NSString str = null;    
            var ptr = C.CNContactFormatter_delimiterForContact(contact.ToPointer());
            if (PInvokeUtil.IsNotNull(ptr))
            {
                str = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return str;
        }

        /// <summary>
        /// Returns the display name order.
        /// </summary>
        /// <returns>The order for contact.</returns>
        /// <param name="contact">Contact.</param>
        public static CNContactDisplayNameOrder NameOrderForContact(CNContact contact)
        {
            return contact == null ? CNContactDisplayNameOrder.CNContactDisplayNameOrderUserDefault : C.CNContactFormatter_nameOrderForContact(contact.ToPointer());
        }

#region C wrapper

        private static class C
        {
            // Class Methods
            [DllImport("__Internal")]
            internal static extern /* id<CNKeyDescriptor> */IntPtr CNContactFormatter_descriptorForRequiredKeysForStyle(CNContactFormatterStyle style);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContactFormatter_stringFromContact(/* CNContact */IntPtr contact, CNContactFormatterStyle style);

            // Manual Formatting
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContactFormatter_delimiterForContact(/* CNContact */IntPtr contact);

            [DllImport("__Internal")]
            internal static extern CNContactDisplayNameOrder CNContactFormatter_nameOrderForContact(/* CNContact */IntPtr contact);
        }

#endregion

    }
}
#endif