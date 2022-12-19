using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EasyMobile.Internal.Sharing
{
    internal class UnsupportedSharingClient : ISharingClient
    {
        private const string UNSUPPORTED_MESSAGE = "Sharing is not supported on this platform. Please test on an iOS or Android device.";

        public void ShareImage(string imagePath, string message, string subject = "")
        {
            Debug.Log(UNSUPPORTED_MESSAGE);
        }

        public void ShareText(string text, string subject = "")
        {
            Debug.Log(UNSUPPORTED_MESSAGE);
        }

        public void ShareURL(string url, string subject = "")
        {
            Debug.Log(UNSUPPORTED_MESSAGE);
        }
    }
}
