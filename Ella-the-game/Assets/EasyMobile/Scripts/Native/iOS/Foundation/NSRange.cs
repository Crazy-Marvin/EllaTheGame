#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.iOS.Foundation
{
    /// <summary>
    /// A structure used to describe a portion of a series, 
    /// such as characters in a string or objects in an array.
    /// </summary>
    internal struct NSRange
    {
        public uint location;
        public uint length;

        public NSRange(uint loc, uint len)
        {
            this.location = loc;
            this.length = len;
        }
    }
}
#endif