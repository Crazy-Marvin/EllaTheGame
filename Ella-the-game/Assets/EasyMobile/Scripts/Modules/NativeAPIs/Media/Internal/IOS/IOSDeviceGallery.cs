#if UNITY_IOS && EM_CAMERA_GALLERY
using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.MobileCoreServices;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class iOSDeviceGallery : IDeviceGallery
    {
        private UIImagePickerController mPickerController;

        public void Pick(Action<string, MediaResult[]> callback)
        {
            Util.NullArgumentTest(callback);

            if (mPickerController != null)
            {
                Debug.Log("Ignoring Pick call because another image picker UI is being shown.");
                return;
            }

            callback = RuntimeHelper.ToMainThread(callback);

            // Create a new image picker.
            var picker = InteropObjectFactory<UIImagePickerController>.Create(
                             () => new UIImagePickerController(),
                             c => c.ToPointer());

            // Source type must be photo library.
            picker.SourceType = UIImagePickerController.SourceTypeEnum.PhotoLibrary;

            // Allow image & video.
            NSMutableArray<NSString> mediaTypes = new NSMutableArray<NSString>();
            mediaTypes.AddObject(UTTypeConstants.kUTTypeImage);
            mediaTypes.AddObject(UTTypeConstants.kUTTypeMovie);
            picker.MediaTypes = mediaTypes;

            // Create a delegate for the TBM VC.
            picker.Delegate = new InternalUIImagePickerControllerDelegate(InternalUIImagePickerControllerDelegate.PickerOperation.PickGallery)
            {
                CloseAndResetMatchmakerVC = () =>
                {
                    if (mPickerController != null)
                    {
                        mPickerController.DismissViewController(true, null);
                        mPickerController = null;
                    }
                },
                CompleteCallback = (error, result) =>
                {
                    callback(error, result != null ? new MediaResult[] { result } : null);
                }
            };

            // Store the VC ref.
            mPickerController = picker;

            // Now show the VC.
            using (var unityVC = UIViewController.UnityGetGLViewController())
                unityVC.PresentViewController(picker, true, null);
        }

        public void SaveImage(Texture2D image, string name, ImageFormat format = ImageFormat.JPG, Action<string> callback = null)
        {
            callback = callback == null ? null : RuntimeHelper.ToMainThread(callback);

            if (image == null)
            {
                if (callback != null)
                    callback("Can't save a null image.");

                return;
            }

            byte[] rawImage = TextureUtilities.EncodeAsByteArray(image, format);
            NSData data = NSData.DataWithBytesNoCopy(rawImage, (uint)rawImage.Length);
            UIImage uiImage = UIImage.ImageWithData(data);
            UIFunctions.UIImageWriteToSavedPhotosAlbum(uiImage, (savedImage, nsError) =>
                {
                    if (callback != null)
                        callback(nsError != null ? nsError.LocalizedDescription : null);
                });
        }

        public void LoadImage(MediaResult result, Action<string, Texture2D> callback, int maxSize = -1)
        {
            Util.NullArgumentTest(result);
            Util.NullArgumentTest(callback);

            callback = RuntimeHelper.ToMainThread(callback);

            if (string.IsNullOrEmpty(result.Uri))
            {
                callback("Invalid image URL.", null);
                return;
            }

            var uiImage = LoadImageAtPath(result.Uri, maxSize);

            if (uiImage == null)
            {
                callback("The image couldn't be loaded.", null);
                return;
            }
                
            Texture2D image = TextureUtilities.Decode(UIFunctions.UIImagePNGRepresentation(uiImage.NormalizedImage).ToBytes());
            image = Resize(image, maxSize);
            callback(null, image);
        }

#region Private Stuff

        private static UIImage LoadImageAtPath(string url, int maxSize)
        {
            try
            {
                var nsurl = NSURL.URLWithString(NSString.StringWithUTF8String(url));
                return UIImage.ImageWithContentsOfFile(nsurl.Path);
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("Exception when trying to load image from url: {0}. Exception: {1}", url ?? "null", e.ToString()));
                return null;
            }
        }

        /// <summary> Scale the loaded image down if nesscessary. </summary>
        private Texture2D Resize(Texture2D image, int maxSize)
        {
            if (maxSize < 0)
                return image;

            // No need to resize.
            if (maxSize >= image.width && maxSize >= image.height)
                return image;

            float scaleFactor = maxSize / (float)Mathf.Max(image.width, image.height);
            int newWidth = (int)(image.width * scaleFactor);
            int newHeight = (int)(image.height * scaleFactor);

            image.BilinearScale(newWidth, newHeight);
            return image;
        }

#endregion
    }
}
#endif
