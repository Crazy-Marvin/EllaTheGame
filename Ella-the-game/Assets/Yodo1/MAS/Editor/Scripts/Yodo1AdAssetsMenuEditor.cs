namespace Yodo1.MAS
{
    using UnityEditor;

    public class Yodo1AdAssetsMenuEditor : Editor
    {
        [MenuItem("Yodo1/MAS/MAS Conflict Manager")]
        public static void ConflictManager()
        {
            Yodo1AdIntegrationManagerWindow.ShowManager();
        }

        [MenuItem("Yodo1/MAS/MAS Settings/Android Settings")]
        public static void AndroidSettings()
        {
            Yodo1AdWindows.Initialize(Yodo1AdWindows.PlatfromTab.Android);
        }

        [MenuItem("Yodo1/MAS/MAS Settings/iOS Settings")]
        public static void IOSSettings()
        {
            Yodo1AdWindows.Initialize(Yodo1AdWindows.PlatfromTab.iOS);
        }


    }
}
