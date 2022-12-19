using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile.Internal
{
    internal static class TextureUtilities
    {
        #region Encode & Decode

        /// <summary>
        /// Decode base64 <see cref="string"/> into <see cref="Texture2D"/>.
        /// </summary>
        public static Texture2D Decode(string encodedImage)
        {
            if (encodedImage == null)
                return null;

            byte[] bytes = Convert.FromBase64String(encodedImage);
            return Decode(bytes);
        }

        /// <summary>
        /// Decode byte array into <see cref="Texture2D"/>.
        /// </summary>
        public static Texture2D Decode(byte[] bytes)
        {
            if (bytes == null)
                return null;

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            texture.LoadImage(bytes);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Encode from <see cref="Texture2D"/> into base64 <see cref="string"/>.
        /// </summary>
        public static string Encode(Texture2D texture, ImageFormat format = ImageFormat.PNG)
        {
            byte[] bytes = EncodeAsByteArray(texture, format);
            return bytes != null ? Convert.ToBase64String(bytes) : null;
        }

        /// <summary>
        /// Encode <see cref="Texture2D"/> into byte array.
        /// </summary>
        public static byte[] EncodeAsByteArray(Texture2D texture, ImageFormat format = ImageFormat.PNG)
        {
            if (texture == null)
                return null;

            return format == ImageFormat.JPG ? texture.EncodeToJPG() : texture.EncodeToPNG();
        }

        #endregion

        #region Scale

        public class ThreadData
        {
            public int start;
            public int end;
            public ThreadData(int s, int e)
            {
                start = s;
                end = e;
            }
        }

        private static Color[] texColors;
        private static Color[] newColors;
        private static int oldWidth;
        private static float ratioX;
        private static float ratioY;
        private static int newWidth;
        private static int finishCount;
        private static Mutex mutex;

        public static void PointScale(this Texture2D texture, int newWidth, int newHeight)
        {
            ThreadedScale(texture, newWidth, newHeight, false);
        }

        public static void BilinearScale(this Texture2D texture, int newWidth, int newHeight)
        {
            ThreadedScale(texture, newWidth, newHeight, true);
        }

        private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
        {
            texColors = tex.GetPixels();
            newColors = new Color[newWidth * newHeight];
            if (useBilinear)
            {
                ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
            }
            else
            {
                ratioX = ((float)tex.width) / newWidth;
                ratioY = ((float)tex.height) / newHeight;
            }
            oldWidth = tex.width;
            TextureUtilities.newWidth = newWidth;
            var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
            var slice = newHeight / cores;

            finishCount = 0;
            if (mutex == null)
            {
                mutex = new Mutex(false);
            }
            if (cores > 1)
            {
                int i = 0;
                ThreadData threadData;
                for (i = 0; i < cores - 1; i++)
                {
                    threadData = new ThreadData(slice * i, slice * (i + 1));
                    ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                    Thread thread = new Thread(ts);
                    thread.Start(threadData);
                }
                threadData = new ThreadData(slice * i, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
                while (finishCount < cores)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                ThreadData threadData = new ThreadData(0, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
            }

            tex.Reinitialize(newWidth, newHeight);
            tex.SetPixels(newColors);
            tex.Apply();

            texColors = null;
            newColors = null;
        }

        private static void BilinearScale(object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                int yFloor = (int)Mathf.Floor(y * ratioY);
                var y1 = yFloor * oldWidth;
                var y2 = (yFloor + 1) * oldWidth;
                var yw = y * newWidth;

                for (var x = 0; x < newWidth; x++)
                {
                    int xFloor = (int)Mathf.Floor(x * ratioX);
                    var xLerp = x * ratioX - xFloor;
                    newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                           ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                           y * ratioY - yFloor);
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static void PointScale(object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                var thisY = (int)(ratioY * y) * oldWidth;
                var yw = y * newWidth;
                for (var x = 0; x < newWidth; x++)
                {
                    newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
        {
            return new Color(c1.r + (c2.r - c1.r) * value,
                              c1.g + (c2.g - c1.g) * value,
                              c1.b + (c2.b - c1.b) * value,
                              c1.a + (c2.a - c1.a) * value);
        }

        #endregion
    }
}
