#if UNITY_IOS && EM_CAMERA_GALLERY
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.Internal.iOS.UIKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.UIKit
{
    internal class UIImagePickerController : UIViewController
    {
        private const string FrameworkName = "UIKit";

        /// <summary>
        /// The source to use when picking an image or when determining available media types.
        /// </summary>
        public enum SourceTypeEnum
        {
            /// <summary>
            /// Specifies the device’s photo library as the source for the image picker controller.
            /// </summary>
            PhotoLibrary = 0,
            /// <summary>
            /// Specifies the device’s built-in camera as the source for the image picker controller. 
            /// Indicate the specific camera you want (front or rear, as available) by using the cameraDevice property.
            /// </summary>
            Camera,
            /// <summary>
            /// Specifies the device’s Camera Roll album as the source for the image picker controller. 
            /// If the device does not have a camera, specifies the Saved Photos album as the source.
            /// </summary>
            SavedPhotosAlbum
        }

        /// <summary>
        /// Video quality settings for movies recorded with the built-in camera, or transcoded by displaying in the image picker.
        /// </summary>
        public enum QualityType
        {
            /// <summary>
            /// If recording, specifies that you want to use the highest-quality video recording supported for the active camera on the device.
            /// </summary>
            High = 0,
            /// <summary>
            /// If recording, specifies that you want to use VGA-quality video recording (pixel dimensions of 640x480).
            /// </summary>
            Vga640x480,
            /// <summary>
            /// If recording, specifies that you want to use medium-quality video recording.
            /// </summary>
            Medium,
            /// <summary>
            /// If recording, specifies that you want to use low-quality video recording.
            /// </summary>
            Low,
            /// <summary>
            /// If recording, specifies that you want to use 1280x720 iFrame format.
            /// </summary>
            IFrame1280x720,
            /// <summary>
            /// If recording, specifies that you want to use 960x540 iFrame format.
            /// </summary>
            IFrame960x540
        }

        /// <summary>
        /// The camera to use for image or movie capture.
        /// </summary>
        public enum CameraDeviceEnum
        {
            /// <summary>
            /// Specifies the camera on the rear of the device.
            /// </summary>
            Rear = 0,
            /// <summary>
            /// Specifies the camera on the front of the device.
            /// </summary>
            Front
        }

        /// <summary>
        /// The category of media for the camera to capture.
        /// </summary>
        public enum CameraCaptureModeEnum
        {
            /// <summary>
            /// Specifies that the camera captures still images.
            /// </summary>
            Photo = 0,
            /// <summary>
            /// Specifies that the camera captures movies.
            /// </summary>
            Video
        }

        /// <summary>
        /// The flash mode to use with the active camera.
        /// </summary>
        public enum CameraFlashModeEnum
        {
            /// <summary>
            /// Specifies that flash illumination is always off, no matter what the ambient light conditions are.
            /// </summary>
            Off = 0,
            /// <summary>
            /// Specifies that the device should consider ambient light conditions to automatically determine whether or not to use flash illumination.
            /// </summary>
            Auto,
            /// <summary>
            /// Specifies that flash illumination is always on, no matter what the ambient light conditions are.
            /// </summary>
            On
        }

        /// <summary>
        /// Constants indicating how to export images to the client app.
        /// </summary>
        public enum ImageURLExportPreset
        {
            /// <summary>
            /// A preset for converting HEIF formatted images to JPEG.
            /// </summary>
            Compatible = 0,
            /// <summary>
            /// A preset for passing image data as-is to the client.
            /// </summary>
            Current
        }

        #region UIImagePickerControllerInfoKey

        /* *
         * There's no need to cache these constants as LookupStringConstant will do that itself.
         * */

        /// <summary>
        /// Specifies the cropping rectangle that was applied to the original image.
        /// </summary>
        public static NSString UIImagePickerControllerCropRect
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerCropRect, FrameworkName);
            }
        }

        /// <summary>
        /// Specifies an image edited by the user.
        /// </summary>
        public static NSString UIImagePickerControllerEditedImage
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerEditedImage, FrameworkName);
            }
        }

        /// <summary>
        /// A key containing the URL of the image file.
        /// </summary>
        public static NSString UIImagePickerControllerImageURL
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerImageURL, FrameworkName);
            }
        }

        /// <summary>
        /// The Live Photo representation of the selected or captured photo.
        /// </summary>
        public static NSString UIImagePickerControllerLivePhoto
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerLivePhoto, FrameworkName);
            }
        }

        /// <summary>
        /// Metadata for a newly-captured photograph.
        /// </summary>
        public static NSString UIImagePickerControllerMediaMetadata
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerMediaMetadata, FrameworkName);
            }
        }

        /// <summary>
        /// Specifies the media type selected by the user.
        /// </summary>
        public static NSString UIImagePickerControllerMediaType
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerMediaType, FrameworkName);
            }
        }

        /// <summary>
        /// Specifies the filesystem URL for the movie.
        /// </summary>
        public static NSString UIImagePickerControllerMediaURL
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerMediaURL, FrameworkName);
            }
        }

        /// <summary>
        /// Specifies the original, uncropped image selected by the user.
        /// </summary>
        public static NSString UIImagePickerControllerOriginalImage
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerOriginalImage, FrameworkName);
            }
        }

        /// <summary>
        /// The key to use when retrieving a Photos asset for the image.
        /// </summary>
        public static NSString UIImagePickerControllerPHAsset
        {
            get
            {
                return iOSInteropUtil.LookupStringConstant(
                    () => UIImagePickerController.UIImagePickerControllerPHAsset, FrameworkName);
            }
        }

        #endregion

        #region UIImagePickerControllerDelegate

        /// <summary>
        /// A set of methods that your delegate object must implement to interact with the image picker interface.
        /// </summary>
        public interface UIImagePickerControllerDelegate
        {
            /// <summary>
            /// Tells the delegate that the user picked a still image or movie.
            /// </summary>
            /// <param name="picker">Picker.</param>
            /// <param name="info">Info.</param>
            void ImagePickerControllerDidFinishPickingMediaWithInfo(UIImagePickerController picker, NSDictionary<NSString, iOSObjectProxy> info);

            /// <summary>
            /// Tells the delegate that the user cancelled the pick operation.
            /// </summary>
            /// <param name="picker">Picker.</param>
            void ImagePickerControllerDidCancel(UIImagePickerController picker);
        }

        #endregion

        //private DelegateForwarder mDelegateForwarder;
        private UIImagePickerControllerDelegate mDelegate;
        private UIImagePickerControllerDelegateForwarder mDelegateForwarder;

        internal UIImagePickerController(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        internal UIImagePickerController()
            : base(C.UIImagePickerController_new())
        {
            // We're using a pointer returned by a native constructor: must call CFRelease().
            CoreFoundation.CFFunctions.CFRelease(this.ToPointer());
        }

        /// <summary>
        /// The image picker’s delegate object.
        /// </summary>
        /// <value>The delegate.</value>
        public UIImagePickerControllerDelegate Delegate
        {
            get
            {
                return mDelegate;
            }
            set
            {
                mDelegate = value;

                if (mDelegate == null)
                {
                    // Nil out the native delegate.
                    mDelegateForwarder = null;
                    C.UIImagePickerController_setDelegate(SelfPtr(), IntPtr.Zero);
                }
                else
                {
                    // Create a delegate forwarder if needed.
                    if (mDelegateForwarder == null)
                    {
                        mDelegateForwarder = InteropObjectFactory<UIImagePickerControllerDelegateForwarder>.Create(
                            () => new UIImagePickerControllerDelegateForwarder(),
                            fwd => fwd.ToPointer());

                        // Assign on native side.
                        C.UIImagePickerController_setDelegate(SelfPtr(), mDelegateForwarder.ToPointer());
                    }

                    // Set delegate.
                    mDelegateForwarder.Listener = mDelegate;
                }
            }
        }

        /// <summary>
        /// Returns an array of the available media types for the specified source type.
        /// </summary>
        /// <returns>An array of the available media types for the specified source type.</returns>
        /// <param name="sourceType">The source to use to pick an image.</param>
        public static NSArray<NSString> AvailableMediaTypesForSourceType(SourceTypeEnum sourceType)
        {
            var ptr = C.UIImagePickerController_availableMediaTypesForSourceType(sourceType);
            var nsArray = InteropObjectFactory<NSArray<NSString>>.FromPointer(ptr, p => new NSArray<NSString>(p));
            CoreFoundation.CFFunctions.CFRelease(ptr);
            return nsArray;
        }

        /// <summary>
        /// Returns a Boolean value indicating whether the device supports picking media using the specified source type.
        /// </summary>
        /// <returns><c>true</c>, if source type available was ised, <c>false</c> otherwise.</returns>
        /// <param name="sourceType">Source type.</param>
        public static bool IsSourceTypeAvailable(SourceTypeEnum sourceType)
        {
            return C.UIImagePickerController_isSourceTypeAvailable(sourceType);
        }

        /// <summary>
        /// The type of picker interface to be displayed by the controller.
        /// </summary>
        /// <value>The source type.</value>
        public SourceTypeEnum SourceType
        {
            get
            {
                return C.UIImagePickerController_sourceType(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setSourceType(SelfPtr(), value);
            }
        }

        /// <summary>
        /// An array indicating the media types to be accessed by the media picker controller.
        /// </summary>
        /// <value>The  media types array.</value>
        public NSArray<NSString> MediaTypes
        {
            get
            {
                var ptr = C.UIImagePickerController_mediaTypes(SelfPtr());
                var nsArray = InteropObjectFactory<NSArray<NSString>>.FromPointer(ptr, p => new NSArray<NSString>(p));
                CoreFoundation.CFFunctions.CFRelease(ptr);
                return nsArray;
            }
            set
            {
                C.UIImagePickerController_setMediaTypes(SelfPtr(), value != null ? value.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// A Boolean value indicating whether the user is allowed to edit a selected still image or movie.
        /// </summary>
        /// <value><c>true</c>, if editing is allowed.<c>false</c> otherwise.</value>
        public bool AllowsEditing
        {
            get
            {
                return C.UIImagePickerController_allowsEditing(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setAllowsEditing(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The video recording and transcoding quality.
        /// </summary>
        /// <value>The video quality</value>
        public QualityType VideoQuality
        {
            get
            {
                return C.UIImagePickerController_videoQuality(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setVideoQuality(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The maximum duration, in seconds, for a video recording.
        /// </summary>
        /// <value>The maximum duration</value>
        public Int64 VideoMaximumDuration
        {
            get
            {
                return C.UIImagePickerController_videoMaximumDuration(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setVideoMaximumDuration(SelfPtr(), value);
            }
        }

        /// <summary>
        /// Indicates whether the image picker displays the default camera controls.
        /// </summary>
        /// <value><c>true</c>, if the image picker displays the default camera controls.<c>false</c> otherwise.</value>
        public bool ShowsCameraControls
        {
            get
            {
                return C.UIImagePickerController_showsCameraControls(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setShowsCameraControls(SelfPtr(), value);
            }
        }

        /// <summary>
        /// Captures a still image using the camera.
        /// </summary>
        public void TakePicture()
        {
            C.UIImagePickerController_takePicture(SelfPtr());
        }

        /// <summary>
        /// Starts video capture using the camera specified by the  <see cref="CameraDeviceEnum"/>  property.
        /// </summary>
        public void StartVideoCapture()
        {
            C.UIImagePickerController_startVideoCapture(SelfPtr());
        }

        /// <summary>
        /// Stops video capture.
        /// </summary>
        public void StopVideoCapture()
        {
            C.UIImagePickerController_stopVideoCapture(SelfPtr());
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether a given camera is available.
        /// </summary>
        /// <returns><c>true</c>, if the given camera is available, <c>false</c> otherwise.</returns>
        /// <param name="cameraDevice">Camera Device.</param>
        public static bool IsCameraDeviceAvailable(CameraDeviceEnum cameraDevice)
        {
            return C.UIImagePickerController_isCameraDeviceAvailable(cameraDevice);
        }

        /// <summary>
        /// The camera used by the image picker controller.
        /// </summary>
        /// <value>The camera device</value>
        public CameraDeviceEnum CameraDevice
        {
            get
            {
                return C.UIImagePickerController_cameraDevice(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setCameraDevice(SelfPtr(), value);
            }
        }

        /// <summary>
        /// Returns an array of NSNumber objects indicating the capture modes supported by a given camera device.
        /// </summary>
        /// <returns>An array of the capture modes.</returns>
        /// <param name="cameraDevice">Camera Device.</param>
        public static NSArray<NSString> AvailableCaptureModesForCameraDevice(CameraDeviceEnum cameraDevice)
        {
            var ptr = C.UIImagePickerController_availableCaptureModesForCameraDevice(cameraDevice);
            var nsArray = InteropObjectFactory<NSArray<NSString>>.FromPointer(ptr, p => new NSArray<NSString>(p));
            CoreFoundation.CFFunctions.CFRelease(ptr);
            return nsArray;
        }

        /// <summary>
        /// The capture mode used by the camera.
        /// </summary>
        /// <value>The camera capture mode</value>
        public CameraCaptureModeEnum CameraCaptureMode
        {
            get
            {
                return C.UIImagePickerController_cameraCaptureMode(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setCameraCaptureMode(SelfPtr(), value);
            }
        }

        /// <summary>
        /// Indicates whether a given camera has flash illumination capability.
        /// </summary>
        /// <returns><c>true</c>, if the camera has flash illumination capability. <c>false</c> otherwise.</returns>
        /// <param name="cameraDevice">Camera Device.</param>
        public static bool IsFlashAvailableForCameraDevice(CameraDeviceEnum cameraDevice)
        {
            return C.UIImagePickerController_isFlashAvailableForCameraDevice(cameraDevice);
        }

        /// <summary>
        /// The flash mode used by the active camera.
        /// </summary>
        /// <value>The camera flash mode</value>
        public CameraFlashModeEnum CameraFlashMode
        {
            get
            {
                return C.UIImagePickerController_cameraFlashMode(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setCameraFlashMode(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The preset to use when preparing images for export to your app.
        /// </summary>
        /// <value>The image export preset.</value>
        public ImageURLExportPreset ImageExportPreset
        {
            get
            {
                return C.UIImagePickerController_imageExportPreset(SelfPtr());
            }
            set
            {
                C.UIImagePickerController_setImageExportPreset(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The preset to use when preparing video for export to your app.
        /// </summary>
        /// <value>The video export preset.</value>
        public NSString VideoExportPreset
        {
            get
            {
                var ptr = C.UIImagePickerController_videoExportPreset(SelfPtr());
                var preset = new NSString(ptr);
                CoreFoundation.CFFunctions.CFRelease(ptr);
                return preset;
            }
            set
            {
                C.UIImagePickerController_setVideoExportPreset(SelfPtr(), value != null ? value.ToPointer() : IntPtr.Zero);
            }
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* UIImagePickerController */IntPtr UIImagePickerController_new();

            [DllImport("__Internal")]
            internal static extern /* InteropUIImagePickerControllerDelegate */ IntPtr UIImagePickerController_delegate(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setDelegate(HandleRef selfPtr, /* InteropUIImagePickerControllerDelegate */IntPtr delegatePtr);

            [DllImport("__Internal")]
            internal static extern /* InteropNSArray<InteropNSString> */ IntPtr UIImagePickerController_availableMediaTypesForSourceType(SourceTypeEnum sourceType);

            [DllImport("__Internal")]
            internal static extern bool UIImagePickerController_isSourceTypeAvailable(SourceTypeEnum sourceType);

            [DllImport("__Internal")]
            internal static extern SourceTypeEnum UIImagePickerController_sourceType(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setSourceType(
            /* InteropUIImagePickerController */ HandleRef selfPtr, SourceTypeEnum sourceType);

            [DllImport("__Internal")]
            internal static extern /* InteropNSArray<InteropNSString> */ IntPtr UIImagePickerController_mediaTypes(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setMediaTypes(
            /* InteropUIImagePickerController */ HandleRef selfPtr, /* NSArray<NSString> */IntPtr mediaTypes);

            [DllImport("__Internal")]
            internal static extern bool UIImagePickerController_allowsEditing(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setAllowsEditing(
            /* InteropUIImagePickerController */ HandleRef selfPtr, bool allowsEditing);

            [DllImport("__Internal")]
            internal static extern QualityType UIImagePickerController_videoQuality(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setVideoQuality(
            /* InteropUIImagePickerController */ HandleRef selfPtr, QualityType videoQuality);

            [DllImport("__Internal")]
            internal static extern long UIImagePickerController_videoMaximumDuration(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setVideoMaximumDuration(
            /* InteropUIImagePickerController */ HandleRef selfPtr, long videoMaximumDuration);

            [DllImport("__Internal")]
            internal static extern bool UIImagePickerController_showsCameraControls(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setShowsCameraControls(
            /* InteropUIImagePickerController */ HandleRef selfPtr, bool showsCameraControls);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_takePicture(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern bool UIImagePickerController_startVideoCapture(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_stopVideoCapture(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern bool UIImagePickerController_isCameraDeviceAvailable(
            /* InteropUIImagePickerController */ CameraDeviceEnum cameraDevice);

            [DllImport("__Internal")]
            internal static extern CameraDeviceEnum UIImagePickerController_cameraDevice(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setCameraDevice(
            /* InteropUIImagePickerController */ HandleRef selfPtr, CameraDeviceEnum cameraDevice);

            [DllImport("__Internal")]
            internal static extern /* InteropNSArray<InteropNSNumber> */ IntPtr UIImagePickerController_availableCaptureModesForCameraDevice(
            /* InteropUIImagePickerController */ CameraDeviceEnum cameraDevice);

            [DllImport("__Internal")]
            internal static extern CameraCaptureModeEnum UIImagePickerController_cameraCaptureMode(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setCameraCaptureMode(
            /* InteropUIImagePickerController */ HandleRef selfPtr, CameraCaptureModeEnum cameraCaptureMode);

            [DllImport("__Internal")]
            internal static extern bool UIImagePickerController_isFlashAvailableForCameraDevice(
            /* InteropUIImagePickerController */ CameraDeviceEnum cameraDevice);

            [DllImport("__Internal")]
            internal static extern CameraFlashModeEnum UIImagePickerController_cameraFlashMode(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setCameraFlashMode(
            /* InteropUIImagePickerController */ HandleRef selfPtr, CameraFlashModeEnum cameraFlashMode);

            [DllImport("__Internal")]
            internal static extern ImageURLExportPreset UIImagePickerController_imageExportPreset(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setImageExportPreset(
            /* InteropUIImagePickerController */ HandleRef selfPtr, ImageURLExportPreset imageExportPreset);

            [DllImport("__Internal")]
            internal static extern /* NSString */ IntPtr UIImagePickerController_videoExportPreset(
            /* InteropUIImagePickerController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIImagePickerController_setVideoExportPreset(
            /* InteropUIImagePickerController */ HandleRef selfPtr, /* NSString */IntPtr videoExportPreset);
        }

        #endregion
    }
}
#endif
