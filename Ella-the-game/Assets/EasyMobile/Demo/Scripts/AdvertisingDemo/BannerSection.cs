using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;

namespace EasyMobile.Demo
{
    [Serializable]
    public class BannerSection : AdsSection
    {
        [Serializable]
        private class DefaulBannerUI
        {
            [SerializeField]
            protected Button showBannerButton = null, hideBannerButton = null, destroyBannerButton = null;

            [SerializeField]
            protected Dropdown bannerPositionSelector = null, bannerSizeSelector = null;

            [SerializeField]
            private List<BannerAdSize> allBannerSizes;
            private List<BannerAdPosition> allBannerPositions;

            public BannerAdSize SelectedBannerSize { get { return allBannerSizes[bannerSizeSelector.value]; } }

            public BannerAdPosition SelectedBannerPosition { get { return allBannerPositions[bannerPositionSelector.value]; } }

            public virtual void Start()
            {
                InitBannerSizesDropdown();
                InitBannerPositionDropdown();
                InitButtons();
            }

            protected void InitBannerPositionDropdown()
            {
                allBannerPositions = new List<BannerAdPosition>();
                List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();

                foreach (BannerAdPosition position in Enum.GetValues(typeof(BannerAdPosition)))
                {
                    allBannerPositions.Add(position);
                    optionDatas.Add(new Dropdown.OptionData(position.ToString()));
                }

                bannerPositionSelector.ClearOptions();
                bannerPositionSelector.AddOptions(optionDatas);

                // Default banner position as Bottom.
                bannerPositionSelector.value = allBannerPositions.IndexOf(BannerAdPosition.Bottom);
            }

            protected void InitBannerSizesDropdown()
            {
                allBannerSizes = new List<BannerAdSize>
                {
                    BannerAdSize.Banner,
                    BannerAdSize.IABBanner,
                    BannerAdSize.Leaderboard,
                    BannerAdSize.MediumRectangle,
                    BannerAdSize.SmartBanner
                };

                bannerSizeSelector.ClearOptions();
                bannerSizeSelector.AddOptions(new List<Dropdown.OptionData>
                    {
                        new Dropdown.OptionData("Banner"),
                        new Dropdown.OptionData("IAB Banner"),
                        new Dropdown.OptionData("Leaderboard"),
                        new Dropdown.OptionData("Medium Rectangle"),
                        new Dropdown.OptionData("Smart Banner")
                    });

                // Default size as SmartBanner.
                bannerSizeSelector.value = allBannerSizes.IndexOf(BannerAdSize.SmartBanner);
            }

            protected void InitButtons()
            {
                showBannerButton.onClick.AddListener(ShowBanner);
                hideBannerButton.onClick.AddListener(HideBanner);
                destroyBannerButton.onClick.AddListener(DestroyBanner);
            }

            protected virtual void ShowBanner()
            {
                if (Advertising.IsAdRemoved())
                {
                    NativeUI.Alert("Alert", "Ads were removed.");
                    return;
                }

                Advertising.ShowBannerAd(SelectedBannerPosition, SelectedBannerSize);
            }

            protected virtual void HideBanner()
            {
                Advertising.HideBannerAd();
            }

            protected virtual void DestroyBanner()
            {
                Advertising.DestroyBannerAd();
            }
        }

        [Serializable]
        private class CustomBannerUI : DefaulBannerUI
        {
            [SerializeField]
            private Dropdown networkSelector = null;

            [SerializeField]
            private InputField customKeyInputField = null;

            private List<BannerAdNetwork> allBannerNetworks = null;

            public BannerAdNetwork SelectedNetwork { get { return allBannerNetworks[networkSelector.value]; } }

            public override void Start()
            {
                base.Start();
                InitBannerNetworkDropdown();
            }

            private void InitBannerNetworkDropdown()
            {
                allBannerNetworks = new List<BannerAdNetwork>();
                List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();

                foreach (BannerAdNetwork network in Enum.GetValues(typeof(BannerAdNetwork)))
                {
                    allBannerNetworks.Add(network);
                    optionDatas.Add(new Dropdown.OptionData(network.ToString()));
                }

                networkSelector.ClearOptions();
                networkSelector.AddOptions(optionDatas);
            }

            protected override void ShowBanner()
            {
                if (Advertising.IsAdRemoved())
                {
                    NativeUI.Alert("Alert", "Ads were removed.");
                    return;
                }

                Advertising.ShowBannerAd(
                    SelectedNetwork,
                    AdPlacement.PlacementWithName(customKeyInputField.text),
                    SelectedBannerPosition,
                    SelectedBannerSize);
            }

            protected override void HideBanner()
            {
                Advertising.HideBannerAd(
                    SelectedNetwork,
                    AdPlacement.PlacementWithName(customKeyInputField.text));
            }

            protected override void DestroyBanner()
            {
                Advertising.DestroyBannerAd(
                    SelectedNetwork,
                    AdPlacement.PlacementWithName(customKeyInputField.text));
            }
        }

        [SerializeField]
        private DefaulBannerUI defaultBannerUI = null;

        [SerializeField]
        private CustomBannerUI customBannerUI = null;

        public override void Start()
        {
            base.Start();
            defaultBannerUI.Start();
            customBannerUI.Start();
        }
    }
}
