using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace EasyMobile.Editor
{
    public static class EM_ExternalPluginManager
    {
        // AdColony
        public const string AdColonyNameSpace = "AdColony";

        // AdMob
        public const string GoogleMobileAdsNameSpace = "GoogleMobileAds";

        // AppLovin //AppLovin class has been remove and replaced with MaxSdk
        public const string AppLovinClassName = "MaxSdk";

        // Chartboost
        public const string ChartboostNameSpace = "ChartboostSDK";
        public const string ChartboostClassName = "Chartboost";

        // Facebook Audience Network.
        public const string FBAudienceNameSpace = "AudienceNetwork";

        // FairBid
        public const string FairBidNameSpace = "Fyber";

        // IronSource
        public const string IronSourceClassname = "IronSource";

        // MoPub
        public const string MoPubClassName = "MoPub";

        // TapJoy
        public const string TapJoyNameSpace = "TapjoyUnity";

        //UnityAds
        public const string UnityAdNameSpace = "UnityEngine.Advertisements";
        public const string UnityAdvertisementClass = "Advertisement";

        //Vungle
        public const string VungleClassName = "Vungle";

        // Unity Monetization
        public const string UnityMonetizationClass = "UnityEngine.Monetization";

        // UnityIAP
        public const string UnityPurchasingAssemblyName = "UnityEngine.Purchasing";
        public const string UnityPurchasingNameSpace = "UnityEngine.Purchasing";
        public const string UnityPurchasingSecurityNameSpace = "UnityEngine.Purchasing.Security";
        public const string UnityPurchasingClassName = "UnityPurchasing";

        // Google Play Games
        public const string GPGSNameSpace = "GooglePlayGames";
        public const string GPGSClassName = "PlayGamesPlatform";

        // OneSignal
        public const string OneSignalClassName = "OneSignal";

        // Firebase Messaging
        public const string FirebaseMessagingNameSpace = "Firebase.Messaging";
        public const string FirebaseMessagingClassName = "FirebaseMessaging";

        // PlayMaker Unity UI add-on
        public const string PlayMakerUguiAddOnClass = "PlayMakerUGuiSceneProxy";

        // External Dependency Manager namespaces and classes
        public const string GoogleNameSpace = "Google";
        public const string IosResolverClass = "IOSResolver";
        public const string JarResolverNamespace = "Google.JarResolver";
        public const string JarResolverDependencyClass = "Dependency";
        public const string PackageManagerResolverClass = "PackageManagerResolver";

        // URP namespace
        public const string URPNamespace = "UnityEngine.Rendering.Universal";

        // Advertising 3rd party plugins URLs
        public const string AdColonyDownloadURL = "https://github.com/AdColony/AdColony-Unity-Plugin";
        public const string ChartboostDownloadURL = "https://answers.chartboost.com/en-us/articles/download";
        public const string FBAudienceDownloadURL = "https://developers.facebook.com/docs/audience-network/download#unity";
        public const string GoogleMobileAdsDownloadURL = "https://github.com/googleads/googleads-mobile-unity/releases";
        public const string AppLovinDownloadURL = "https://www.applovin.com/monetize/";
        public const string FairBidDownloadURL = "https://dev-unity.fyber.com/docs";
        public const string IronSourceDownloadURL = "https://developers.ironsrc.com/ironsource-mobile/unity/unity-plugin/#step-1";
        public const string MoPubDownloadURL = "https://github.com/mopub/mopub-unity-sdk/";
        public const string TapJoyDownloadURL = "https://ltv.tapjoy.com/d/sdks";
        public const string VungleDownloadURL = "https://publisher.vungle.com/sdk/plugins/unity";

        // Game Services 3rd party plugins URLs
        public const string GooglePlayGamesDownloadURL = "https://github.com/playgameservices/play-games-plugin-for-unity";

        // Notifications 3rd party plugins URLs
        public const string OneSignalDownloadURL = "https://github.com/OneSignal/OneSignal-Unity-SDK";
        public const string FirebaseDownloadURL = "https://firebase.google.com/docs/unity/setup";

        // PlayServicesResolver
        public const string PlayServicesResolverPackagePath = EM_Constants.PackagesFolder + "/ExternalDependencyManager/external-dependency-manager-latest.unitypackage";

        // PlayMaker actions for EM.
        public const string PlayMakerActionsPackagePath = EM_Constants.PackagesFolder + "/PlayMakerActions/PlayMakerActions.unitypackage";
        public const string PlayMakerActionsDownloadURL = "http://u3d.as/1xGQ";

        /// <summary>
        /// Determines if AdColony plugin is available.
        /// </summary>
        /// <returns><c>true</c> if AdColony plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsAdColonyAvail()
        {
            if (!EM_Settings.Advertising.AdColony.Enable)
                return false;
            return EM_EditorUtil.NamespaceExists(AdColonyNameSpace);
        }

        /// <summary>
        /// Determines if AdMob plugin is available.
        /// </summary>
        /// <returns><c>true</c> if AdMob plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsAdMobAvail()
        {
            if (!EM_Settings.Advertising.AdMob.Enable)
                return false;
            return EM_EditorUtil.NamespaceExists(GoogleMobileAdsNameSpace);
        }

        /// <summary>
        /// Determines if AppLovin plugin is available.
        /// </summary>
        /// <returns><c>true</c> if AppLovin plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsAppLovinAvail()
        {
            if (!EM_Settings.Advertising.AppLovin.Enable)
                return false;
            return EM_EditorUtil.FindClass(AppLovinClassName) != null;
        }

        /// <summary>
        /// Determines if Chartboost plugin is available.
        /// </summary>
        /// <returns><c>true</c> if Chartboost plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsChartboostAvail()
        {
            if (!EM_Settings.Advertising.Chartboost.Enable)
                return false;
            System.Type chartboost = EM_EditorUtil.FindClass(ChartboostClassName, ChartboostNameSpace);
            return chartboost != null;
        }

        /// <summary>
        /// Determines if Facebook Audience Network plugin is available.
        /// </summary>
        /// <returns></returns>
        public static bool IsFBAudienceAvail()
        {
            if (!EM_Settings.Advertising.AudienceNetwork.Enable)
                return false;
            return EM_EditorUtil.NamespaceExists(FBAudienceNameSpace);
        }

        /// <summary>
        /// Determines if FairBid plugin is available.
        /// </summary>
        /// <returns><c>true</c> if FairBid plugin available; otherwise, <c>false</c>.</returns>
        public static bool IsFairBidAvail()
        {
            if (!EM_Settings.Advertising.FairBid.Enable)
                return false;
            return EM_EditorUtil.NamespaceExists(FairBidNameSpace);
        }

        /// <summary>
        /// Determines if MoPub plugin is available.
        /// </summary>
        /// <returns><c>true</c> if MoPub plugin is available; otherwise, <c>false</c>.</returns>
        public static bool IsMoPubAvail()
        {
            if (!EM_Settings.Advertising.MoPub.Enable)
                return false;
            return EM_EditorUtil.FindClass(MoPubClassName) != null;
        }

        /// Determines if IronSource plugin is available.
        /// </summary>
        public static bool IsIronSourceAvail()
        {
            if (!EM_Settings.Advertising.IronSource.Enable)
                return false;
            return EM_EditorUtil.FindClass(IronSourceClassname) != null;
        }

        /// Determindes if TapJoy plugin is available.
        /// </summary>
        /// <returns><c>true</c> if TapJoy plugin is available, otherwise <c>false</c>.</returns>
        public static bool IsTapJoyAvail()
        {
            if (!EM_Settings.Advertising.Tapjoy.Enable)
                return false;
            return EM_EditorUtil.NamespaceExists(TapJoyNameSpace);
        }

        /// Determindes if UnityAds plugin is available.
        /// </summary>
        /// <returns><c>true</c> if UnityAds plugin is available, otherwise <c>false</c>.</returns>
        public static bool IsUnityAdAvail()
        {
            if (!EM_Settings.Advertising.UnityAds.Enable)
                return false;
            return EM_EditorUtil.NamespaceExists(UnityAdNameSpace) && EM_EditorUtil.FindClass(UnityAdvertisementClass, UnityAdNameSpace) != null;
        }

        //Determindes if Vungle plugin is available.
        /// </summary>
        /// <returns><c>true</c> if Vungle plugin is available, otherwise <c>false</c>.</returns>
        public static bool IsVungleAvail()
        {
            if (!EM_Settings.Advertising.VungleAds.Enable)
                return false;
            return EM_EditorUtil.FindClass(VungleClassName) != null;
        }

        /// Determines if Unity Monetization plugin is available.
        /// </summary>
        /// <returns><c>true</c> if Unity Monetization plugin is available, otherwise <c>false</c>.</returns>
        public static bool IsUnityMonetizationAvail()
        {
            if (!EM_Settings.Advertising.UnityAds.Enable)
                return false;
            return EM_EditorUtil.NamespaceExists(UnityMonetizationClass);
        }

        /// <summary>
        /// Determines if UnityIAP is enabled.
        /// </summary>
        /// <returns><c>true</c> if enabled; otherwise, <c>false</c>.</returns>
        public static bool IsUnityIAPAvail()
        {
            // Here we check for the existence of the Security namespace instead of UnityPurchasing class in order to
            // make sure that the plugin is actually imported (rather than the service just being enabled).
            return EM_EditorUtil.NamespaceExists(UnityPurchasingSecurityNameSpace);
        }

        /// <summary>
        /// Determines if GooglePlayGames plugin is available.
        /// </summary>
        /// <returns><c>true</c> if is GPGS avail; otherwise, <c>false</c>.</returns>
        public static bool IsGPGSAvail()
        {
            System.Type gpgs = EM_EditorUtil.FindClass(GPGSClassName, GPGSNameSpace);
            return gpgs != null;
        }

        /// <summary>
        /// Determines if OneSignal plugin is available.
        /// </summary>
        /// <returns><c>true</c> if is one signal avail; otherwise, <c>false</c>.</returns>
        public static bool IsOneSignalAvail()
        {
            System.Type oneSignal = EM_EditorUtil.FindClass(OneSignalClassName);
            return oneSignal != null;
        }

        /// <summary>
        /// Determines if FirebaseMessaging plugin is available.
        /// </summary>
        public static bool IsFirebaseMessagingAvail()
        {
            System.Type firMsg = EM_EditorUtil.FindClass(FirebaseMessagingClassName, FirebaseMessagingNameSpace);
            return firMsg != null;
        }

        /// <summary>
        /// Determines if URP is in use.
        /// </summary>
        /// <returns><c>true</c> if URP is not used; otherwise, <c>false</c>.</returns>
        public static bool IsUsingURP()
        {
            return EM_EditorUtil.NamespaceExists(URPNamespace);
        }

        /// <summary>
        /// Determines if PlayMaker is installed.
        /// </summary>
        /// <returns><c>true</c> if is play maker avail; otherwise, <c>false</c>.</returns>
        public static bool IsPlayMakerAvail()
        {
#if PLAYMAKER
            return true;
#else
            return false;
#endif
        }

        public static bool IsPlayMakerUguiAddOnAvail()
        {
            System.Type uGui = EM_EditorUtil.FindClass(PlayMakerUguiAddOnClass);

            return uGui != null;
        }

        public static void DownloadGoogleMobileAdsPlugin()
        {
            Application.OpenURL(GoogleMobileAdsDownloadURL);
        }

        public static void DownloadGooglePlayGamesPlugin()
        {
            Application.OpenURL(GooglePlayGamesDownloadURL);
        }

        public static void DownloadOneSignalPlugin()
        {
            Application.OpenURL(OneSignalDownloadURL);
        }

        public static void DownloadChartboostPlugin()
        {
            Application.OpenURL(ChartboostDownloadURL);
        }

        public static void DownloadFairBidPlugin()
        {
            Application.OpenURL(FairBidDownloadURL);
        }

        public static void DownloadAdColonyPlugin()
        {
            Application.OpenURL(AdColonyDownloadURL);
        }

        public static void DownloadAppLovinPlugin()
        {
            Application.OpenURL(AppLovinDownloadURL);
        }

        public static void DownloadFirebasePlugin()
        {
            Application.OpenURL(FirebaseDownloadURL);
        }

        public static void DownloadFacebookAudiencePlugin()
        {
            Application.OpenURL(FBAudienceDownloadURL);
        }

        public static void DownloadMoPubPlugin()
        {
            Application.OpenURL(MoPubDownloadURL);
        }

        public static void DownloadIronSourcePlugin()
        {
            Application.OpenURL(IronSourceDownloadURL);
        }

        public static void DownloadTapJoyPlugin()
        {
            Application.OpenURL(TapJoyDownloadURL);
        }

        public static void DownloadVunglePlugin()
        {
            Application.OpenURL(VungleDownloadURL);
        }

        public static void InstallPlayMakerActions(bool interactive)
        {
            // First check if the PlayMaker Actions package has been imported.
            if (!FileIO.FileExists(PlayMakerActionsPackagePath))
            {
                if (EM_EditorUtil.DisplayDialog(
                    "PlayMaker Actions Not Found",
                    "Looks like you haven't imported the \"PlayMaker Actions for Easy Mobile Pro\" package. " +
                    "Please download and import it from the Unity Asset Store first.",
                    "Get it now",
                    "Later"))
                {
                    Application.OpenURL(PlayMakerActionsDownloadURL);
                }

                return;
            }

            // Check if PlayMaker itself has been installed.
            if (!EM_ExternalPluginManager.IsPlayMakerAvail())
            {
                if (EM_EditorUtil.DisplayDialog(
                        "Installing PlayMaker Actions",
                        "Looks like you haven't installed PlayMaker, please install it to use these actions.",
                        "Continue Anyway",
                        "Cancel"))
                {
                    DoInstallPlayMakerActions(interactive);
                }
            }
            else
            {
                DoInstallPlayMakerActions(interactive);
            }
        }

        private static void DoInstallPlayMakerActions(bool interactive)
        {
            AssetDatabase.ImportPackage(PlayMakerActionsPackagePath, interactive);
        }

        public static bool IsPlayServicesResolverImported()
        {
            var imported = EM_ProjectSettings.Instance.GetBool(EM_Constants.PSK_ImportedPlayServicesResolver, false);

            var iosResolver = EM_EditorUtil.FindClass(IosResolverClass, GoogleNameSpace);
            var jarResolver = EM_EditorUtil.FindClass(JarResolverDependencyClass, JarResolverNamespace);
            var packageResolver = EM_EditorUtil.FindClass(PackageManagerResolverClass, GoogleNameSpace);

            return iosResolver != null || jarResolver != null || packageResolver != null || imported;
        }

        public static void ImportPlayServicesResolver(bool interactive)
        {
            AssetDatabase.ImportPackage(PlayServicesResolverPackagePath, interactive);
            EM_ProjectSettings.Instance.Set(EM_Constants.PSK_ImportedPlayServicesResolver, true);
        }
    }
}

