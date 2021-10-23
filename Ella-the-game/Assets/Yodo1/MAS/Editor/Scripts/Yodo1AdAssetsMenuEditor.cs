using System;
using UnityEngine;
using UnityEditor;

namespace Yodo1.MAS
{
    public class Yodo1AdAssetsMenuEditor : Editor
    {
        [MenuItem("Assets/Yodo1/MAS Conflict Manager")]
        public static void ConflictManager()
        {
            Yodo1AdIntegrationManagerWindow.ShowManager();
        }

        [MenuItem("Assets/Yodo1/MAS Settings/Android Settings")]
        public static void AndroidSettings()
        {
            Yodo1AdWindows.Initialize(Yodo1AdWindows.PlatfromTab.Android);
        }

        [MenuItem("Assets/Yodo1/MAS Settings/iOS Settings")]
        public static void IOSSettings()
        {
            Yodo1AdWindows.Initialize(Yodo1AdWindows.PlatfromTab.iOS);
        }
              

    }
}
