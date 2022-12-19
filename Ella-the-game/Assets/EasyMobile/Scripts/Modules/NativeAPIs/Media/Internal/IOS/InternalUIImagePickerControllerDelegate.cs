#if UNITY_IOS && EM_CAMERA_GALLERY
using UnityEngine;
using System;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.UIKit;
using UIImagePickerControllerDelegate = EasyMobile.iOS.UIKit.UIImagePickerController.UIImagePickerControllerDelegate;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    // Matchmaker VC delegate.
    internal class InternalUIImagePickerControllerDelegate : UIImagePickerControllerDelegate
    {
        public enum PickerOperation
        {
            PickGallery,
            TakePicture,
            RecordVideo
        }

        public PickerOperation Operation { get; private set; }

        public Action CloseAndResetMatchmakerVC { get; set; }

        public Action<string, MediaResult> CompleteCallback { get; set; }

        internal InternalUIImagePickerControllerDelegate(PickerOperation op)
        {
            Operation = op;
        }

#region UIImagePickerControllerDelegate implementation

        public void ImagePickerControllerDidFinishPickingMediaWithInfo(UIImagePickerController picker, NSDictionary<NSString, iOSObjectProxy> info)
        {
            if (CloseAndResetMatchmakerVC != null)
                CloseAndResetMatchmakerVC();

            MediaResult result = CreateResult(Operation, info);
            string error = result == null ? "Couldn't get url of the selected item." : null;

            if (CompleteCallback != null)
                CompleteCallback(error, result);
        }

        public void ImagePickerControllerDidCancel(UIImagePickerController picker)
        {
            if (CloseAndResetMatchmakerVC != null)
                CloseAndResetMatchmakerVC();
        }

        private MediaResult CreateResult(PickerOperation operation, NSDictionary<NSString, iOSObjectProxy> info)
        {
            if (info == null)
                return null;

            switch (Operation)
            {
                case PickerOperation.PickGallery:
                    // User can either pick an image or a video
                    NSURL pickedURL = null;
                    if (TryGetUrl(info, UIImagePickerController.UIImagePickerControllerImageURL, out pickedURL))
                        return new MediaResult(MediaType.Image, null, pickedURL.AbsoluteString.UTF8String);
                    else if (TryGetUrl(info, UIImagePickerController.UIImagePickerControllerMediaURL, out pickedURL))
                        return new MediaResult(MediaType.Video, null, pickedURL.AbsoluteString.UTF8String);
                    else
                        return null;
                    
                case PickerOperation.TakePicture:
                    // Get the newly taken image, save it into the into user temporary folder and return the URL.
                    // The image name will be "IMG_" + timestamp and format is JPG. 
                    UIImage picture = (UIImage)info.ValueForKey(UIImagePickerController.UIImagePickerControllerOriginalImage,
                                          ptr => PInvokeUtil.IsNull(ptr) ? null : new UIImage(ptr));

                    if (picture == null)
                    {
                        Debug.LogError("Couldn't get the taken picture.");
                        return null;
                    }

                    // Save the image into user temporary folder and return the URL.
                    NSString tempDir = NSFileManager.NSTemporaryDirectory();

                    if (tempDir == null)
                    {
                        Debug.LogError("Couldn't find the path of user's temporary directory.");
                        return null;
                    }

                    // The image name is "IMG_" + timestamp and format is JPG.
                    NSString pictureName = NSString.StringWithUTF8String("IMG_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                    NSURL pictureURL = NSURL.FileURLWithPath(tempDir).URLByAppendingPathComponent(pictureName);

                    if (pictureURL != null)
                    {
                        NSData imageData = UIFunctions.UIImageJPEGRepresentation(picture, 1f);
                        NSFileManager.DefaultManager.CreateFileAtPath(pictureURL.Path, imageData, null);
                        return new MediaResult(MediaType.Image, null, pictureURL.AbsoluteString.UTF8String);
                    }
                    else
                    {
                        return null;
                    }

                case PickerOperation.RecordVideo:
                    NSURL videoURL = null;
                    if (TryGetUrl(info, UIImagePickerController.UIImagePickerControllerMediaURL, out videoURL))
                        return new MediaResult(MediaType.Video, null, videoURL.AbsoluteString.UTF8String);
                    else
                        return null;
                    
                default:
                    return null;
            }
        }

        private bool TryGetUrl(NSDictionary<NSString, iOSObjectProxy> info, NSString key, out NSURL url)
        {
            try
            {
                NSURL nsurl = (NSURL)info.ValueForKey(key, ptr =>
                    {
                        if (ptr.Equals(IntPtr.Zero))
                            return null;

                        return new NSURL(ptr);
                    });

                url = nsurl;
                return nsurl != null;
            }
            catch (Exception)
            {
                url = null;
                return false;
            }
        }

#endregion
    }
}
#endif // UNITY_IOS
