using System;
using UnityEngine;

namespace MonKey.Extensions
{

    [Flags]
    public enum ColorChannel
    {
        R = 1 << 1,
        G = 1 << 2,
        B = 1 << 3,
        A = 1 << 4,
        COLORS = R | G | B,
        ALL = R | G | B | A
    }

    public static class ColorExt
    {

        public static string ToHtml(this Color c)
        {
            Color32 col = c;
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", col.r, col.g, col.b, col.a);
        }

        public static Color DarkerBrighter(this Color col, float lightToAdd)
        {
            return new Color(col.r + lightToAdd, col.g + lightToAdd, col.b + lightToAdd, col.a);
        }

        public static Color ChannelOffseted(this Color col, ColorChannel channel, float offset)
        {


            if (channel.HasFlag(ColorChannel.R))
            {
                col.r += offset;
            }

            if (channel.HasFlag(ColorChannel.G))
            {
                col.g += offset;
            }

            if (channel.HasFlag(ColorChannel.B))
            {
                col.b += offset;
            }

            if (channel.HasFlag(ColorChannel.A))
            {
                col.a += offset;
            }
            return col;
        }

        public static Color Color256(int r, int g, int b, int a = 256)
        {
            return new Color((float)r / 256, (float)g / 256, (float)b / 256, (float)a / 256);
        }

        public static Color HTMLColor(string id)
        {
            Color color;

            if (!id.StartsWith("#"))
            {
                id = id.Insert(0, "#");
            }

            ColorUtility.TryParseHtmlString(id, out color);
            return color;
        }

        public static Color Alphaed(this Color col, float alpha)
        {
            return new Color(col.r, col.g, col.b, alpha);
        }

        public static Color Step(this Color col, float step)
        {
            return new Color(col.r + step, col.g + step, col.b + step, col.a);
        }

        public static Color Inverted(this Color col, bool invertAlpha = false)
        {
            return new Color(1 - col.r, 1 - col.g, 1 + -col.b, (invertAlpha) ? 1 - col.a : col.a);
        }
    }
}

