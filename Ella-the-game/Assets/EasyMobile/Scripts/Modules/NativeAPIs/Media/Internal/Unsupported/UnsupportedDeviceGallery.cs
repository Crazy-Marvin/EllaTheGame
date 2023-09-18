using System;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class UnsupportedDeviceGallery : IDeviceGallery
    {
        private const string UnsupportedMessage = "Device gallery is not supported on this platform.";

        public void Pick(Action<string, MediaResult[]> callback)
        {
            Debug.LogWarning(UnsupportedMessage);
        }

        public void SaveImage(Texture2D image, string name, ImageFormat format = ImageFormat.JPG, Action<string> callback = null)
        {
            Debug.LogWarning(UnsupportedMessage);
        }

        public void LoadImage(MediaResult media, Action<string, Texture2D> callback, int maxSize = -1)
        {
            Debug.LogWarning(UnsupportedMessage);
        }
    }
}
