#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.Internal.iOS
{
    internal class iOSObjectProxy : InteropObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="iOSObjectProxy"/> class.
        /// </summary>
        /// <param name="selfPointer">Self pointer.</param>
        internal iOSObjectProxy(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Effectively increments the reference count of the native object
        /// at the given pointer. This is called inside the base constructor
        /// and should never be called anywhere else.
        /// </summary>
        /// <param name="selfPointer">Self pointer.</param>
        protected override void AttachHandle(HandleRef selfPointer)
        {
            CFFunctions.CFRetain(selfPointer);
        }

        /// <summary>
        /// Effectively decrements the reference count of the native object
        /// at the give pointer. This is called when the object is disposed
        /// and should never be called anywhere else.
        /// </summary>
        /// <param name="selfPointer">Self pointer.</param>
        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            CFFunctions.CFRelease(selfPointer);
        }
    }
}
#endif
