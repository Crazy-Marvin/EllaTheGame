#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.UIKit
{
    internal class UIActivity : iOSObjectProxy
    {
        public class UIActivityType : NSString
        {
            internal UIActivityType(IntPtr selfPointer) : base(selfPointer)
            {
            }
        }

        internal UIActivity(IntPtr selfPointer)
            : base(selfPointer)
        {
        }
    }
}
#endif
