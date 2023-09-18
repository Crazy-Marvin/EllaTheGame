#if UNITY_IOS && EM_CAMERA_GALLERY
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.UIKit
{
    internal static partial class UIFunctions
    {
#region Images And PDF - Photo Album

        /// <summary>
        /// Adds the specified image to the user’s Camera Roll album.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public static void UIImageWriteToSavedPhotosAlbum(UIImage image, Action<UIImage, NSError> completionHandler)
        {
            Util.NullArgumentTest(image);

            C.UIKit_UIImageWriteToSavedPhotosAlbum(
                image.ToPointer(),
                WriteToSavedPhotosAlbumCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

#endregion

#region Internal Callbacks

        [MonoPInvokeCallback(typeof(C.WriteToSavedPhotosAlbumCallback))]
        private static void WriteToSavedPhotosAlbumCallback(
            /* UIImage */IntPtr imagePtr, /* InteropNSError */IntPtr errorPtr, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var image = InteropObjectFactory<UIImage>.FromPointer(imagePtr, p => new UIImage(p));
            var error = PInvokeUtil.IsNotNull(errorPtr) ? new NSError(errorPtr) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "UIKitFunctions#WriteToSavedPhotosAlbumCallback",
                PInvokeCallbackUtil.Type.Temporary,
                image, error, secondaryCallback);
        }

#endregion

#region C wrapper

        private static partial class C
        {
            internal delegate void WriteToSavedPhotosAlbumCallback(
            /* UIImage */IntPtr image,
            /* NSError */IntPtr error,
            IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void UIKit_UIImageWriteToSavedPhotosAlbum(
                /* UIImage */IntPtr image,
                             WriteToSavedPhotosAlbumCallback callback,
                             IntPtr secondaryCallback);
        }

#endregion
    }
}
#endif // UNITY_IOS