#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.UIKit
{
    internal class UIActivityViewController : UIViewController
    {
        internal UIActivityViewController(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Initializes and returns a new activity view controller object that acts on the specified data.
        /// </summary>
        /// <returns>The with activity items.</returns>
        /// <param name="activityItems">Activity items.</param>
        public static UIActivityViewController InitWithActivityItems(NSArray<iOSObjectProxy> activityItems)
        {
            if (activityItems == null || activityItems.Count < 1)
                return null;

            var pointer = C.UIActivityViewController_initWithActivityItems(activityItems.ToPointer(), IntPtr.Zero);   // ignoring applicationActivities for now

            if (PInvokeUtil.IsNotNull(pointer))
            {
                var obj = new UIActivityViewController(pointer);
                CoreFoundation.CFFunctions.CFRelease(pointer);
                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The list of services that should not be displayed.
        /// </summary>
        /// <value>The excluded activity types.</value>
        public NSArray<UIActivity.UIActivityType> ExcludedActivityTypes
        {
            get
            {
                var ptr = C.UIActivityViewController_excludedActivityTypes(SelfPtr());

                if (PInvokeUtil.IsNotNull(ptr))
                {
                    var types = new NSArray<UIActivity.UIActivityType>(ptr);
                    CoreFoundation.CFFunctions.CFRelease(ptr);
                    return types;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                C.UIActivityViewController_setExcludedActivityTypes(SelfPtr(), value != null ? value.ToPointer() : IntPtr.Zero);
            }
        }

        #region C wrapper

        private static class C
        {
            // Initializing the Activity View Controller

            [DllImport("__Internal")]
            internal static extern /* UIActivityViewController */ IntPtr UIActivityViewController_initWithActivityItems(/* NSArray */IntPtr activityItems, /* NSArray<__kindof UIActivity> */IntPtr applicationActivities);

            // Excluding Specific Activity Types

            [DllImport("__Internal")]
            internal static extern /* NSArray<UIActivityType> */
            IntPtr UIActivityViewController_excludedActivityTypes(/* UIActivityViewController */HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIActivityViewController_setExcludedActivityTypes(/* UIActivityViewController */HandleRef selfPtr, /* NSArray<UIActivityType> */IntPtr excludedActivityTypes);
        }

        #endregion
    }
}
#endif
