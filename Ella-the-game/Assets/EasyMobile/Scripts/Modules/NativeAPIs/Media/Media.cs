using UnityEngine;
using EasyMobile.Internal.NativeAPIs.Media;

namespace EasyMobile
{
    /// <summary>
    /// Entry class for device camera & gallery APIs.
    /// </summary>
    public static class Media
    {
        /// <summary>
        /// Entry interface to use native device's Camera APIs.
        /// </summary>
        public static IDeviceCamera Camera
        {
            get
            {
                if (sCameraClient == null)
                    sCameraClient = GetCamera();
                return sCameraClient;
            }
        }

        /// <summary>
        /// Entry interface to use native device's Gallery APIs.
        /// </summary>
        public static IDeviceGallery Gallery
        {
            get
            {
                if (sGalleryClient == null)
                    sGalleryClient = GetGallery();
                return sGalleryClient;
            }
        }

        private static IDeviceCamera sCameraClient;
        private static IDeviceGallery sGalleryClient;

        private static IDeviceCamera GetCamera()
        {
#if !EM_CAMERA_GALLERY
            Debug.LogError("Camera & Gallery submodule is currently disable. Please enable it to use Media.Camera API.");
            return null;
#elif UNITY_EDITOR
            return new UnsupportedDeviceCamera();
#elif UNITY_ANDROID
            return new AndroidDeviceCamera();
#elif UNITY_IOS
            return new iOSDeviceCamera();
#else
            return new UnsupportedDeviceCamera();
#endif
        }

        private static IDeviceGallery GetGallery()
        {
#if !EM_CAMERA_GALLERY
            Debug.LogError("Camera & Gallery submodule is currently disable. Please enable it to use Media.Gallery API.");
            return null;
#elif UNITY_EDITOR
            return new UnsupportedDeviceGallery();
#elif UNITY_ANDROID
            return new AndroidDeviceGallery();
#elif UNITY_IOS
            return new iOSDeviceGallery();
#else
            return new UnsupportedDeviceGallery();
#endif
        }
    }
}
