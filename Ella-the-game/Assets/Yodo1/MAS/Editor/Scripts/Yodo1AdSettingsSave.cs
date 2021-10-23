using System;

using UnityEditor;
using UnityEngine;
using System.IO;

namespace Yodo1.MAS
{
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
                    Debug.Log("[Yodo1 Mas] Creating new Yodo1AdSettings.asset");
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
                    Debug.LogError("[Yodo1 Mas] Failed to create the Yodo1 Ad Settings asset.");
                }
            }
            return settings;
        }

        public static void Save(Yodo1AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }

        public static void UpdateDependencies(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                Debug.LogError("[Yodo1 Mas] Update dependencies failed. Yodo1 ad settings is not exsit.");
                return;
            }
            string dependenciesPath = Path.GetFullPath(Application.dataPath + "/Yodo1/MAS/Editor/Dependencies/");
            string dependenciesTemplatePath = dependenciesPath + "Template/";

            string sourcePath = string.Empty;
            if (settings.androidSettings.ChineseAndroidStores)
            {
                sourcePath = dependenciesTemplatePath + "Android/Yodo1AdsAndroidDependenciesChina.xml";
            }
            else if (settings.androidSettings.GooglePlayStore)
            {
                sourcePath = dependenciesTemplatePath + "Android/Yodo1AdsAndroidDependenciesGlobal.xml";
            }
            if (File.Exists(sourcePath))
            {
                string destFile = dependenciesPath + "Yodo1AdsAndroidDependencies.xml";
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                File.Copy(sourcePath, destFile, true);
            }

            //if (settings.iOSSettings.ChinaRegion) {
            //    string sourcePath = dependenciesTemplatePath + "iOS/ChinaDependenciesTemplate.xml";
            //    File.Copy(sourcePath,dependenciesPath + "Yodo1AdsiOSDependencies.xml", true);
            //} else if (settings.iOSSettings.GlobalRegion) {
            //    string sourcePath = dependenciesTemplatePath + "iOS/GlobalDependenciesTemplate.xml";
            //    File.Copy(sourcePath,dependenciesPath + "Yodo1AdsiOSDependencies.xml", true);
            //}

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

}


