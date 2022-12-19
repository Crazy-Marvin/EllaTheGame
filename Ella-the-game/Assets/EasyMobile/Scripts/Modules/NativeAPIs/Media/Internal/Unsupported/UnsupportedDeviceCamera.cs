using System;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class UnsupportedDeviceCamera : IDeviceCamera
    {
        private const string UnsupportedMessage = "Device camera is not supported on this platform.";

        public bool IsCameraAvailable(CameraType cameraType)
        {
            return false;
        }

        public void TakePicture(CameraType cameraType, Action<string, MediaResult> callback)
        {
            Debug.LogWarning(UnsupportedMessage);
        }

        public void RecordVideo(CameraType cameraType, Action<string, MediaResult> callback)
        {
            Debug.LogWarning(UnsupportedMessage);
        }
    }
}
