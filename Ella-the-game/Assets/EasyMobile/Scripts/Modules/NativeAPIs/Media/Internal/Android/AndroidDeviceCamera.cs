#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class AndroidDeviceCamera : IDeviceCamera
    {
        private const string NativeClassName = "com.sglib.easymobile.androidnative.media.DeviceCamera";
        private const string NativeIsCameraAvailableName = "isCameraAvailable";
        private const string NativeTakePicktureName = "takePicture";
        private const string NativeRecordVideoName = "recordVideo";
        private const string CantCreateNativeCameraMessage = "The native camera object coundn't be initialized.";
        private const string CameraUnsupportedDeviceMessage = " camera is not supported on this device.";

        private AndroidJavaObject NativeCamera = null;

        public AndroidDeviceCamera()
        {
            NativeCamera = new AndroidJavaObject(NativeClassName);
        }

        public bool IsCameraAvailable(CameraType cameraType)
        {
            if (NativeCamera == null)
            {
                Debug.LogError(CantCreateNativeCameraMessage);
                return false;
            }

            return NativeCamera.Call<bool>(NativeIsCameraAvailableName, (int)cameraType);
        }

        public void TakePicture(CameraType cameraType, Action<string, MediaResult> callback)
        {
            if (NativeCamera == null)
            {
                if (callback != null)
                    callback(CantCreateNativeCameraMessage, null);

                return;
            }

            if (!IsCameraAvailable(cameraType))
            {
                if (callback != null)
                    callback(cameraType + CameraUnsupportedDeviceMessage, null);

                return;
            }

            NativeCamera.Call(NativeTakePicktureName, (int)cameraType, new AndroidMediaCollectedProxy(callback));
        }

        public void RecordVideo(CameraType cameraType, Action<string, MediaResult> callback)
        {
            if (NativeCamera == null)
            {
                if (callback != null)
                    callback(CantCreateNativeCameraMessage, null);

                return;
            }

            if (!IsCameraAvailable(cameraType))
            {
                if (callback != null)
                    callback(cameraType + CameraUnsupportedDeviceMessage, null);

                return;
            }

            NativeCamera.Call(NativeRecordVideoName, (int)cameraType, new AndroidMediaCollectedProxy(callback));
        }
    }
}
#endif