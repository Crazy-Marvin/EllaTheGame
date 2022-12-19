using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    public abstract class AdClientImpl : IAdClient, IConsentRequirable
    {
        #region IAdClient Implementation

        protected bool mIsInitialized = false;

        /// <summary>
        /// Whether the required SDK is available.
        /// </summary>
        /// <value><c>true</c> if avail; otherwise, <c>false</c>.</value>
        public abstract bool IsSdkAvail { get; }

        /// <summary>
        /// Checks if the placement is valid, i.e. it has non-empty
        /// associated IDs if such placement require dedicated IDs.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public abstract bool IsValidPlacement(AdPlacement placement, AdType type);

        /// <summary>
        /// The message to print if the required SDK is not available.
        /// </summary>
        /// <value>The no sdk message.</value>
        protected abstract string NoSdkMessage { get; }

        /// <summary>
        /// Does the SDK-specific initialization. Only invoked if the client is not initialized.
        /// </summary>
        protected abstract void InternalInit();

        /// <summary>
        /// Instructs the underlaying SDK to show a banner ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        protected abstract void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size);

        /// <summary>
        /// Instructs the underlaying SDK to hide a banner ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected abstract void InternalHideBannerAd(AdPlacement placement);

        /// <summary>
        /// Instructs the underlaying SDK to destroy a banner ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected abstract void InternalDestroyBannerAd(AdPlacement placement);

        /// <summary>
        /// Instructs the underlaying SDK to load an interstitial ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected abstract void InternalLoadInterstitialAd(AdPlacement placement);

        /// <summary>
        /// Checks with the underlaying SDK to see if an interstitial ad is loaded. Only invoked if the client is initialized.
        /// </summary>
        /// <returns><c>true</c>, if is interstitial ad ready was internaled, <c>false</c> otherwise.</returns>
        /// <param name="placement">Placement.</param>
        protected abstract bool InternalIsInterstitialAdReady(AdPlacement placement);

        /// <summary>
        /// Instructs the underlaying SDK to show an interstitial ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected abstract void InternalShowInterstitialAd(AdPlacement placement);

        /// <summary>
        /// Instructs the underlaying SDK to load a rewarded ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected abstract void InternalLoadRewardedAd(AdPlacement placement);

        /// <summary>
        /// Checks with the underlaying SDK to see if a rewarded ad is loaded. Only invoked if the client is initialized.
        /// </summary>
        /// <returns><c>true</c>, if is rewarded ad ready was internaled, <c>false</c> otherwise.</returns>
        /// <param name="placement">Placement.</param>
        protected abstract bool InternalIsRewardedAdReady(AdPlacement placement);

        /// <summary>
        /// Instructs the underlaying SDK to show a rewarded ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected abstract void InternalShowRewardedAd(AdPlacement placement);

        /// <summary>
        /// Occurs when an interstitial ad completed.
        /// </summary>
        public event Action<IAdClient, AdPlacement> InterstitialAdCompleted;

        /// <summary>
        /// Occurs when a rewarded ad is skipped.
        /// </summary>
        public event Action<IAdClient, AdPlacement> RewardedAdSkipped;

        /// <summary>
        /// Occurs when a rewarded ad completed.
        /// </summary>
        public event Action<IAdClient, AdPlacement> RewardedAdCompleted;

        /// <summary>
        /// Gets the associated ad network of this client.
        /// </summary>
        /// <value>The network.</value>
        public abstract AdNetwork Network { get; }

        /// <summary>
        /// Whether banner ads are supported.
        /// </summary>
        /// <value><c>true</c> if banner ads are supported; otherwise, <c>false</c>.</value>
        public abstract bool IsBannerAdSupported { get; }

        /// <summary>
        /// Whether interstitial ads are supported.
        /// </summary>
        /// <value><c>true</c> if interstitial ads are supported; otherwise, <c>false</c>.</value>
        public abstract bool IsInterstitialAdSupported { get; }

        /// <summary>
        /// Whether rewarded ads are supported.
        /// </summary>
        /// <value><c>true</c> if rewarded ads are supported; otherwise, <c>false</c>.</value>
        public abstract bool IsRewardedAdSupported { get; }

        /// <summary>
        /// All the custom interstitial <see cref="AdPlacement"/>(s) defined in <see cref="EM_Settings"/>.
        /// If there's no such custom placement defined, this will return <c>null</c>.
        /// </summary>
        public List<AdPlacement> DefinedCustomInterstitialAdPlacements
        {
            get
            {
                return GetCustomPlacementsFromDefinedDict(CustomInterstitialAdsDict);
            }
        }

        /// <summary>
        /// All the custom rewarded <see cref="AdPlacement"/>(s) defined in <see cref="EM_Settings"/>.
        /// If there's no such custom placement defined, this will return <c>null</c>.
        /// </summary>
        public List<AdPlacement> DefinedCustomRewardedAdPlacements
        {
            get
            {
                return GetCustomPlacementsFromDefinedDict(CustomRewardedAdsDict);
            }
        }

        /// <summary>
        /// Defined interstitial <see cref="AdPlacement"/>(s) in <see cref="EM_Settings"/>.
        /// </summary>
        protected abstract Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict { get; }

        /// <summary>
        /// Defined rewarded <see cref="AdPlacement"/>(s) in <see cref="EM_Settings"/>.
        /// </summary>
        protected abstract Dictionary<AdPlacement, AdId> CustomRewardedAdsDict { get; }

        /// <summary>
        /// Gets a value indicating whether this client is initialized.
        /// </summary>
        /// <value>true</value>
        /// <c>false</c>
        public virtual bool IsInitialized
        {
            get { return mIsInitialized; }
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        public virtual void Init()
        {
            if (IsSdkAvail)
            {
                if (!IsInitialized)
                    InternalInit();
                else
                    Debug.Log(Network.ToString() + " client is already initialized. Ignoring this call.");
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Shows the banner ad at the default placement.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        public virtual void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            ShowBannerAd(AdPlacement.Default, position, size);
        }

        /// <summary>
        /// Shows the banner ad at the specified placement, position and size.
        /// </summary>
        /// <param name="placement">Placement.</param>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        public virtual void ShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
            if (IsSdkAvail)
            {
                if (placement == null)
                {
                    Debug.LogFormat("Cannot show {0} banner ad at placement: null", Network.ToString());
                    return;
                }

                if (size == null)
                {
                    Debug.LogFormat("Cannot show {0} banner ad with ad size: null", Network.ToString());
                    return;
                }

                if (CheckInitialize())
                    InternalShowBannerAd(placement, position, size);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Hides the banner ad at the default placement.
        /// </summary>
        public virtual void HideBannerAd()
        {
            HideBannerAd(AdPlacement.Default);
        }

        /// <summary>
        /// Hides the banner ad at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public virtual void HideBannerAd(AdPlacement placement)
        {
            if (CheckInitialize() && placement != null)
                InternalHideBannerAd(placement);
        }

        /// <summary>
        /// Destroys the banner ad at the default placement.
        /// </summary>
        public virtual void DestroyBannerAd()
        {
            DestroyBannerAd(AdPlacement.Default);
        }

        /// <summary>
        /// Destroys the banner ad at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public virtual void DestroyBannerAd(AdPlacement placement)
        {
            if (CheckInitialize() && placement != null)
                InternalDestroyBannerAd(placement);
        }

        /// <summary>
        /// Loads the interstitial ad at the default placement.
        /// </summary>
        public virtual void LoadInterstitialAd()
        {
            LoadInterstitialAd(AdPlacement.Default);
        }

        /// <summary>
        /// Loads the interstitial ad at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public virtual void LoadInterstitialAd(AdPlacement placement)
        {
            if (IsSdkAvail)
            {
                if (placement == null)
                {
                    Debug.LogFormat("Cannot load {0} interstitial ad at placement: null", Network.ToString());
                    return;
                }

                if (!CheckInitialize())
                    return;

                // Not reloading a loaded ad.
                if (!IsInterstitialAdReady(placement))
                    InternalLoadInterstitialAd(placement);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Determines whether the interstitial ad at the default placement is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        public virtual bool IsInterstitialAdReady()
        {
            return IsInterstitialAdReady(AdPlacement.Default);
        }

        /// <summary>
        /// Determines whether the interstitial ad at the specified placement is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        /// <param name="placement">Placement.</param>
        public virtual bool IsInterstitialAdReady(AdPlacement placement)
        {
            if (CheckInitialize(false) && placement != null)
                return InternalIsInterstitialAdReady(placement);
            else
                return false;
        }

        /// <summary>
        /// Shows the interstitial ad at the specified placement.
        /// </summary>
        public virtual void ShowInterstitialAd()
        {
            ShowInterstitialAd(AdPlacement.Default);
        }

        /// <summary>
        /// Shows the interstitial ad at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public virtual void ShowInterstitialAd(AdPlacement placement)
        {
            if (IsSdkAvail)
            {
                if (placement == null)
                {
                    Debug.LogFormat("Cannot show {0} interstitial ad at placement: null", Network.ToString());
                    return;
                }

                if (!CheckInitialize())
                    return;

                if (!IsInterstitialAdReady(placement))
                {
                    Debug.LogFormat("Cannot show {0} interstitial ad at placement {1}: ad is not loaded.",
                        Network.ToString(),
                        AdPlacement.GetPrintableName(placement));
                    return;
                }

                InternalShowInterstitialAd(placement);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Loads the rewarded ad at the default placement.
        /// </summary>
        public virtual void LoadRewardedAd()
        {
            LoadRewardedAd(AdPlacement.Default);
        }

        /// <summary>
        /// Loads the rewarded ad at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public virtual void LoadRewardedAd(AdPlacement placement)
        {
            if (IsSdkAvail)
            {
                if (placement == null)
                {
                    Debug.LogFormat("Cannot load {0} rewarded ad at placement: null", Network.ToString());
                    return;
                }

                if (!CheckInitialize())
                    return;

                // Not reloading a loaded ad.
                if (!IsRewardedAdReady(placement))
                    InternalLoadRewardedAd(placement);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Determines whether the rewarded ad ready at the default placement is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        public virtual bool IsRewardedAdReady()
        {
            return IsRewardedAdReady(AdPlacement.Default);
        }

        /// <summary>
        /// Determines whether the rewarded ad at the specified placement is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        /// <param name="placement">Placement.</param>
        public virtual bool IsRewardedAdReady(AdPlacement placement)
        {
            if (CheckInitialize(false) && placement != null)
                return InternalIsRewardedAdReady(placement);
            else
                return false;
        }

        /// <summary>
        /// Shows the rewarded ad at the default placement.
        /// </summary>
        public virtual void ShowRewardedAd()
        {
            ShowRewardedAd(AdPlacement.Default);
        }

        /// <summary>
        /// Shows the rewarded ad at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public virtual void ShowRewardedAd(AdPlacement placement)
        {
            if (IsSdkAvail)
            {
                if (placement == null)
                {
                    Debug.LogFormat("Cannot show {0} rewarded ad at placement: null", Network.ToString());
                    return;
                }

                if (!CheckInitialize())
                    return;

                if (!IsRewardedAdReady(placement))
                {
                    Debug.LogFormat("Cannot show {0} rewarded ad at placement {1}: ad is not loaded.",
                        Network.ToString(),
                        AdPlacement.GetPrintableName(placement));
                    return;
                }

                InternalShowRewardedAd(placement);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Raises the <see cref="InterstitialAdCompleted"/> event on main thread.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected virtual void OnInterstitialAdCompleted(AdPlacement placement)
        {
            RuntimeHelper.RunOnMainThread(() =>
            {
                if (InterstitialAdCompleted != null)
                    InterstitialAdCompleted(this, placement);
            });
        }

        /// <summary>
        /// Raises the <see cref="RewardedAdSkipped"/> event on main thread.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected virtual void OnRewardedAdSkipped(AdPlacement placement)
        {
            RuntimeHelper.RunOnMainThread(() =>
            {
                if (RewardedAdSkipped != null)
                    RewardedAdSkipped(this, placement);
            });
        }

        /// <summary>
        /// Raises the <see cref="RewardedAdCompleted"/> event on main thread.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected virtual void OnRewardedAdCompleted(AdPlacement placement)
        {
            RuntimeHelper.RunOnMainThread(() =>
            {
                if (RewardedAdCompleted != null)
                    RewardedAdCompleted(this, placement);
            });
        }

        /// <summary>
        /// Returns the cross-platform ID associated with placement in the given dictionary.
        /// Returns <c>string.Empty</c> if no such ID found.
        /// </summary>
        /// <returns>The identifier for placement.</returns>
        /// <param name="dict">Dict.</param>
        /// <param name="placement">Placement.</param>
        protected virtual string FindIdForPlacement(Dictionary<AdPlacement, AdId> dict, AdPlacement placement)
        {
            AdId idObj = null;
            if (placement != null && dict != null)
            {
                dict.TryGetValue(placement, out idObj);
            }

            if (idObj != null && !string.IsNullOrEmpty(idObj.Id))
            {
                return idObj.Id;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks if the client is initialized and print a warning message if not.
        /// </summary>
        /// <returns><c>true</c>, if initialize was checked, <c>false</c> otherwise.</returns>
        protected virtual bool CheckInitialize(bool logMessage = true)
        {
            if (Network == AdNetwork.None)
                return false;

            bool isInit = IsInitialized;

            if (!isInit && logMessage)
                Debug.Log("Please initialize the " + Network.ToString() + " client first.");

            return isInit;
        }

        /// <summary>
        /// Return a list of <see cref="AdPlacement"/>(s) from the dictionary defined in <see cref="EM_Settings"/>.
        /// </summary>
        protected List<AdPlacement> GetCustomPlacementsFromDefinedDict(Dictionary<AdPlacement, AdId> dict)
        {
            if (dict == null || dict.Count < 1)
                return null;

            List<AdPlacement> definedPlacements = new List<AdPlacement>();
            foreach (var pair in dict)
            {
                var placement = pair.Key;
                if (placement != null && placement != AdPlacement.Default && !definedPlacements.Contains(placement))
                    definedPlacements.Add(placement);
            }
            return definedPlacements;
        }

        #endregion

        #region IConsentRequirable Implementation

        /// <summary>
        /// Raised when the data privacy consent of the associated ad network is changed.
        /// </summary>
        public event Action<ConsentStatus> DataPrivacyConsentUpdated;

        /// <summary>
        /// The data privacy consent status of the associated ad network, 
        /// default to ConsentStatus.Unknown. 
        /// </summary>
        public virtual ConsentStatus DataPrivacyConsent
        {
            get
            {
                return ReadDataPrivacyConsent();
            }
            private set
            {
                if (DataPrivacyConsent != value)
                {
                    // Store new consent to the persistent storage.
                    SaveDataPrivacyConsent(value);

                    // Configure the client with new consent.
                    // This is the most local consent (highest priority) for this client 
                    // so we just use it without having to consult GetApplicableDataPrivacyConsent.
                    ApplyDataPrivacyConsent(value);

                    // Raise event.
                    if (DataPrivacyConsentUpdated != null)
                        DataPrivacyConsentUpdated(value);
                }
            }
        }

        /// <summary>
        /// Grants provider-level data privacy consent for the associated ad network.
        /// This consent persists across app launches.
        /// </summary>
        public virtual void GrantDataPrivacyConsent()
        {
            DataPrivacyConsent = ConsentStatus.Granted;
        }

        /// <summary>
        /// Revokes the provider-level data privacy consent of the associated ad network.
        /// This consent persists across app launches.
        /// </summary>
        public virtual void RevokeDataPrivacyConsent()
        {
            DataPrivacyConsent = ConsentStatus.Revoked;
        }

        protected abstract string DataPrivacyConsentSaveKey { get; }

        /// <summary>
        /// Reads the data privacy consent from persistent storage.
        /// </summary>
        /// <returns>The data privacy consent.</returns>
        protected virtual ConsentStatus ReadDataPrivacyConsent()
        {
            return ConsentStorage.ReadConsent(DataPrivacyConsentSaveKey);
        }

        /// <summary>
        /// Saves the data privacy consent to persistent storage.
        /// </summary>
        /// <param name="consent">Consent.</param>
        protected virtual void SaveDataPrivacyConsent(ConsentStatus consent)
        {
            ConsentStorage.SaveConsent(DataPrivacyConsentSaveKey, consent);
        }

        /// <summary>
        /// Finds the applicable data privacy consent for this client by searching
        /// upward from provider-level consent to global-level consent (high priority -> low priority).
        /// </summary>
        /// <returns>The applicable data privacy consent.</returns>
        protected virtual ConsentStatus GetApplicableDataPrivacyConsent()
        {
            // Provider-level consent.
            if (DataPrivacyConsent != ConsentStatus.Unknown)
                return DataPrivacyConsent;
            // Module-level consent.
            else if (AdvertisingConsentManager.Instance.DataPrivacyConsent != ConsentStatus.Unknown)
                return AdvertisingConsentManager.Instance.DataPrivacyConsent;
            // Global-level consent.
            else
                return GlobalConsentManager.Instance.DataPrivacyConsent;
        }

        /// <summary>
        /// Implement this method and perform whatever actions needed
        /// for the client to response to the specified data privacy consent.
        /// This is intended to be used with the consent obtained from
        /// <see cref="GetApplicableDataPrivacyConsent"/>.
        /// </summary>
        /// <param name="consent">Consent.</param>
        protected abstract void ApplyDataPrivacyConsent(ConsentStatus consent);

        #endregion

    }
}