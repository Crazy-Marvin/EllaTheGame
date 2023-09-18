using UnityEngine;
using System.Collections;
using System.Linq;

namespace EasyMobile.Editor
{
    public static class EM_ScriptingSymbols
    {
        public const string EasyMobile = "EASY_MOBILE;EASY_MOBILE_PRO";
        public const string UnityIAP = "EM_UIAP";
        public const string GooglePlayGames = "EM_GPGS";
        public const string NoGooglePlayGames = "NO_GPGS";
        public const string OneSignal = "EM_ONESIGNAL";
        public const string FirebaseMessaging = "EM_FIR_MESSAGING";
        public const string ContactsSubmodule = "EM_CONTACTS";
        public const string CameraGallerySubmodule = "EM_CAMERA_GALLERY";
        public const string UniversalRenderPipeline = "EM_URP";

        // Ad networks
        public const string AdColony = "EM_ADCOLONY";
        public const string AdMob = "EM_ADMOB";
        public const string AppLovin = "EM_APPLOVIN";
        public const string Chartboost = "EM_CHARTBOOST";
        public const string FairBid = "EM_FAIRBID";
        public const string MoPub = "EM_MOPUB";
        public const string FBAudience = "EM_FBAN";
        public const string IronSource = "EM_IRONSOURCE";
        public const string TapJoy = "EM_TAPJOY";
        public const string UnityAds = "EM_UNITY_ADS";
        public const string Vungle = "EM_VUNGLE";
        public const string UnityMonetization = "UNITY_MONETIZATION";

        public static string[] GetAllSymbols()
        {
            return EM_EditorUtil.GetConstants(typeof(EM_ScriptingSymbols)).Select(c => c.GetRawConstantValue() as string).ToArray();
        }

    }
}

