#if UNITY_IOS && EM_CONTACTS
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS;
using System;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal.iOS.Contact
{
    internal class InternalCNKeyDescriptor : iOSObjectProxy, CNKeyDescriptor
    {
        internal InternalCNKeyDescriptor(IntPtr selfPointer)
            : base(selfPointer)
        {
        }
    }
}
#endif