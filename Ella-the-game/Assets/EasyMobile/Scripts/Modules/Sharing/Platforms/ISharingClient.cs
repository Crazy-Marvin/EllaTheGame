using System;

namespace EasyMobile.Internal.Sharing
{
    internal interface ISharingClient
    {
        /// <summary>
        /// Shares a text.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="sharePlatform">Share platform.</param>
        void ShareText(string text, string subject = "");

        /// <summary>
        /// Shares a URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="sharePlatform">Share platform.</param>
        void ShareURL(string url, string subject = "");

        /// <summary>
        /// Shares an image.
        /// </summary>
        /// <param name="imagePath">Image path.</param>
        /// <param name="message">Message.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="sharePlatform">Share platform.</param>
        void ShareImage(string imagePath, string message, string subject = "");
    }
}
