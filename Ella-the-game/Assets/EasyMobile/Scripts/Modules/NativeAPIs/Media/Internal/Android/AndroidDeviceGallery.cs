#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class AndroidDeviceGallery : IDeviceGallery
    {
        private const string NativeClassName = "com.sglib.easymobile.androidnative.media.DeviceGallery";
        private const string NativeSaveImageName = "saveImage";
        private const string NativePickName = "pick";
        private const string NativeLoadImageContentUri = "loadImageFromContentUri";
        private const string NativeLoadImageAbsoluteUri = "loadImageFromAbsoluteUri";

        private const string NullNativeGalleryMessage = "Couldn't create Gallery native wrapper class.";
        private const string NullImageMessage = "Can't save a null image.";
        private const string NullCallbackMessage = "This method won't run because the callback is null.";

        private AndroidJavaObject nativeGallery = null;

        public AndroidDeviceGallery()
        {
            nativeGallery = new AndroidJavaObject(NativeClassName);
        }

        public void Pick(Action<string, MediaResult[]> callback)
        {
            if (callback == null)
            {
                Debug.LogError(NullCallbackMessage);
                return;
            }

            if (nativeGallery == null)
            {
                callback(NullNativeGalleryMessage, null);
                return;
            }

            nativeGallery.Call(NativePickName, true, new AndroidPickFromGalleryProxy(callback));    // true = allowMultiSelect
        }

        public void SaveImage(Texture2D image, string name, ImageFormat format = ImageFormat.JPG, Action<string> callback = null)
        {
            if (callback == null)
            {
                Debug.LogError(NullCallbackMessage);
                return;
            }

            if (image == null)
            {
                callback(NullImageMessage);
                return;
            }

            if (nativeGallery == null)
            {
                callback(NullNativeGalleryMessage);
                return;
            }

            nativeGallery.Call(NativeSaveImageName, TextureUtilities.Encode(image, format), name, (int)format, new AndroidSaveImageProxy(callback), false); // false = saveToInternal
        }

        public void LoadImage(MediaResult media, Action<string, Texture2D> callback, int maxSize = -1)
        {
            if (callback == null)
            {
                Debug.LogError(NullCallbackMessage);
                return;
            }

            if (nativeGallery == null)
            {
                callback(NullNativeGalleryMessage, null);
                return;
            }

            if (media == null || media.Type != MediaType.Image)
            {
                callback("Unvalid MediaResult.", null);
                return;
            }

            /// Both uris are null or empty.
            if (string.IsNullOrEmpty(media.Uri))
            {
                Debug.LogError("Couldn't find a valid uri.");
                return;
            }

            /// Try the absolute uri first.
            if (!string.IsNullOrEmpty(media.absoluteUri))
            {
                LoadImageFromAbsoluteUri(media.absoluteUri,
                    (error, image) =>
                    {
                        /// The image is loaded successfully with absolute uri,
                        /// so we just invoke the callback and return.
                        if (error == null && image != null)
                        {
                            callback(null, image);
                            return;
                        }

                        /// Try the content uri if it's available.
                        /// Even if this fails, the callback will be invoked with the error when loading with content uri.
                        if (!string.IsNullOrEmpty(media.contentUri))
                        {
                            LoadImageFromContentUri(media.contentUri, callback, maxSize);
                            return;
                        }

                        /// Otherwise there's no other choice, invoke the callback with error when loading with absolute uri.
                        callback(error, null);
                    },
                    maxSize);
                return;
            }

            /// If we can reach here, this mean only content uri is available, so just load it.
            LoadImageFromContentUri(media.contentUri, callback, maxSize);
        }

        internal void LoadImageFromContentUri(string uri, Action<string, Texture2D> callback, int maxSize = -1)
        {
            if (maxSize > 0)
                nativeGallery.Call(NativeLoadImageContentUri, uri, maxSize, new AndroidLoadImageProxy(callback));
            else
                nativeGallery.Call(NativeLoadImageContentUri, uri, new AndroidLoadImageProxy(callback));
        }

        internal void LoadImageFromAbsoluteUri(string uri, Action<string, Texture2D> callback, int maxSize = -1)
        {
            if (maxSize > 0)
                nativeGallery.Call(NativeLoadImageAbsoluteUri, uri, maxSize, new AndroidLoadImageProxy(callback));
            else
                nativeGallery.Call(NativeLoadImageAbsoluteUri, uri, new AndroidLoadImageProxy(callback));
        }
    }
}
#endif