using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    public struct GiphyUploadParams
    {
        /// <summary>
        /// The local image filepath, required if no sourceImageUrl supplied.
        /// If both localImagePath and sourceImageUrl are supplied, the local file
        /// will be used over the sourceImageUrl.
        /// </summary>
        public string localImagePath;
        /// <summary>
        /// The URL for the image to be uploaded, required if no localImagePath specified.
        /// If both localImagePath and sourceImageUrl are supplied, the local file
        /// will be used over the sourceImageUrl.
        /// </summary>
        public string sourceImageUrl;
        /// <summary>
        /// [Optional] Comma-delimited list of tags.
        /// </summary>
        public string tags;
        /// <summary>
        /// [Optional] The source of the asset.
        /// </summary>
        public string sourcePostUrl;
        /// <summary>
        /// [Optional] If set to true, the uploaded image will be marked as private (only visible to the uploader).
        /// </summary>
        public bool isHidden;
    }
}
