#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.iOS.CoreFoundation
{
    internal static partial class CFFunctions
    {
        /// <summary>
        /// Calls native CFRelease method on the specified pointer to relinquish ownership of it. 
        /// The only case this should be called is after creating an object using a pointer
        /// obtained as a return value of a native method, instead of as a callback argument.
        /// Use this with care as it may cause memory issues due to unbalanced reference count
        /// on the native side.
        /// </summary>
        /// <param name="pointer">Pointer.</param>
        public static void CFRelease(IntPtr pointer)
        {
            C.CFType_CFRelease(pointer);
        }

        /// <summary>
        /// Calls native CFRelease method on the specified pointer to relinquish ownership of it. 
        /// The only case this should be called is after creating an object using a pointer
        /// obtained as a return value of a native method, instead of as a callback argument.
        /// Use this with care as it may cause memory issues due to unbalanced reference count
        /// on the native side.
        /// </summary>
        /// <param name="handleRef">Handle reference.</param>
        public static void CFRelease(HandleRef handleRef)
        {
            CFRelease(handleRef.Handle);
        }

        /// <summary>
        /// Calls native CFRetain method on the specified pointer to increase it reference count.
        /// Use this with care as it may cause memory issues due to unbalanced reference count
        /// on the native side.
        /// </summary>
        /// <param name="pointer">Pointer.</param>
        public static void CFRetain(IntPtr pointer)
        {
            C.CFType_CFRetain(pointer);
        }

        /// <summary>
        /// Calls native CFRetain method on the specified pointer to increase it reference count.
        /// Use this with care as it may cause memory issues due to unbalanced reference count
        /// on the native side.
        /// </summary>
        /// <param name="handleRef">Handle reference.</param>
        public static void CFRetain(HandleRef handleRef)
        {
            CFRetain(handleRef.Handle);
        }

        #region C wrapper

        private static partial class C
        {
            [DllImport("__Internal")]
            internal static extern void CFType_CFRelease(IntPtr pointer);

            [DllImport("__Internal")]
            internal static extern void CFType_CFRetain(IntPtr pointer);
        }

        #endregion
    }
}
#endif