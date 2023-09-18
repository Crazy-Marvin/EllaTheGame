namespace Yodo1.MAS
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;

    public static class Yodo1AdSettingsSave
    {
        const string YODO1_RESOURCE_PATH = "Assets/Resources/Yodo1/";
        const string YODO1_ADS_SETTINGS_PATH = YODO1_RESOURCE_PATH + "Yodo1AdSettings.asset";

        public static Yodo1AdSettings Load()
        {
            Yodo1AdSettings settings = AssetDatabase.LoadAssetAtPath<Yodo1AdSettings>(YODO1_ADS_SETTINGS_PATH);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<Yodo1AdSettings>();
                try
                {
                    Debug.Log(Yodo1U3dMas.TAG + "Creating new Yodo1AdSettings.asset");
                    string resPath = Path.GetFullPath(YODO1_RESOURCE_PATH);
                    if (!Directory.Exists(resPath))
                    {
                        Directory.CreateDirectory(resPath);
                    }
                    AssetDatabase.CreateAsset(settings, YODO1_ADS_SETTINGS_PATH);
                    AssetDatabase.SaveAssets();

                    settings = AssetDatabase.LoadAssetAtPath<Yodo1AdSettings>(YODO1_ADS_SETTINGS_PATH);
                    //Set default AppLovinSdkKey
                    settings.iOSSettings.AppLovinSdkKey = Yodo1AdEditorConstants.DEFAULT_APPLOVIN_SDK_KEY;
                }
                catch (UnityException)
                {
                    Debug.LogError(Yodo1U3dMas.TAG + "Failed to create the Yodo1 Ad Settings asset.");
                }
            }
            return settings;
        }

        public static void Save(Yodo1AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static bool CheckConfiguration_iOS(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                string message = "MAS iOS settings is null, please check the configuration.";
                Debug.LogError(Yodo1U3dMas.TAG + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(settings.iOSSettings.AppKey.Trim()))
            {
                string message = "MAS iOS AppKey is null, please check the configuration.";
                Debug.LogError(Yodo1U3dMas.TAG + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (settings.iOSSettings.GlobalRegion && string.IsNullOrEmpty(settings.iOSSettings.AdmobAppID.Trim()))
            {
                string message = "MAS iOS AdMob App ID is null, please check the configuration.";
                Debug.LogError(Yodo1U3dMas.TAG + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }
            return true;
        }
    }

}


