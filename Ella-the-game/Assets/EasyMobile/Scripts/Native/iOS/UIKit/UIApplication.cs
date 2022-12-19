#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace EasyMobile.iOS.UIKit
{
    internal class UIApplication
    {
        public static UIApplication SharedApplication
        {
            get
            {
                if (sSharedApplication == null)
                    sSharedApplication = new UIApplication();

                return sSharedApplication;
            }
        }

        private static UIApplication sSharedApplication;

        private UIApplication()
        {
        }

        public int GetApplicationIconBadgeNumber()
        {
            return C.UIApplication_applicationIconBadgeNumber();
        }

        public void SetApplicationIconBadgeNumber(int value)
        {
            C.UIApplication_setApplicationIconBadgeNumber(value);
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern int UIApplication_applicationIconBadgeNumber();

            [DllImport("__Internal")]
            internal static extern void UIApplication_setApplicationIconBadgeNumber(int value);
        }

        #endregion
    }
}
#endif