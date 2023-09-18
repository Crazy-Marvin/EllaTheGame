#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace EasyMobile.ManifestGenerator
{
    public static class FolderHelper
    {
        public static void CreateFolder(string folderPath)
        {
            if (IsFolderExists(folderPath))
                return;

            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        public static void DeleteFolder (string folderPath, bool deletedAllContents = false)
        {
            if (!IsFolderExists(folderPath))
                return;

            Directory.Delete(folderPath, deletedAllContents);
            AssetDatabase.Refresh();
        }

        public static bool IsFolderExists(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return false;

            return Directory.Exists(folderPath);
        }
    }
}
#endif
