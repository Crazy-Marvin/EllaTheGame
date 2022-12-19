#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.iOS.UIKit;

namespace EasyMobile.Internal.GameServices.iOS
{
    internal static partial class GameKitTypeConverter
    {
        public static Texture2D ToTexture2D(this UIImage image)
        {
            byte[] pngData = UIImage.UIImagePNGRepresentation(image);

            if (pngData == null || pngData.Length == 0)
                return null;

            var tex = new Texture2D(4, 4);
            tex.LoadImage(pngData);
            return tex;
        }

    }
}
#endif