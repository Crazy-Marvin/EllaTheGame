using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    [Serializable]
    public class AdColonySettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets the AdColony app identifier.
        /// </summary>
        /// <value>The app identifier.</value>
        public AdId AppId
        {
            get { return mAppId; }
            set { mAppId = value; }
        }

        /// <summary>
        /// Gets or sets the ad orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public AdOrientation Orientation
        {
            get { return mOrientation; }
            set { mOrientation = value; }
        }

        /// <summary>
        /// Enables or disables the default pre-popup that shows before a rewarded ad begins.
        /// </summary>
        /// <value><c>true</c> if enable rewarded ad pre popup; otherwise, <c>false</c>.</value>
        public bool EnableRewardedAdPrePopup
        {
            get { return mEnableRewardedAdPrePopup; }
            set { mEnableRewardedAdPrePopup = value; }
        }

        /// <summary>
        /// Enables or disables the default post-popup that shows after a rewarded ad has completed.
        /// </summary>
        /// <value><c>true</c> if enable rewarded ad post popup; otherwise, <c>false</c>.</value>
        public bool EnableRewardedAdPostPopup
        {
            get { return mEnableRewardedAdPostPopup; }
            set { mEnableRewardedAdPostPopup = value; }
        }

        /// <summary>
        /// Gets or sets the default interstitial ad identifier.
        /// </summary>
        /// <value>The default interstitial ad identifier.</value>
        public AdId DefaultInterstitialAdId
        {
            get { return mDefaultInterstitialAdId; }
            set { mDefaultInterstitialAdId = value; }
        }

        /// <summary>
        /// Gets or sets the default rewarded ad identifier.
        /// </summary>
        /// <value>The default rewarded ad identifier.</value>
        public AdId DefaultRewardedAdId
        {
            get { return mDefaultRewardedAdId; }
            set { mDefaultRewardedAdId = value; }
        }

        /// <summary>
        /// Gets or sets the default banner ad identifier.
        /// </summary>
        /// <value>The default rewarded ad identifier.</value>
        public AdId DefaultBannerAdId
        {
            get { return mDefaultBannerAdId; }
            set { mDefaultBannerAdId = value; }
        }

        /// <summary>
        /// Gets or sets the list of custom interstitial ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        /// <value>The custom interstitial ad identifiers.</value>
        public Dictionary<AdPlacement, AdId> CustomInterstitialAdIds
        {
            get { return mCustomInterstitialAdIds; }
            set { mCustomRewardedAdIds = (Dictionary_AdPlacement_AdId)value; }
        }

        /// <summary>
        /// Gets or sets the list of custom rewarded ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        /// <value>The custom rewarded ad identifiers.</value>
        public Dictionary<AdPlacement, AdId> CustomRewardedAdIds
        {
            get { return mCustomRewardedAdIds; }
            set { mCustomRewardedAdIds = (Dictionary_AdPlacement_AdId)value; }
        }

        /// <summary>
        /// Gets or sets the list of custom banner ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        /// <value>The custom rewarded ad identifiers.</value>
        public Dictionary<AdPlacement, AdId> CustomBannerAdIds
        {
            get { return mCustomBannerAdIds; }
            set { mCustomBannerAdIds = (Dictionary_AdPlacement_AdId)value; }
        }

        [SerializeField]
        private AdId mAppId = null;
        [SerializeField]
        private AdOrientation mOrientation = AdOrientation.AdOrientationAll;
        [SerializeField]
        private bool mEnableRewardedAdPrePopup = false;
        [SerializeField]
        private bool mEnableRewardedAdPostPopup = false;

        [SerializeField]
        private AdId mDefaultInterstitialAdId = null;
        [SerializeField]
        private AdId mDefaultRewardedAdId = null;
        [SerializeField]
        private AdId mDefaultBannerAdId = null;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomInterstitialAdIds = null;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomRewardedAdIds = null;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomBannerAdIds = null;
    }
}