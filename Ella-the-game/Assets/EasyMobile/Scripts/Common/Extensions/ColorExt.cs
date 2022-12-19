using UnityEngine;
using System.Collections;

namespace EasyMobile.Internal
{
    public static class ColorExt
    {
        public static string ToRGBAHexString(this Color32 c)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
        }

        public static string ToRGBAHexString(this Color color)
        {
            Color32 c = color;
            return c.ToRGBAHexString();
        }

        public static string ToARGBHexString(this Color32 c)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.a, c.r, c.g, c.b);
        }

        public static string ToARGBHexString(this Color color)
        {
            Color32 c = color;
            return c.ToARGBHexString();
        }

        public static int ToRGBAHex(this Color32 c)
        {
            return (c.r << 24) | (c.g << 16) | (c.b << 8) | (c.a);
        }

        public static int ToRGBAHex(this Color color)
        {
            Color32 c = color;
            return c.ToRGBAHex();
        }

        public static int ToARGBHex(this Color32 c)
        {
            return (c.a << 24) | (c.r << 16) | (c.g << 8) | (c.b);
        }

        public static int ToARGBHex(this Color color)
        {
            Color32 c = color;
            return c.ToARGBHex();
        }
    }
}