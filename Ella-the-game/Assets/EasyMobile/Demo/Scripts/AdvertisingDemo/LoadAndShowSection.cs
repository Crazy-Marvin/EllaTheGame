using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    /// <summary>
    /// Base class for interstitial and rewarded video UI sections.
    /// </summary>
    /// <typeparam name="T">Type of the default element inner class.</typeparam>
    /// <typeparam name="U">Type of the custom element inner class.</typeparam>
    [Serializable]
    public class LoadAndShowSection<T, U> : AdsSection where T : LoadAndShowSection<T, U>.DefaultElement
                                                       where U : LoadAndShowSection<T, U>.CustomElement
    {
        [Serializable]
        public abstract class DefaultElement
        {
            [SerializeField]
            protected Button loadAdButton, showAdButton;

            [SerializeField]
            protected GameObject isAdLoadedToggle;

            protected DemoUtils DemoUtils { get; private set; }

            protected string LoadedStatusMessage { get { return IsAdReady() ? AdReadyMessage : AdNotReadyMessage; } }

            public virtual void Start(DemoUtils demoUtils)
            {
                DemoUtils = demoUtils;

                /// Set up buttons event.
                loadAdButton.onClick.AddListener(LoadAd);
                showAdButton.onClick.AddListener(ShowAd);
            }

            public void Update()
            {
                /// Check and display the ad loaded status.
                DemoUtils.DisplayBool(isAdLoadedToggle, IsAdReady(), LoadedStatusMessage);
            }

            protected void ShowAdIfAvailable()
            {
                if (Advertising.IsAdRemoved())
                {
                    NativeUI.Alert("Alert", "Ads were removed.");
                    return;
                }

                if (IsAdReady())
                {
                    ShowAd();
                }
                else
                {
                    NativeUI.Alert("Alert", UnavailableAdAlertMessage);
                }
            }

            protected abstract void LoadAd();

            protected abstract void ShowAd();

            protected abstract bool IsAdReady();

            protected abstract string AdReadyMessage { get; }

            protected abstract string AdNotReadyMessage { get; }

            protected abstract string UnavailableAdAlertMessage { get; }
        }

        [Serializable]
        public abstract class CustomElement : DefaultElement
        {
            [SerializeField]
            protected Dropdown networkSelector;

            [SerializeField]
            protected GameObject inputFieldRoot;

            [SerializeField]
            protected InputField customKeyInputField;

            [SerializeField]
            protected Toggle enableCustomKey;

            protected string CustomKey
            {
                get
                {
                    /// If custom has been enabled, we return the value in the input field,
                    /// otherwise return an empty string so the default placement can be called.
                    return enableCustomKey.isOn ? customKeyInputField.text : null;
                }
            }

            public override void Start(DemoUtils demoUtils)
            {
                base.Start(demoUtils);
                InitNetworkDropdown();
                InitEnableCustomKeyToggle();
            }

            private void InitEnableCustomKeyToggle()
            {
                enableCustomKey.onValueChanged.AddListener(flag =>
                    {
                        inputFieldRoot.gameObject.SetActive(flag);
                    });
            }

            protected abstract void InitNetworkDropdown();
        }

        [SerializeField]
        private T defaultElement = null;

        [SerializeField]
        private U customElement = null;

        public virtual void Start(DemoUtils demoUtils)
        {
            base.Start();
            defaultElement.Start(demoUtils);
            customElement.Start(demoUtils);
        }

        public virtual void Update()
        {
            defaultElement.Update();
            customElement.Update();
        }
    }
}
