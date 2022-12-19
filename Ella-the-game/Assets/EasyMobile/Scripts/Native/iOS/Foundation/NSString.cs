#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.Foundation
{
    /// <summary>
    /// A static, plain-text Unicode string object.
    /// </summary>
    internal class NSString : iOSObjectProxy
    {
        internal NSString(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Creates a NSString object from a pointer and release that pointer if required.
        /// </summary>
        /// <returns>The pointer.</returns>
        /// <param name="ptr">Ptr.</param>
        /// <param name="releasePointer">If set to <c>true</c> release pointer.</param>
        internal static NSString FromPointer(IntPtr ptr, bool releasePointer)
        {
            if (PInvokeUtil.IsNull(ptr))
                return null;

            var str = new NSString(ptr);
            if (releasePointer)
                CFFunctions.CFRelease(ptr);
            return str;
        }

        /// <summary>
        /// Returns a string created by copying the data from a given C array of UTF8-encoded bytes.
        /// </summary>
        /// <returns>The with UT f8 string.</returns>
        /// <param name="nullTerminatedCString">Null terminated C string.</param>
        public static NSString StringWithUTF8String(string nullTerminatedCString)
        {
            IntPtr ptr = C.NSString_stringWithUTF8String(nullTerminatedCString);
            var newNSString = new NSString(ptr);
            CoreFoundation.CFFunctions.CFRelease(ptr);   // balance reference count on native.
            return newNSString;
        }

        /// <summary>
        /// A null-terminated UTF8 representation of the string.
        /// </summary>
        /// <value>The UT f8 string.</value>
        public string UTF8String
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    C.NSString_UTF8String(SelfPtr(), strBuffer, strLen));
            }
        }

        public override string ToString()
        {
            return string.Format("[NSString: UTF8String={0}]", UTF8String);
        }

        public override bool Equals(object obj)
        {
            var other = obj as NSString;

            if (other == null)
                return false;

            string thisUTF8String = this.UTF8String;
            string otherUTF8String = other.UTF8String;

            if (string.IsNullOrEmpty(thisUTF8String))
                return string.IsNullOrEmpty(otherUTF8String);

            return thisUTF8String.Equals(otherUTF8String);
        }

        public override int GetHashCode()
        {
            string thisUTF8String = this.UTF8String;
            return thisUTF8String == null ? 0 : thisUTF8String.GetHashCode();
        }

        public static bool operator ==(NSString nsstringA, NSString nsstringB)
        {
            if (ReferenceEquals(nsstringA, null))
                return ReferenceEquals(nsstringB, null);

            return nsstringA.Equals(nsstringB);
        }

        public static bool operator !=(NSString nsstringA, NSString nsstringB)
        {
            return !(nsstringA == nsstringB);
        }

        #region C Wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern IntPtr NSString_stringWithUTF8String(string nullTerminatedCString);

            [DllImport("__Internal")]
            internal static extern int NSString_UTF8String(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);
        }

        #endregion
    }
}
#endif