using System;
using UnityEngine;

namespace EasyMobile
{
    [Serializable]
    public class ChartboostSettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets Chartboost's custom interstitial placements (used in <see cref="AutoLoadAdsMode.LoadAllDefinedPlacements"/>).
        /// </summary>
        public AdPlacement[] CustomInterstitialPlacements
        {
            get { return mCustomInterstitialPlacements; }
            set { mCustomInterstitialPlacements = value; }
        }

        /// <summary>
        /// Gets or sets Chartboost's custom rewarded ad placements (used in <see cref="AutoLoadAdsMode.LoadAllDefinedPlacements"/>).
        /// </summary>
        public AdPlacement[] CustomRewardedPlacements
        {
            get { return mCustomRewardedPlacements; }
            set { mCustomRewardedPlacements = value; }
        }

        [SerializeField]
        private AdPlacement[] mCustomInterstitialPlacements;
        [SerializeField]
        private AdPlacement[] mCustomRewardedPlacements;
    }
}
