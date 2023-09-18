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
    /// Indicates the sorting order for contacts.
    /// </summary>
    internal enum CNContactSortOrder
    {
        /// <summary>
        /// No sorting order.
        /// </summary>
        CNContactSortOrderNone = 0,
        /// <summary>
        /// The user’s default sorting order.
        /// </summary>
        CNContactSortOrderUserDefault,
        /// <summary>
        /// Sorting contacts by given name.
        /// </summary>
        CNContactSortOrderGivenName,
        /// <summary>
        /// Sorting contacts by family name.
        /// </summary>
        CNContactSortOrderFamilyName
    }
}
#endif