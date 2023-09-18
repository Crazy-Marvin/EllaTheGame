#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.UIKit
{
    internal static partial class UIFunctions
    {
        #region Images And PDF - Image Creation

        /// <summary>
        /// Returns the data for the specified image in JPEG format.
        /// </summary>
        /// <returns>The image JPEG representation.</returns>
        /// <param name="image">Image.</param>
        /// <param name="compressionQuality">Compression quality.</param>
        public static NSData UIImageJPEGRepresentation(UIImage image, float compressionQuality)
        {
            Util.NullArgumentTest(image);

            IntPtr ptr = C.UIKit_UIImageJPEGRepresentation(image.ToPointer(), compressionQuality);
            NSData data = new NSData(ptr);
            CoreFoundation.CFFunctions.CFRelease(ptr);
            return data;
        }

        /// <summary>
        /// Returns the data for the specified image in PNG format.
        /// </summary>
        /// <returns>The image PNG representation.</returns>
        /// <param name="image">Image.</param>
        public static NSData UIImagePNGRepresentation(UIImage image)
        {
            Util.NullArgumentTest(image);

            IntPtr ptr = C.UIKit_UIImagePNGRepresentation(image.ToPointer());
            NSData data = new NSData(ptr);
            CoreFoundation.CFFunctions.CFRelease(ptr);
            return data;
        }

        #endregion

        #region C wrapper

        private static partial class C
        {
            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr UIKit_UIImageJPEGRepresentation(/* UIImage */IntPtr image, float compressionQuality);

            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr UIKit_UIImagePNGRepresentation(/* UIImage */IntPtr image);
        }

        #endregion
    }
}
#endif // UNITY_IOS