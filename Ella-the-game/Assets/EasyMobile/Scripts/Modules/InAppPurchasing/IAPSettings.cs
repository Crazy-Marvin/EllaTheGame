using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public class IAPSettings
    {
        /// <summary>
        /// Whether the In-App Purchasing module should initialize itself automatically.
        /// </summary>
        /// <value><c>true</c> if is auto init; otherwise, <c>false</c>.</value>
        public bool IsAutoInit
        {
            get { return mAutoInit; }
            set { mAutoInit = value; }
        }

        /// <summary>
        /// Gets the target Android store. This value can be set
        /// in the settings UI of the In-App Purchasing module.
        /// </summary>
        /// <value>The target android store.</value>
        public IAPAndroidStore TargetAndroidStore
        {
            get { return mTargetAndroidStore; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Apple's Ask-To-Buy 
        /// simulation in the sandbox app store should be enabled or disabled.
        /// This value is used during the initialization process so any changes made to it after
        /// the module has been initialized won't have any effect.
        /// On non-Apple platforms this value is ignored.
        /// </summary>
        /// <value><c>true</c> if simulate apple ask to buy; otherwise, <c>false</c>.</value>
        public bool SimulateAppleAskToBuy
        {
            get { return mSimulateAppleAskToBuy; }
            set { mSimulateAppleAskToBuy = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we should intercept Apple promotional purchases.
        /// This value is used during the initialization process so any changes made to it after
        /// the module has been initialized won't have any effect.
        /// On non-Apple platforms this value is ignored.
        /// </summary>
        /// <value><c>true</c> if intercept Apple promotional purchases; otherwise, <c>false</c>.</value>
        public bool InterceptApplePromotionalPurchases
        {
            get { return mInterceptApplePromotionalPurchases; }
            set { mInterceptApplePromotionalPurchases = value; }
        }

        /// <summary>
        /// Gets a value indicating Amazon sandbox testing mode is enabled.
        /// This value is used during the initialization process so any changes made to it after
        /// the module has been initialized won't have any effect until the next initialization.
        /// On non-Amazon platforms this value is ignored.
        /// </summary>
        /// <value><c>true</c> if enable Amazon sandbox testing; otherwise, <c>false</c>.</value>
        public bool EnableAmazonSandboxTesting
        {
            get { return mEnableAmazonSandboxTesting; }
            set { mEnableAmazonSandboxTesting = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Apple receipts should be validated while processing purchases.
        /// </summary>
        /// <value><c>true</c> if validate Apple receipt; otherwise, <c>false</c>.</value>
        public bool ValidateAppleReceipt
        {
            get { return mValidateAppleReceipt; }
            set { mValidateAppleReceipt = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Google Play receipts should be validated while processing purchases.
        /// </summary>
        /// <value><c>true</c> if validate Google Play receipt; otherwise, <c>false</c>.</value>
        public bool ValidateGooglePlayReceipt
        {
            get { return mValidateGooglePlayReceipt; }
            set { mValidateGooglePlayReceipt = value; }
        }

        /// <summary>
        /// Gets or sets the product catalog.
        /// </summary>
        /// <value>The products.</value>
        public IAPProduct[] Products
        {
            get { return mProducts; }
            set { mProducts = value; }
        }

        [SerializeField]
        private bool mAutoInit = true;
        [SerializeField]
        private IAPAndroidStore mTargetAndroidStore = IAPAndroidStore.GooglePlay;
        [SerializeField]
        private bool mSimulateAppleAskToBuy = false;
        [SerializeField]
        private bool mInterceptApplePromotionalPurchases = false;
        [SerializeField]
        private bool mEnableAmazonSandboxTesting = false;
        [SerializeField]
        private bool mValidateAppleReceipt = true;
        [SerializeField]
        private bool mValidateGooglePlayReceipt = true;
        [SerializeField]
        private IAPProduct[] mProducts;
    }
}
