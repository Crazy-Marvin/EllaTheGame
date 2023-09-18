using System;
using UnityEngine;

namespace EasyMobile
{
    /// <summary>
    /// Entry interface to use native device's Gallery APIs.
    /// </summary>
    public interface IDeviceGallery
    {
        /// <summary>
        /// Pick item(s) from gallery.
        /// </summary>
        /// <param name="callback">
        /// Callback called when user finish picking and return to Unity.
        /// Param 1: Error, null means success.
        /// Param 2: All the picked items, null if there's an error.
        /// </param>
        void Pick(Action<string, MediaResult[]> callback);

        /// <summary>
        /// Save an image into gallery.
        /// </summary>
        /// <param name="image">The image you want to save.</param>
        /// <param name="format">The image will be saved in this format.</param>
        /// <param name="name">Image will be saved with this name.</param>
        /// <param name="callback">
        /// Callback called after the image is saved. Param: Error, null means success.
        /// </param>
        void SaveImage(Texture2D image, string name, ImageFormat format = ImageFormat.JPG, Action<string> callback = null);

        /// <summary>
        /// Load image from <see cref="MediaResult"/>.
        /// </summary>
        /// <param name="media">
        /// Target result, note that this method only work if the <see cref="MediaResult.Type"/> equals <see cref="MediaType.Image"/>.
        /// </param>
        /// <param name="callback">
        /// Callback called when the image is loaded.
        /// Param 1: Error, null means the image has been loaded successfully.
        /// Param 2: Loaded image, null if there's error.
        /// </param>
        /// <param name="maxSize">Maximum size of the image. Load fullsize if non-positive.</param>
        void LoadImage(MediaResult media, Action<string, Texture2D> callback, int maxSize = -1);
    }
}
