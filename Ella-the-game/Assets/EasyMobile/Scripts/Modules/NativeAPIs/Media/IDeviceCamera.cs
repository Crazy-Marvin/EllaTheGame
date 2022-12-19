using System;
using System.Collections.Generic;

namespace EasyMobile
{
    /// <summary>
    /// Entry interface to use native device's Camera APIs.
    /// </summary>
    public interface IDeviceCamera
    {
        /// <summary>
        /// Determines whether the camera of the specified type is available on the current device.
        /// </summary>
        /// <returns><c>true</c> if available; otherwise, <c>false</c>.</returns>
        /// <param name="cameraType">Camera type.</param>
        bool IsCameraAvailable(CameraType cameraType);

        /// <summary>
        /// Open device's camera to take a picture.
        /// </summary>
        /// <param name="cameraType">Camera type.</param>
        /// <param name="callback">Callback.</param>
        void TakePicture(CameraType cameraType, Action<string, MediaResult> callback);

        /// <summary>
        /// Open device's camera to record video.
        /// </summary>
        /// <param name="cameraType">Camera type.</param>
        /// <param name="callback">Callback.</param>
        void RecordVideo(CameraType cameraType, Action<string, MediaResult> callback);
    }
}
