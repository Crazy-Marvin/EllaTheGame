#if UNITY_2018_4_OR_NEWER && (UNITY_ANDROID || UNITY_IOS)

using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Yodo1.MAS;
using UnityEditor;
using System;

public class Yodo1IdSync : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
#if UNITY_ANDROID
    private String defaultAdmobAppID = "ca-app-pub-5580537606944457~4465578836";
#elif UNITY_IOS
    private String defaultAdmobAppID = "ca-app-pub-5580537606944457~2166718551";
#endif

    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_ANDROID
        if (!Yodo1AdUtils.IsGooglePlayVersion())
        {
            return;
        }
#endif

        Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();

        string appKey = string.Empty;
        string admobAppID = string.Empty;
        string bundleId = string.Empty;
#if UNITY_ANDROID
        appKey = settings.androidSettings.AppKey;
        admobAppID = settings.androidSettings.AdmobAppID;
#if UNITY_2023_2_OR_NEWER
        bundleId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
#else
        bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#endif

#elif UNITY_IOS
        appKey = settings.iOSSettings.AppKey;
        admobAppID = settings.iOSSettings.AdmobAppID;
#if UNITY_2023_2_OR_NEWER
        bundleId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.iOS);
#else
        bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
#endif

#endif
        if (string.IsNullOrEmpty(appKey))
        {
            return;
        }

        Dictionary<string, object> obj = Yodo1Net.GetInstance().GetAppInfoByAppKey(appKey);
        if (obj.ContainsKey("bundle_id"))
        {
            if (string.IsNullOrEmpty((string)obj["bundle_id"]))
            {
                UnityEngine.Debug.Log(Yodo1U3dMas.TAG + "Update the store link when your game is live on Play Store or App Store.");
                return;
            }
        }

        if (!string.Equals(admobAppID, defaultAdmobAppID))
        {
#if UNITY_ANDROID
            Dictionary<string, object> data = Yodo1Net.GetInstance().GetAppInfoByBundleID("android", bundleId);
#elif UNITY_IOS
            Dictionary<string, object> data = Yodo1Net.GetInstance().GetAppInfoByBundleID("iOS", bundleId);
#endif
            Yodo1AdAssetsImporter.UpdateData(settings, data);
        }
    }

}

#endif