using UnityEngine;
using System.Collections;

namespace EasyMobile.Editor
{
    public static partial class EM_Constants
    {
        // Product name
        public const string ProductName = "Easy Mobile";
        public const string Copyright = "© 2017-2020 SgLib Games LLC. All Rights Reserved.";

        // Folder
        public const string RootPath = "Assets/EasyMobile";
        public const string EditorFolder = RootPath + "/Editor";
        public const string TemplateFolder = EditorFolder + "/Templates";
        public const string GeneratedFolder = RootPath + "/Generated";
        public const string MainPrefabFolder = RootPath;
        public const string MaterialsFolder = RootPath + "/Materials";
        public const string PackagesFolder = RootPath + "/Packages";
        public const string SkinFolder = RootPath + "/GUISkins";
        public const string SkinTextureFolder = SkinFolder + "/Textures";
        public const string ResourcesFolder = RootPath + "/Resources";
        public const string ScriptsFolder = RootPath + "/Scripts";
        public const string ReceiptValidationFolder = "Assets/Plugins/UnityPurchasing/generated";
        public const string AssetsPluginsAndroidFolder = "Assets/Plugins/Android";
        public const string AssetsPluginsIOSFolder = "Assets/Plugins/iOS";

        // Assets and stuff
        public const string SettingsAssetName = "EM_Settings";
        public const string SettingsAssetExtension = ".asset";
        public const string SettingsAssetPath = ResourcesFolder + "/EM_Settings.asset";
        public const string MainPrefabName = "EasyMobile";
        public const string PrefabExtension = ".prefab";
        public const string MainPrefabPath = MainPrefabFolder + "/EasyMobile.prefab";
        public const string PluginSettingsFilePath = EditorFolder + "/EasyMobileSettings.txt";
        public const string ClipPlayerMaterialPath = MaterialsFolder + "/ClipPlayerMat.mat";

        // Demo app.
        public const string DemoFolder = RootPath + "/Demo";
        public const string DemoScenesFolder = DemoFolder + "/Scenes";
        public const string DemoHomeScenePath = DemoScenesFolder + "/DemoHome.unity";
        public const string DemoAdvertisingScenePath = DemoScenesFolder + "/Modules/AvertisingDemo.unity";
        public const string DemoGameServicesScenePath = DemoScenesFolder + "/Modules/GameServicesDemo.unity";
        public const string DemoInAppPurchasingScenePath = DemoScenesFolder + "/Modules/InAppPurchasingDemo.unity";
        public const string DemoNativeUIScenePath = DemoScenesFolder + "/Modules/NativeUIDemo.unity";
        public const string DemoNotificationsScenePath = DemoScenesFolder + "/Modules/NotificationsDemo.unity";
        public const string DemoPrivacyScenePath = DemoScenesFolder + "/Modules/PrivacyDemo.unity";
        public const string DemoSharingScenePath = DemoScenesFolder + "/Modules/SharingDemo.unity";
        public const string DemoUtilitiesScenePath = DemoScenesFolder + "/Modules/UtilitiesDemo.unity";

        // Android native package names.
        public const string AndroidNativePackageName = "com.sglib.easymobile.androidnative";
        public const string AndroidNativeNotificationPackageName = "com.sglib.easymobile.androidnative.notification";

        // Generated class names
        public const string RootNameSpace = "EasyMobile";
        public const string AndroidGPGSConstantClassName = "EM_GPGSIds";
        public const string GameServicesConstantsClassName = "EM_GameServicesConstants";
        public const string IAPConstantsClassName = "EM_IAPConstants";
        public const string NotificationsConstantsClassName = "EM_NotificationsConstants";
        public const string NotificationAndroidResFolderName = "EMNotificationResources";
        public const string EasyMobileAndroidResFolderName = "EasyMobile";
        public const string AdvertisingConstantsClassName = "EM_AdvertisingConstants";

        // URLs
        public const string EasyMobileWebsiteURL = "https://easymobile.sglibgames.com";
#if EASY_MOBILE_PRO
        public const string DocumentationURL = EasyMobileWebsiteURL + "/docs/pro";
        public const string ScriptingRefURL = EasyMobileWebsiteURL + "/api/pro";
        public const string AssetStoreURL = "http://u3d.as/Dd2";


#else
        public const string DocumentationURL = EasyMobileWebsiteURL + "/docs/basic";
        public const string ScriptingRefURL = EasyMobileWebsiteURL + "/api/basic";
        public const string AssetStoreURL = "http://u3d.as/18pa";
#endif

        public const string VideoTutorialsURL = "https://www.youtube.com/watch?v=NHg9efWJQyk&list=PLuy0qyrX_-GIqUxjT1PwvfiThmvtgytLL";
        public const string SupportEmail = "support@sglibgames.com";

#if EASY_MOBILE_PRO
        public const string SupportEmailSubject = "[EM Pro][YOUR_INVOICE_NUMBER]";

#else
        public const string SupportEmailSubject = "[EM Basic][YOUR_INVOICE_NUMBER]";
#endif

        // Common symbols
        public const string NoneSymbol = "[None]";
        public const string DeleteSymbol = "-";
        public const string UpSymbol = "↑";
        public const string DownSymbol = "↓";

        // ProjectSettings keys
        public const string PSK_EMVersionString = "VERSION";
        public const string PSK_EMVersionInt = "VERSION_INT";
        public const string PSK_ImportedPlayServicesResolver = "IMPORTED_PLAY_SERVICES_RESOLVER";
    }
}

