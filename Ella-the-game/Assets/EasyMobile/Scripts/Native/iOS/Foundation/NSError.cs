#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.Foundation
{
    /// <summary>
    /// Information about an error condition including a domain, 
    /// a domain-specific error code, and application-specific information.
    /// </summary>
    internal class NSError : iOSObjectProxy
    {
        internal NSError(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// The error code.
        /// </summary>
        /// <value>The code.</value>
        public int Code
        {
            get { return C.NSError_code(SelfPtr()); }
        }

        /// <summary>
        /// A string containing the error domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    C.NSError_domain(SelfPtr(), strBuffer, strLen));
            }
        }

        /// <summary>
        /// A string containing the localized description of the error.
        /// </summary>
        /// <value>The localized description.</value>
        public string LocalizedDescription
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    C.NSError_localizedDescription(SelfPtr(), strBuffer, strLen));
            }
        }

        /// <summary>
        /// A string containing the localized recovery suggestion for the error.
        /// </summary>
        /// <value>The localized recovery suggestion.</value>
        public string LocalizedRecoverySuggestion
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    C.NSError_localizedRecoverySuggestion(SelfPtr(), strBuffer, strLen));
            }
        }

        /// <summary>
        /// A string containing the localized explanation of the reason for the error.
        /// </summary>
        /// <value>The localized failure reason.</value>
        public string LocalizedFailureReason
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    C.NSError_localizedFailureReason(SelfPtr(), strBuffer, strLen));
            }
        }

        #region C Wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern int NSError_code(HandleRef self);

            [DllImport("__Internal")]
            internal static extern int NSError_domain(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSError_localizedDescription(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSError_localizedRecoverySuggestion(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int NSError_localizedFailureReason(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

        }

        #endregion
    }
}
#endif