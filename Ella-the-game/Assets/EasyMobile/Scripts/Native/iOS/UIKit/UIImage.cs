#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.UIKit
{
    using EasyMobile.iOS.CoreGraphics;

    internal class UIImage : iOSObjectProxy
    {
        private UIImage mNormalizedImage;

        internal UIImage(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Creates a UIImage object from a pointer and release the pointer if required.
        /// </summary>
        /// <returns>The UIImage object.</returns>
        /// <param name="pointer">Pointer.</param>
        /// <param name="releasePointer">If set to <c>true</c> release pointer.</param>
        internal static UIImage FromPointer(IntPtr pointer, bool releasePointer)
        {
            if (PInvokeUtil.IsNotNull(pointer))
            {
                var obj = new UIImage(pointer);
                if (releasePointer)
                    CoreFoundation.CFFunctions.CFRelease(pointer);
                return obj;
            }
            else
            {
                return null;
            }
        }

        #region UIImage API

        /// <summary>
        /// Creates and returns an image object by loading the image data from the file at the specified path.
        /// </summary>
        /// <returns>The with contents of file.</returns>
        /// <param name="path">Path.</param>
        public static UIImage ImageWithContentsOfFile(NSString path)
        {
            return path == null ? null : FromPointer(C.UIImage_imageWithContentsOfFile(path.ToPointer()), true);
        }

        /// <summary>
        /// Creates and returns an image object that uses the specified image data.
        /// </summary>
        /// <returns>The with data.</returns>
        /// <param name="data">Data.</param>
        public static UIImage ImageWithData(NSData data)
        {
            return data == null ? null : FromPointer(C.UIImage_imageWithData(data.ToPointer()), true);
        }

        /// <summary>
        /// Creates and returns an image object that uses the specified image data and scale factor.
        /// </summary>
        /// <returns>The with data.</returns>
        /// <param name="data">Data.</param>
        /// <param name="scale">Scale.</param>
        public static UIImage ImageWithData(NSData data, float scale)
        {
            return data == null ? null : FromPointer(C.UIImage_imageWithDataAtScale(data.ToPointer(), scale), true);
        }

        /// <summary>
        /// The scale factor of the image.
        /// </summary>
        /// <value>The scale.</value>
        public float Scale
        {
            get
            {
                return C.UIImage_scale(SelfPtr());
            }
        }

        /// <summary>
        /// The logical dimensions of the image, measured in points.
        /// </summary>
        /// <value>The size.</value>
        public CGSize Size
        {
            get
            {
                var size = new CGSize(0, 0);
                C.UIImage_size(SelfPtr(), ref size);
                return size;
            }
        }

        #endregion

        #region Custom API

        /// <summary>
        /// Returns a copy of the image with default orientation and
        /// the original pixel data matches the image's intended display orientation.
        /// </summary>
        /// <value>The normalized image.</value>
        public UIImage NormalizedImage
        {
            get
            {
                if (mNormalizedImage == null)
                {
                    var ptr = C.UIImage_normalizedImage(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mNormalizedImage = new UIImage(ptr);
                        CoreFoundation.CFFunctions.CFRelease(ptr);
                    }
                }
                return mNormalizedImage;
            }
        }

        /// <summary>
        /// Returns the data for the specified image in PNG format.
        /// </summary>
        /// <returns>The image PNG representation.</returns>
        /// <param name="image">The original image data.</param>
        public static byte[] UIImagePNGRepresentation(UIImage image)
        {
            return image == null ? null : PInvokeUtil.GetNativeArray<byte>((buffer, length) =>
                C.UIImage_PNGRepresentation(image.SelfPtr(), buffer, length));
        }

        /// <summary>
        /// Returns the data for the specified image in JPEG format.
        /// </summary>
        /// <returns>The image JPEG representation.</returns>
        /// <param name="image">The original image data.</param>
        /// <param name="compressionQuality">The quality of the resulting JPEG image, 
        /// expressed as a value from 0.0 to 1.0. The value 0.0 represents the maximum compression 
        /// (or lowest quality) while the value 1.0 represents the least compression (or best quality).</param>
        public static byte[] UIImageJPEGRepresentation(UIImage image, float compressionQuality)
        {
            if (image == null)
                return null;
            
            compressionQuality = UnityEngine.Mathf.Clamp(compressionQuality, 0, 1);
            return PInvokeUtil.GetNativeArray<byte>((buffer, length) =>
                C.UIImage_JPEGRepresentation(image.SelfPtr(), compressionQuality, buffer, length));
        }

        #endregion

        #region C wrapper

        private static class C
        {
            // Creating and Initializing Image Objects
            [DllImport("__Internal")]
            internal static extern /* UIImage */IntPtr UIImage_imageWithContentsOfFile(/* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern /* UIImage */IntPtr UIImage_imageWithData(/* NSData */IntPtr data);

            [DllImport("__Internal")]
            internal static extern /* UIImage */IntPtr UIImage_imageWithDataAtScale(/* NSData */IntPtr data, float scale);

            // Getting the Image Size and Scale
            [DllImport("__Internal")]
            internal static extern float UIImage_scale(HandleRef self);

            [DllImport("__Internal")]
            internal static extern float UIImage_size(HandleRef self, [In, Out] ref CGSize buffer);

            // Custom API
            [DllImport("__Internal")]
            internal static extern /* UIImage */IntPtr UIImage_normalizedImage(HandleRef self);

            [DllImport("__Internal")]
            internal static extern int UIImage_PNGRepresentation(
                HandleRef self, [In, Out] /* from(unsigned char *) */ byte[] buffer, int byteCount);

            [DllImport("__Internal")]
            internal static extern int UIImage_JPEGRepresentation(
                HandleRef self, float compressionQuality, [In, Out] /* from(unsigned char *) */ byte[] buffer, int byteCount);

        }

        #endregion
    }
}
#endif
