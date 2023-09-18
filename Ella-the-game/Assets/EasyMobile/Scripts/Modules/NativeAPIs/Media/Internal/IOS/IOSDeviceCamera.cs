#if UNITY_IOS && EM_CAMERA_GALLERY
using UnityEngine;
using System;
using System.Collections.Generic;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.MobileCoreServices;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class iOSDeviceCamera : IDeviceCamera
    {
        private UIImagePickerController mPickerController;

        public bool IsCameraAvailable(CameraType cameraType)
        {
            return UIImagePickerController.IsCameraDeviceAvailable(
                cameraType == CameraType.Front ? UIImagePickerController.CameraDeviceEnum.Front : UIImagePickerController.CameraDeviceEnum.Rear);
        }

        public void RecordVideo(CameraType cameraType, Action<string, MediaResult> callback)
        {
            Util.NullArgumentTest(callback);

            if (mPickerController != null)
            {
                Debug.Log("Ignoring RecordVideo call because another image picker UI is being shown.");
                return;
            }

            callback = RuntimeHelper.ToMainThread(callback);

            if (!IsCameraAvailable(cameraType))
            {
                callback(cameraType.ToString() + " camera is not supported on this device.", null);
                return;
            }

            // Create a new image picker.
            var picker = InteropObjectFactory<UIImagePickerController>.Create(
                             () => new UIImagePickerController(),
                             c => c.ToPointer());

            // Source type must be camera.
            picker.SourceType = UIImagePickerController.SourceTypeEnum.Camera;

            // Set camera type.
            picker.CameraDevice = cameraType == CameraType.Front ? UIImagePickerController.CameraDeviceEnum.Front : UIImagePickerController.CameraDeviceEnum.Rear;

            // Only allow video.
            NSMutableArray<NSString> mediaTypes = new NSMutableArray<NSString>();
            mediaTypes.AddObject(UTTypeConstants.kUTTypeMovie);
            picker.MediaTypes = mediaTypes;

            // Create a delegate for the TBM VC.
            picker.Delegate = new InternalUIImagePickerControllerDelegate(InternalUIImagePickerControllerDelegate.PickerOperation.RecordVideo)
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
                    callback(error, result);
                }
            };

            // Store the VC ref.
            mPickerController = picker;

            // Now show the VC.
            using (var unityVC = UIViewController.UnityGetGLViewController())
                unityVC.PresentViewController(picker, true, null);
        }

        public void TakePicture(CameraType cameraType, Action<string, MediaResult> callback)
        {
            Util.NullArgumentTest(callback);

            if (mPickerController != null)
            {
                Debug.Log("Ignoring TakePicture call because another image picker UI is being shown.");
                return;
            }

            callback = RuntimeHelper.ToMainThread(callback);

            if (!IsCameraAvailable(cameraType))
            {
                callback(cameraType.ToString() + " camera is not supported on this device.", null);
                return;
            }

            // Create a new image picker.
            var picker = InteropObjectFactory<UIImagePickerController>.Create(
                             () => new UIImagePickerController(),
                             c => c.ToPointer());

            // Source type must be camera.
            picker.SourceType = UIImagePickerController.SourceTypeEnum.Camera;

            // Set camera type.
            picker.CameraDevice = cameraType == CameraType.Front ? UIImagePickerController.CameraDeviceEnum.Front : UIImagePickerController.CameraDeviceEnum.Rear;

            // Only allow image.
            NSMutableArray<NSString> mediaTypes = new NSMutableArray<NSString>();
            mediaTypes.AddObject(UTTypeConstants.kUTTypeImage);
            picker.MediaTypes = mediaTypes;

            // Create a delegate for the TBM VC.
            picker.Delegate = new InternalUIImagePickerControllerDelegate(InternalUIImagePickerControllerDelegate.PickerOperation.TakePicture)
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
                    callback(error, result);
                }
            };

            // Store the VC ref.
            mPickerController = picker;

            // Now show the VC.
            using (var unityVC = UIViewController.UnityGetGLViewController())
                unityVC.PresentViewController(picker, true, null);
        }
    }
}
#endif