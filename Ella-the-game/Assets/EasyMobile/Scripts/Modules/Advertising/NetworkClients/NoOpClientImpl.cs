using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
    internal class NoOpClientImpl : AdClientImpl
    {
        // Singleton.
        private static NoOpClientImpl sInstance;

        private NoOpClientImpl()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static NoOpClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new NoOpClientImpl();
            }
            return sInstance;
        }

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.None; } }

        public override bool IsBannerAdSupported { get { return false; } }

        public override bool IsInterstitialAdSupported { get { return false; } }

        public override bool IsRewardedAdSupported { get { return false; } }

        public override bool IsSdkAvail{ get { return true; } }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict { get { return null; } }

        protected override Dictionary<AdPlacement, AdId> CustomRewardedAdsDict { get { return null; } }

        protected override string NoSdkMessage { get { return string.Empty; } }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
            return false;
        }

        protected override void InternalInit()
        {
        }

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
            return false;
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
            return false;
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
        }

        #endregion

        #region IConsentRequirable Overrides

        protected override string DataPrivacyConsentSaveKey { get { return string.Empty; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
        }

        #endregion
    }
}