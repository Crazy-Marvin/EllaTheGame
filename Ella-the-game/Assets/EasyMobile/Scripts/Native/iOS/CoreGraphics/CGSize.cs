#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.iOS.CoreGraphics
{
    /// <summary>
    /// A structure that contains width and height values.
    /// </summary>
    internal struct CGSize
    {
        public float height;
        public float width;

        public CGSize(float width, float height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
#endif