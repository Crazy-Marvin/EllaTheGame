using UnityEngine;
using System.Collections;

namespace Yodo1.MAS
{
    public class Yodo1U3dAdsAndroid
    {
#if UNITY_ANDROID
        static AndroidJavaClass javaClass = null;
        static AndroidJavaObject currentActivity;
        static Yodo1U3dAdsAndroid()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                javaClass = new AndroidJavaClass("com.yodo1.mas.UntiyYodo1Mas");

                using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                }
            }
        }

        /// <summary>
        /// Initialize the with app key.
        /// </summary>
        /// <param name="appKey">App key.</param>
        public static void InitWithAppKey(string appKey)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("init", currentActivity, appKey, Yodo1U3dMasCallback.Instance.SdkObjectName, Yodo1U3dMasCallback.Instance.SdkMethodName);
            }
        }

         public static void InitMasWithAppKey(string appKey)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("initMas", currentActivity, appKey, Yodo1U3dMasCallback.Instance.SdkObjectName, Yodo1U3dMasCallback.Instance.SdkMethodName);
            }
        }

        public static void SetUserConsent(bool consent)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("setGDPR", consent);
            }
        }

        public static bool IsGDPRUserConsent()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                bool value = javaClass.CallStatic<bool>("isGDPRUserConsent");
                return value;
            }
            return false;
        }

        public static void SetAdBuildConfig(string adBuildConfig)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("setAdBuildConfig", currentActivity, adBuildConfig);
            }
        }

        public static void SetTagForUnderAgeOfConsent(bool underAgeOfConsent)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("setCOPPA", underAgeOfConsent);
            }
        }

        public static bool IsCOPPAAgeRestricted()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                bool value = javaClass.CallStatic<bool>("isCOPPAAgeRestricted");
                return value;
            }
            return false;
        }

        public static void SetDoNotSell(bool doNotSell)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("setCCPA", doNotSell);
            }
        }

        public static bool IsCCPADoNotSell()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                bool value = javaClass.CallStatic<bool>("isCCPADoNotSell");
                return value;
            }
            return false;
        }

        public static int GetUserAge()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                int value = javaClass.CallStatic<int>("getUserAge");
                return value;
            }
            return 0;
        }

        public static void ShowInterstitialAd()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showInterstitialAd", currentActivity);
            }
        }

        public static void ShowInterstitialAd(string placementId)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showInterstitialAd", currentActivity, placementId);
            }
        }

        public static bool IsInterstitialLoaded()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                bool value = javaClass.CallStatic<bool>("isInterstitialAdLoaded");
                return value;
            }
            return false;
        }

        public static void ShowRewardedAd()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showRewardedAd", currentActivity);
            }
        }

        public static void ShowRewardedAd(string placementId)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showRewardedAd", currentActivity, placementId);
            }
        }

        public static bool IsRewardedAdLoaded()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                bool value = javaClass.CallStatic<bool>("isRewardedAdLoaded");
                return value;
            }
            return false;
        }

        public static bool IsBannerAdLoaded()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                bool value = javaClass.CallStatic<bool>("isBannerAdLoaded");
                return value;
            }
            return false;
        }

        public static void ShowBannerAd()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showBannerAd", currentActivity);
            }
        }

        public static void ShowBannerAd(string placementId)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showBannerAd", currentActivity, placementId);
            }
        }

        public static void ShowBannerAd(int align)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showBannerAd", currentActivity, align);
            }
        }

        public static void ShowBannerAd(int align, int offsetX, int offsetY)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showBannerAd", currentActivity, align, offsetX, offsetY);
            }
        }

        public static void ShowBannerAd(string placementId, int align, int offsetX, int offsetY)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("showBannerAd", currentActivity, placementId, align, offsetX, offsetY);
            }
        }

        public static void DismissBannerAd()
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("dismissBannerAd", currentActivity);
            }
        }

        public static void DismissBannerAd(bool destroy)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic("dismissBannerAd", currentActivity, destroy);
            }
        }

        public static void BannerV2(string methodName, string param)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic(methodName, currentActivity, param);
            }
        }

        public static int GetBannerHeight(int type)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                int value = javaClass.CallStatic<int>("getBannerHeight", type);
                return value;
            }
            return 0;
        }

        public static int GetBannerWidth(int type)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                int value = javaClass.CallStatic<int>("getBannerWidth", type);
                return value;
            }
            return 0;
        }

        public static float GetBannerHeightInPixels(int type)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                float value = javaClass.CallStatic<float>("getBannerHeightInPixels", type);
                return value;
            }
            return 0;
        }
        public static float GetBannerWidthInPixels(int type)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                float value = javaClass.CallStatic<float>("getBannerWidthInPixels", type);
                return value;
            }
            return 0;
        }

        public static void Native(string methodName, string param)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic(methodName, currentActivity, param);
            }
        }

        public static void RewardedInterstitial(string methodName, string param)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic(methodName, currentActivity, param);
            }
        }

        public static void AppOpen(string methodName, string param)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic(methodName, currentActivity, param);
            }
        }

        public static void RewardV2(string methodName, string param)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic(methodName, currentActivity, param);
            }
        }


        public static void InterstitialV2(string methodName, string param)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                javaClass.CallStatic(methodName, currentActivity, param);
            }
        }

        public static bool IsAdLoadedV2(string methodName)
        {
            if (Application.platform == RuntimePlatform.Android && javaClass != null)
            {
                bool value = javaClass.CallStatic<bool>(methodName);
                return value;
            }
            return false;
        }
#endif
    }
}