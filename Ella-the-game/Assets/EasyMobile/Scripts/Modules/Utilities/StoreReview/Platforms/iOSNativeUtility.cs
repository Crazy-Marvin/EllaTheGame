using UnityEngine;
using System.Collections;

#if UNITY_IOS
using System.Runtime.InteropServices;

namespace EasyMobile.Internal.StoreReview.iOS
{
    internal static class iOSNativeUtility
    {
        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EM_IsBuiltinRequestReviewAvail();

        [DllImport("__Internal")]
        private static extern void EM_RequestReview();

        internal static bool CanUseBuiltinRequestReview()
        {
            return EM_IsBuiltinRequestReviewAvail();
        }

        internal static void RequestReview()
        {
            EM_RequestReview();
        }
    }
}
#endif
