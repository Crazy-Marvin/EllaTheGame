using System;
using UnityEngine;
using EasyMobile.Internal;
using EasyMobile.Internal.NativeAPIs.Media;

namespace EasyMobile
{
    [Serializable]
    public class MediaResult
    {
        public MediaType Type { get; private set; }

        public string Uri
        {
            get
            {
                if (!string.IsNullOrEmpty(absoluteUri))
                    return absoluteUri;

                if (!string.IsNullOrEmpty(contentUri))
                    return contentUri;

                return null;
            }
        }

        /// <summary>
        /// Path to the content uri.
        /// </summary>
        internal string contentUri;

        /// <summary>
        /// Path to the absolute uri.
        /// </summary>
        internal string absoluteUri;

        /// Used to cached image loaded with <see cref="LoadImage(Action{string, Texture2D}, int)"/>.
        private int loadedImageSize = 0;
        private Texture2D loadedImage = null;

        internal MediaResult(MediaType type, string contentUri, string absoluteUri)
        {
            Type = type;
            this.contentUri = contentUri;
            this.absoluteUri = absoluteUri;
        }

        /// <summary>
        /// Load image from this <see cref="MediaResult"/>,
        /// note that this method only works if this <see cref="Type"/> equals <see cref="MediaType.Image"/>.
        /// </summary>
        /// <param name="callback">
        /// Callback raised when the image is loaded.
        /// Param 1: Error, null means the image has been loaded successfully.
        /// Param 2: Loaded image, null if there's error.
        /// </param>
        /// <param name="maxSize">Maximum size of the image. Load fullsize if non-positive.</param>
        public void LoadImage(Action<string, Texture2D> callback, int maxSize = -1)
        {
            if (callback == null)
                return;

            /// Invoke the callback if an image with same size has been loaded successfully.
            if (loadedImage != null && (maxSize <= 0 || maxSize == loadedImageSize))
            {
                callback(null, loadedImage);
                return;
            }

            Action<string, Texture2D> newCallback = (error, image) =>
            {
                /// Cached the loaded image to reuse it later.
                loadedImageSize = maxSize;
                loadedImage = image;

                callback(error, image);
            };

            Media.Gallery.LoadImage(this, newCallback, maxSize);
        }

        public override string ToString()
        {
            return string.Format("MediaResult[Type={0}, Uri={1}]",
                Type, Uri ?? "null");
        }
    }
}
