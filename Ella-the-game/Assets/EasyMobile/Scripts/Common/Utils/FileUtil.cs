using UnityEngine;
using System.Collections;

namespace EasyMobile.Internal
{
    #if !UNITY_WEBPLAYER
    using System.IO;
    #endif

    internal static class FileUtil
    {
        public static byte[] ReadAllBytes(string path)
        {
            #if !UNITY_WEBPLAYER
            return File.ReadAllBytes(path);
            #else
            return new byte[0];
            #endif
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            #if !UNITY_WEBPLAYER
            File.WriteAllBytes(path, bytes);
            #endif
        }

        public static void WriteAllLines(string path, string[] lines)
        {
            #if !UNITY_WEBPLAYER
            File.WriteAllLines(path, lines);
            #endif
        }
    }
}
