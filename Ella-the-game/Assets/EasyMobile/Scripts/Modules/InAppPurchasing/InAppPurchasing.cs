using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if EM_UIAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

namespace EasyMobile
{
    [AddComponentMenu("")]
    public class InAppPurchasing : MonoBehaviour
    {
        public static InAppPurchasing Instance { get; private set; }

        public const string PROCESSING_PURCHASE_ABORT = "PROCESSING_PURCHASE_ABORT";
        public const string PROCESSING_PURCHASE_INVALID_RECEIPT = "PROCESSING_PURCHASE_INVALID_RECEIPT";
        public const string CONFIRM_PENDING_PURCHASE_FAILED = "CONFIRM_PENDING_PURCHASE_FAILED";

        // Suppress the "Event is never used" warnings.
#pragma warning disable 0067

        /// <summary>
        /// Occurs when the module is initialized successfully and ready to work.
        /// </summary>
        public static event Action InitializeSucceeded;

        /// <summary>
        /// Occurs when the module failed to initialize.
        /// </summary>
        public static event Action InitializeFailed;

        /// <summary>
        /// Occurs when a purchase is completed.
        /// </summary>
        public static event Action<IAPProduct> PurchaseCompleted;

        /// <summary>
        /// Occurs when a purchase failed.
        /// </summary>
        public static event Action<IAPProduct, string> PurchaseFailed;

        /// <summary>
        /// [Apple store only] Occurs as part of Apple's 'Ask to buy' functionality,
        /// when a purchase is requested by a minor and referred to a parent for approval.
        /// When the purchase is approved or rejected, the normal purchase events, 
        /// <see cref="PurchaseCompleted"/> or <see cref="PurchaseFailed"/>, will fire.
        /// On non-Apple platforms this event will never fire.
        /// </summary>
        public static event Action<IAPProduct> PurchaseDeferred;

        /// <summary>
        /// [Apple store only] Occurs when a promotional purchase that comes directly
        /// from the App Store is intercepted. This event only fires if the
        /// <see cref="IAPSettings.InterceptApplePromotionalPurchases"/> setting is <c>true</c>.
        /// On non-Apple platforms this event will never fire.
        /// </summary>
        public static event Action<IAPProduct> PromotionalPurchaseIntercepted;

        /// <summary>
        /// [Apple store only] Occurs when the (non-consumable and subscription) 
        /// purchases were restored successfully.
        /// On non-Apple platforms this event will never fire.
        /// </summary>
        public static event Action RestoreCompleted;

        /// <summary>
        /// [Apple store only] Occurs when the purchase restoration failed.
        /// On non-Apple platforms this event will never fire.
        /// </summary>
        public static event Action RestoreFailed;

#pragma warning restore 0067

#if EM_UIAP
        /// <summary>
        /// The underlying UnityIAP's ConfigurationBuilder used in this module.
        /// </summary>
        /// <value>The builder.</value>
        public static ConfigurationBuilder Builder { get { return sBuilder; } }

        /// <summary>
        /// The underlying UnityIAP's IStoreController used in this module.
        /// </summary>
        /// <value>The store controller.</value>
        public static IStoreController StoreController { get { return sStoreController; } }

        /// <summary>
        /// The underlying UnityIAP's IExtensionProvider used in this module. Use it to access
        /// store-specific extended functionalities.
        /// </summary>
        /// <value>The store extension provider.</value>
        public static IExtensionProvider StoreExtensionProvider { get { return sStoreExtensionProvider; } }

        /// <summary>
        /// Gets the Apple store extensions.
        /// </summary>
        /// <value>The apple store extensions.</value>
        public static IAppleExtensions AppleStoreExtensions { get { return sAppleExtensions; } }

        /// <summary>
        /// Gets the Google Play store extensions.
        /// </summary>
        /// <value>The google play store extensions.</value>
        public static IGooglePlayStoreExtensions GooglePlayStoreExtensions { get { return sGooglePlayStoreExtensions; } }

        /// <summary>
        /// Gets the Amazon store extensions.
        /// </summary>
        /// <value>The amazon store extensions.</value>
        public static IAmazonExtensions AmazonStoreExtensions { get { return sAmazonExtensions; } }

        /// <summary>
        /// Gets the Samsung Apps store extensions.
        /// </summary>
        /// <value>The samsung apps store extensions.</value>
        public static ISamsungAppsExtensions SamsungAppsStoreExtensions { get { return sSamsungAppsExtensions; } }

        // The ConfigurationBuilder
        private static ConfigurationBuilder sBuilder;

        // The Unity Purchasing system
        private static IStoreController sStoreController;

        // The store-specific Purchasing subsystems
        private static IExtensionProvider sStoreExtensionProvider;

        // The Apple store extensions.
        private static IAppleExtensions sAppleExtensions;

        // The Google Play store extensions.
        private static IGooglePlayStoreExtensions sGooglePlayStoreExtensions;

        // The Amazon store extensions.
        private static IAmazonExtensions sAmazonExtensions;

        // The Samsung Apps store extensions.
        private static ISamsungAppsExtensions sSamsungAppsExtensions;

        // Store listener to handle purchasing events
        private static StoreListener sStoreListener = new StoreListener();

        private static bool sIsInitializing = false;

#endif

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
#if EM_UIAP
            // Perform automatic initialization if configured so.
            if (EM_Settings.InAppPurchasing.IsAutoInit)
            {
                InitializePurchasing();
            }
#endif
        }

        /// <summary>
        /// Initializes the in-app purchasing service.
        /// </summary>
        public static void InitializePurchasing()
        {
#if EM_UIAP
            if (IsInitialized() || sIsInitializing)
            {
                return;
            }

            // Start initializing.
            sIsInitializing = true;

            // Create a builder, first passing in a suite of Unity provided stores.
            sBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add products
            foreach (IAPProduct pd in EM_Settings.InAppPurchasing.Products)
            {
                if (pd.StoreSpecificIds != null && pd.StoreSpecificIds.Length > 0)
                {
                    // Add store-specific id if any
                    IDs storeIDs = new IDs();

                    foreach (IAPProduct.StoreSpecificId sId in pd.StoreSpecificIds)
                    {
                        storeIDs.Add(sId.id, new string[] { GetStoreName(sId.store) });
                    }

                    // Add product with store-specific ids
                    sBuilder.AddProduct(pd.Id, GetProductType(pd.Type), storeIDs);
                }
                else
                {
                    // Add product using store-independent id
                    sBuilder.AddProduct(pd.Id, GetProductType(pd.Type));
                }
            }

            // Intercepting Apple promotional purchases if needed.
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (EM_Settings.InAppPurchasing.InterceptApplePromotionalPurchases)
                    sBuilder.Configure<IAppleConfiguration>().SetApplePromotionalPurchaseInterceptorCallback(OnApplePromotionalPurchase);
            }

            // If current store is Amazon and sandbox testing is enable:
            // write a product description to the SD card in the appropriate location.
            if (Application.platform == RuntimePlatform.Android)
            {
                if (EM_Settings.InAppPurchasing.TargetAndroidStore == IAPAndroidStore.AmazonAppStore &&
                    EM_Settings.InAppPurchasing.EnableAmazonSandboxTesting)
                {
                    sBuilder.Configure<IAmazonConfiguration>().WriteSandboxJSON(sBuilder.products);
                }
            }

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(sStoreListener, sBuilder);
#else
            Debug.Log("InitializePurchasing FAILED: IAP module is not enabled.");
#endif
        }

        /// <summary>
        /// Determines whether UnityIAP is initialized. All further actions like purchasing
        /// or restoring can only be done if UnityIAP is initialized.
        /// </summary>
        /// <returns><c>true</c> if initialized; otherwise, <c>false</c>.</returns>
        public static bool IsInitialized()
        {
#if EM_UIAP
            // Only say we are initialized if both the Purchasing references are set.
            return sStoreController != null && sStoreExtensionProvider != null;
#else
            return false;
#endif
        }

        /// <summary>
        /// Purchases the specified product.
        /// </summary>
        /// <param name="product">Product.</param>
        public static void Purchase(IAPProduct product)
        {
            if (product != null && product.Id != null)
            {
                PurchaseWithId(product.Id);
            }
            else
            {
                Debug.Log("IAP purchasing failed: product or product ID is null.");
            }
        }

        /// <summary>
        /// Purchases the product with specified name.
        /// </summary>
        /// <param name="productName">Product name.</param>
        public static void Purchase(string productName)
        {
            IAPProduct pd = GetIAPProductByName(productName);

            if (pd != null && pd.Id != null)
            {
                PurchaseWithId(pd.Id);
            }
            else
            {
                Debug.Log("IAP purchasing failed: Not found product with name: " + productName + " or its ID is invalid.");
            }
        }

        /// <summary>
        /// Purchases the product with specified ID.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        public static void PurchaseWithId(string productId)
        {
#if EM_UIAP
            if (IsInitialized())
            {
                Product product = sStoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log("Purchasing product asychronously: " + product.definition.id);

                    // Buy the product, expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    sStoreController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("IAP purchasing failed: product not found or not available for purchase.");
                }
            }
            else
            {
                // Purchasing has not succeeded initializing yet.
                Debug.Log("IAP purchasing failed: In-App Purchasing is not initialized.");
            }
#else
            Debug.Log("IAP purchasing failed: In-App Purchasing module is not enabled.");
#endif
        }

        /// <summary>
        /// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google Play.
        /// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        /// This method only has effect on iOS and MacOSX apps.
        /// </summary>
        public static void RestorePurchases()
        {
#if EM_UIAP
            if (!IsInitialized())
            {
                Debug.Log("Couldn't restore IAP purchases: In-App Purchasing is not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // Fetch the Apple store-specific subsystem.
                var apple = sStoreExtensionProvider.GetExtension<IAppleExtensions>();

                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                    {
                        // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                        // no purchases are available to be restored.
                        Debug.Log("Restoring IAP purchases result: " + result);

                        if (result)
                        {
                            // Fire restore complete event.
                            if (RestoreCompleted != null)
                                RestoreCompleted();
                        }
                        else
                        {
                            // Fire event failed event.
                            if (RestoreFailed != null)
                                RestoreFailed();
                        }
                    });
            }
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("Couldn't restore IAP purchases: not supported on platform " + Application.platform.ToString());
            }
#else
            Debug.Log("Couldn't restore IAP purchases: In-App Purchasing module is not enabled.");
#endif
        }

        /// <summary>
        /// Determines whether the product with the specified name is owned.
        /// A product is consider owned if it has a receipt. If receipt validation
        /// is enabled, it is also required that this receipt passes the validation check.
        /// Note that this method is mostly useful with non-consumable products.
        /// Consumable products' receipts are not persisted between app restarts,
        /// therefore their ownership only pertains in the session they're purchased.
        /// In the case of subscription products, this method only checks if a product has been purchased before,
        /// it doesn't check if the subscription has been expired or canceled. 
        /// </summary>
        /// <returns><c>true</c> if the product has a receipt and that receipt is valid (if receipt validation is enabled); otherwise, <c>false</c>.</returns>
        /// <param name="productName">Product name.</param>
        public static bool IsProductOwned(string productName)
        {
#if EM_UIAP
            IAPProduct iapProduct = GetIAPProductByName(productName);
            return IsProductOwned(iapProduct);
#else
            return false;
#endif
        }
        
        /// <summary>
        /// Determines whether the product with the specified id is owned.
        /// A product is consider owned if it has a receipt. If receipt validation
        /// is enabled, it is also required that this receipt passes the validation check.
        /// Note that this method is mostly useful with non-consumable products.
        /// Consumable products' receipts are not persisted between app restarts,
        /// therefore their ownership only pertains in the session they're purchased.
        /// In the case of subscription products, this method only checks if a product has been purchased before,
        /// it doesn't check if the subscription has been expired or canceled. 
        /// </summary>
        /// <returns><c>true</c> if the product has a receipt and that receipt is valid (if receipt validation is enabled); otherwise, <c>false</c>.</returns>
        /// <param name="productId">Product id.</param>
        public static bool IsProductWithIdOwned(string productId)
        {
#if EM_UIAP
            IAPProduct iapProduct = GetIAPProductById(productId);
            return IsProductOwned(iapProduct);
#else
            return false;
#endif
        }
        
        /// <summary>
        /// Determines whether the product is owned.
        /// A product is consider owned if it has a receipt. If receipt validation
        /// is enabled, it is also required that this receipt passes the validation check.
        /// Note that this method is mostly useful with non-consumable products.
        /// Consumable products' receipts are not persisted between app restarts,
        /// therefore their ownership only pertains in the session they're purchased.
        /// In the case of subscription products, this method only checks if a product has been purchased before,
        /// it doesn't check if the subscription has been expired or canceled. 
        /// </summary>
        /// <returns><c>true</c> if the product has a receipt and that receipt is valid (if receipt validation is enabled); otherwise, <c>false</c>.</returns>
        /// <param name="product">the product.</param>
        public static bool IsProductOwned(IAPProduct product)
        {
#if EM_UIAP
            if (!IsInitialized())
                return false;
            
            if (product == null)
                return false;

            Product pd = sStoreController.products.WithID(product.Id);

            if (pd.hasReceipt)
            {
                bool isValid = true; // presume validity if not validate receipt.

                if (IsReceiptValidationEnabled())
                {
                    IPurchaseReceipt[] purchaseReceipts;
                    isValid = ValidateReceipt(pd.receipt, out purchaseReceipts);
                }

                return isValid;
            }
            else
            {
                return false;
            }

#else
            return false;
#endif
        }

        /// <summary>
        /// Fetches a new Apple App receipt from their servers.
        /// Note that this will prompt the user for their password.
        /// </summary>
        /// <param name="successCallback">Success callback.</param>
        /// <param name="errorCallback">Error callback.</param>
        public static void RefreshAppleAppReceipt(Action<string> successCallback, Action errorCallback)
        {
#if EM_UIAP
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Refreshing Apple app receipt is only available on iOS.");
                return;
            }

            if (!IsInitialized())
            {
                Debug.Log("Couldn't refresh Apple app receipt: In-App Purchasing is not initialized.");
                return;
            }

            sStoreExtensionProvider.GetExtension<IAppleExtensions>().RefreshAppReceipt(
                (receipt) =>
                {
                    // This handler is invoked if the request is successful.
                    // Receipt will be the latest app receipt.
                    if (successCallback != null)
                        successCallback(receipt);
                },
                () =>
                {
                    // This handler will be invoked if the request fails,
                    // such as if the network is unavailable or the user
                    // enters the wrong password.
                    if (errorCallback != null)
                        errorCallback();
                });

#else
            Debug.Log("Couldn't refresh Apple app receipt: In-App Purchasing module is not enabled.");
            return;
#endif
        }

        /// <summary>
        /// Enables or disables the Apple's Ask-To-Buy simulation in the sandbox app store.
        /// Call this after the module has been initialized to toggle the simulation, regardless
        /// of the <see cref="IAPSettings.SimulateAppleAskToBuy"/> setting.
        /// </summary>
        /// <param name="shouldSimulate">If set to <c>true</c> should simulate.</param>
        public static void SetSimulateAppleAskToBuy(bool shouldSimulate)
        {
#if EM_UIAP
            if (sAppleExtensions != null)
                sAppleExtensions.simulateAskToBuy = shouldSimulate;
#else
            Debug.Log("Couldn't set simulate Apple's Ask to Buy: In-App Purchasing module is not enabled.");
#endif
        }

        /// <summary>
        /// Continues the Apple promotional purchases. Call this inside the handler of
        /// the <see cref="PromotionalPurchaseIntercepted"/> event to continue the
        /// intercepted purchase.
        /// </summary>
        public static void ContinueApplePromotionalPurchases()
        {
#if EM_UIAP
            if (sAppleExtensions != null)
                sAppleExtensions.ContinuePromotionalPurchases(); // iOS and tvOS only; does nothing on Mac
#else
            Debug.Log("Couldn't continue Apple promotional purchase: In-App Purchasing module is not enabled.");
#endif
        }

        /// <summary>
        /// Sets the Apple store promotion visibility for the specified product on the current device.
        /// Call this inside the handler of the <see cref="InitializeSucceeded"/> event to set
        /// the visibility for a promotional product on Apple App Store.
        /// On non-Apple platforms this method is a no-op.
        /// </summary>
        /// <param name="productName">Product name.</param>
        /// <param name="visible">If set to <c>true</c> the product is shown, otherwise it is hidden.</param>
        public static void SetAppleStorePromotionVisibility(string productName, bool visible)
        {
#if EM_UIAP
            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't set promotion visibility: not found product with name: " + productName);
                return;
            }
            
            SetAppleStorePromotionVisibility(iapProduct, visible);
#else
            Debug.Log("Couldn't set Apple store promotion visibility: In-App Purchasing module is not enabled.");
#endif
        }
        
        /// <summary>
        /// Sets the Apple store promotion visibility for the specified product on the current device.
        /// Call this inside the handler of the <see cref="InitializeSucceeded"/> event to set
        /// the visibility for a promotional product on Apple App Store.
        /// On non-Apple platforms this method is a no-op.
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <param name="visible">If set to <c>true</c> the product is shown, otherwise it is hidden.</param>
        public static void SetAppleStorePromotionVisibilityWithId(string productId, bool visible)
        {
#if EM_UIAP
            IAPProduct iapProduct = GetIAPProductById(productId);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't set promotion visibility: not found product with id: " + productId);
                return;
            }
            
            SetAppleStorePromotionVisibility(iapProduct, visible);
#else
            Debug.Log("Couldn't set Apple store promotion visibility: In-App Purchasing module is not enabled.");
#endif
        }
        
        /// <summary>
        /// Sets the Apple store promotion visibility for the specified product on the current device.
        /// Call this inside the handler of the <see cref="InitializeSucceeded"/> event to set
        /// the visibility for a promotional product on Apple App Store.
        /// On non-Apple platforms this method is a no-op.
        /// </summary>
        /// <param name="product">the product.</param>
        /// <param name="visible">If set to <c>true</c> the product is shown, otherwise it is hidden.</param>
        public static void SetAppleStorePromotionVisibility(IAPProduct product, bool visible)
        {
#if EM_UIAP
            if (!IsInitialized())
            {
                Debug.Log("Couldn't set promotion visibility: In-App Purchasing is not initialized.");
                return;
            }
            
            if (product == null)
                return;

            Product prod = sStoreController.products.WithID(product.Id);

            if (sAppleExtensions != null)
            {
                sAppleExtensions.SetStorePromotionVisibility(prod,
                    visible ? AppleStorePromotionVisibility.Show : AppleStorePromotionVisibility.Hide);
            }
#else
            Debug.Log("Couldn't set Apple store promotion visibility: In-App Purchasing module is not enabled.");
#endif
        }

        /// <summary>
        /// Sets the Apple store promotion order on the current device.
        /// Call this inside the handler of the <see cref="InitializeSucceeded"/> event to set
        /// the order for your promotional products on Apple App Store.
        /// On non-Apple platforms this method is a no-op.
        /// </summary>
        /// <param name="products">Products.</param>
        public static void SetAppleStorePromotionOrder(List<IAPProduct> products)
        {
#if EM_UIAP
            if (!IsInitialized())
            {
                Debug.Log("Couldn't set promotion order: In-App Purchasing is not initialized.");
                return;
            }

            var items = new List<Product>();

            for (int i = 0; i < products.Count; i++)
            {
                if (products[i] != null)
                    items.Add(sStoreController.products.WithID(products[i].Id));
            }

            if (sAppleExtensions != null)
                sAppleExtensions.SetStorePromotionOrder(items);
#else
            Debug.Log("Couldn't set Apple store promotion order: In-App Purchasing module is not enabled.");
#endif
        }

        /// <summary>
        /// Gets all IAP products declared in module settings.
        /// </summary>
        /// <returns>The all IAP products.</returns>
        public static IAPProduct[] GetAllIAPProducts()
        {
            return EM_Settings.InAppPurchasing.Products;
        }

        /// <summary>
        /// Gets the IAP product declared in module settings with the specified name.
        /// </summary>
        /// <returns>The IAP product.</returns>
        /// <param name="productName">Product name.</param>
        public static IAPProduct GetIAPProductByName(string productName)
        {
            foreach (IAPProduct pd in EM_Settings.InAppPurchasing.Products)
            {
                if (pd.Name.Equals(productName))
                    return pd;
            }

            return null;
        }
        
        /// <summary>
        /// Gets the IAP product declared in module settings with the specified identifier.
        /// </summary>
        /// <returns>The IAP product.</returns>
        /// <param name="productId">Product identifier.</param>
        public static IAPProduct GetIAPProductById(string productId)
        {
            foreach (IAPProduct pd in EM_Settings.InAppPurchasing.Products)
            {
                if (pd.Id.Equals(productId))
                    return pd;
            }

            return null;
        }

        #region Module-Enable-Only Methods

#if EM_UIAP

        /// <summary>
        /// Gets the product registered with UnityIAP stores by its name. This method returns
        /// a Product object, which contains more information than an IAPProduct
        /// object, whose main purpose is for displaying.
        /// </summary>
        /// <returns>The product.</returns>
        /// <param name="productName">Product name.</param>
        public static Product GetProduct(string productName)
        {
            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get product: not found product with name: " + productName);
                return null;
            }

            return GetProduct(iapProduct);
        }
        
        /// <summary>
        /// Gets the product registered with UnityIAP stores by its id. This method returns
        /// a Product object, which contains more information than an IAPProduct
        /// object, whose main purpose is for displaying.
        /// </summary>
        /// <returns>The product.</returns>
        /// <param name="productId">Product id.</param>
        public static Product GetProductWithId(string productId)
        {
            IAPProduct iapProduct = GetIAPProductById(productId);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get product: not found product with id: " + productId);
                return null;
            }

            return GetProduct(iapProduct);
        }
    
        
        /// <summary>
        /// Gets the product registered with UnityIAP stores. This method returns
        /// a Product object, which contains more information than an IAPProduct
        /// object, whose main purpose is for displaying.
        /// </summary>
        /// <returns>The product.</returns>
        /// <param name="product">the product.</param>
        public static Product GetProduct(IAPProduct product)
        {
            if (!IsInitialized())
            {
                Debug.Log("Couldn't get product: In-App Purchasing is not initialized.");
                return null;
            }
            
            if (product == null)
            {
                return null;
            }

            return sStoreController.products.WithID(product.Id);
        }

        /// <summary>
        /// Gets the product localized data provided by the stores.
        /// </summary>
        /// <returns>The product localized data.</returns>
        /// <param name="productName">Product name.</param>
        public static ProductMetadata GetProductLocalizedData(string productName)
        {
            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get product localized data: not found product with name: " + productName);
                return null;
            }

            return GetProductLocalizedData(iapProduct);
        }
        
        /// <summary>
        /// Gets the product localized data provided by the stores.
        /// </summary>
        /// <returns>The product localized data.</returns>
        /// <param name="productId">Product id.</param>
        public static ProductMetadata GetProductLocalizedDataWithId(string productId)
        {
            IAPProduct iapProduct = GetIAPProductById(productId);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get product localized data: not found product with name: " + productId);
                return null;
            }

            return GetProductLocalizedData(iapProduct);
        }
        
        /// <summary>
        /// Gets the product localized data provided by the stores.
        /// </summary>
        /// <returns>The product localized data.</returns>
        /// <param name="product">the product.</param>
        public static ProductMetadata GetProductLocalizedData(IAPProduct product)
        {
            if (!IsInitialized())
            {
                Debug.Log("Couldn't get product localized data: In-App Purchasing is not initialized.");
                return null;
            }
            
            if (product == null)
            {
                return null;
            }

            return sStoreController.products.WithID(product.Id).metadata;
        }

        /// <summary>
        /// Gets the parsed Apple InAppPurchase receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Apple In App Purchase receipt.</returns>
        /// <param name="productName">Product name.</param>
        public static AppleInAppPurchaseReceipt GetAppleIAPReceipt(string productName)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GetPurchaseReceipt(productName) as AppleInAppPurchaseReceipt;
            }
            else
            {
                Debug.Log("Getting Apple IAP receipt is only available on iOS.");
                return null;
            }
        }
        
        /// <summary>
        /// Gets the parsed Apple InAppPurchase receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Apple In App Purchase receipt.</returns>
        /// <param name="productId">Product id.</param>
        public static AppleInAppPurchaseReceipt GetAppleIAPReceiptWithId(string productId)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GetPurchaseReceiptWithId(productId) as AppleInAppPurchaseReceipt;
            }
            else
            {
                Debug.Log("Getting Apple IAP receipt is only available on iOS.");
                return null;
            }
        }
        
        /// <summary>
        /// Gets the parsed Apple InAppPurchase receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Apple In App Purchase receipt.</returns>
        /// <param name="product">The product.</param>
        public static AppleInAppPurchaseReceipt GetAppleIAPReceipt(IAPProduct product)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GetPurchaseReceipt(product) as AppleInAppPurchaseReceipt;
            }
            else
            {
                Debug.Log("Getting Apple IAP receipt is only available on iOS.");
                return null;
            }
        }

        /// <summary>
        /// Gets the parsed Google Play receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Google Play receipt.</returns>
        /// <param name="productName">Product name.</param>
        public static GooglePlayReceipt GetGooglePlayReceipt(string productName)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return GetPurchaseReceipt(productName) as GooglePlayReceipt;
            }
            else
            {
                Debug.Log("Getting Google Play receipt is only available on Android.");
                return null;
            }
        }
        
        /// <summary>
        /// Gets the parsed Google Play receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Google Play receipt.</returns>
        /// <param name="productId">Product id.</param>
        public static GooglePlayReceipt GetGooglePlayReceiptWithId(string productId)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return GetPurchaseReceiptWithId(productId) as GooglePlayReceipt;
            }
            else
            {
                Debug.Log("Getting Google Play receipt is only available on Android.");
                return null;
            }
        }
        
        /// <summary>
        /// Gets the parsed Google Play receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Google Play receipt.</returns>
        /// <param name="product">The product.</param>
        public static GooglePlayReceipt GetGooglePlayReceipt(IAPProduct product)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return GetPurchaseReceipt(product) as GooglePlayReceipt;
            }
            else
            {
                Debug.Log("Getting Google Play receipt is only available on Android.");
                return null;
            }
        }

        /// <summary>
        /// Gets the parsed purchase receipt for the product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The purchase receipt.</returns>
        /// <param name="productName">Product name.</param>
        public static IPurchaseReceipt GetPurchaseReceipt(string productName)
        {
            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get purchase receipt: not found product with name: " + productName);
                return null;
            }

            return GetPurchaseReceipt(iapProduct);
        }
        
        /// <summary>
        /// Gets the parsed purchase receipt for the product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The purchase receipt.</returns>
        /// <param name="produtId">Product id.</param>
        public static IPurchaseReceipt GetPurchaseReceiptWithId(string produtId)
        {
            IAPProduct iapProduct = GetIAPProductById(produtId);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get purchase receipt: not found product with id: " + produtId);
                return null;
            }

            return GetPurchaseReceipt(iapProduct);
        }
        
        /// <summary>
        /// Gets the parsed purchase receipt for the product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The purchase receipt.</returns>
        /// <param name="product">the product.</param>
        public static IPurchaseReceipt GetPurchaseReceipt(IAPProduct product)
        {
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Getting purchase receipt is only available on Android and iOS.");
                return null;
            }

            if (!IsInitialized())
            {
                Debug.Log("Couldn't get purchase receipt: In-App Purchasing is not initialized.");
                return null;
            }
            
            if (product == null)
            {
                Debug.Log("Couldn't get purchase receipt: product is null");
                return null;
            }

            Product pd = sStoreController.products.WithID(product.Id);

            if (!pd.hasReceipt)
            {
                Debug.Log("Couldn't get purchase receipt: this product doesn't have a receipt.");
                return null;
            }

            if (!IsReceiptValidationEnabled())
            {
                Debug.Log("Couldn't get purchase receipt: please enable receipt validation.");
                return null;
            }

            IPurchaseReceipt[] purchaseReceipts;
            if (!ValidateReceipt(pd.receipt, out purchaseReceipts))
            {
                Debug.Log("Couldn't get purchase receipt: the receipt of this product is invalid.");
                return null;
            }

            foreach (var r in purchaseReceipts)
            {
                if (r.productID.Equals(pd.definition.storeSpecificId))
                    return r;
            }

            // If we reach here, there's no receipt with the matching productID
            return null;
        }

        /// <summary>
        /// Gets the Apple App receipt. This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Apple App receipt.</returns>
        public static AppleReceipt GetAppleAppReceipt()
        {
            if (!IsInitialized())
            {
                Debug.Log("Couldn't get Apple app receipt: In-App Purchasing is not initialized.");
                return null;
            }

            if (!EM_Settings.InAppPurchasing.ValidateAppleReceipt)
            {
                Debug.Log("Couldn't get Apple app receipt: Please enable Apple receipt validation.");
                return null;
            }

            // Note that the code is disabled in the editor for it to not stop the EM editor code (due to ClassNotFound error)
            // from recreating the dummy AppleTangle class if they were inadvertently removed.
#if UNITY_IOS && !UNITY_EDITOR
            // Get a reference to IAppleConfiguration during IAP initialization.
            var appleConfig = sBuilder.Configure<IAppleConfiguration>();
            var receiptData = System.Convert.FromBase64String(appleConfig.appReceipt);
            AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
            return receipt;
#else
            Debug.Log("Getting Apple app receipt is only available on iOS.");
            return null;
#endif
        }

        /// <summary>
        /// Gets the subscription info of the product using the SubscriptionManager class,
        /// which currently supports the Apple store and Google Play store.
        /// </summary>
        /// <returns>The subscription info.</returns>
        /// <param name="productName">Product name.</param>
        public static SubscriptionInfo GetSubscriptionInfo(string productName)
        {
            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get subscription info: not found product with name: " + productName);
                return null;
            }
            return GetSubscriptionInfo(iapProduct);
        }
        
        /// <summary>
        /// Gets the subscription info of the product using the SubscriptionManager class,
        /// which currently supports the Apple store and Google Play store.
        /// </summary>
        /// <returns>The subscription info.</returns>
        /// <param name="productId">Product name.</param>
        public static SubscriptionInfo GetSubscriptionInfoWithId(string productId)
        {
            IAPProduct iapProduct = GetIAPProductById(productId);

            if (iapProduct == null)
            {
                Debug.Log("Couldn't get subscription info: not found product with id: " + productId);
                return null;
            }
            return GetSubscriptionInfo(iapProduct);
        }
        
        /// <summary>
        /// Gets the subscription info of the product using the SubscriptionManager class,
        /// which currently supports the Apple store and Google Play store.
        /// </summary>
        /// <returns>The subscription info.</returns>
        /// <param name="product">The product.</param>
        public static SubscriptionInfo GetSubscriptionInfo(IAPProduct product)
        {
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Getting subscription info is only available on Android and iOS.");
                return null;
            }

            if (!IsInitialized())
            {
                Debug.Log("Couldn't get subscripton info: In-App Purchasing is not initialized.");
                return null;
            }
            
            if (product == null)
            {
                Debug.Log("Couldn't get subscription info: product is null");
                return null;
            }

            Product prod = sStoreController.products.WithID(product.Id);

            if (prod.definition.type != ProductType.Subscription)
            {
                Debug.Log("Couldn't get subscription info: this product is not a subscription product.");
                return null;
            }

            if (string.IsNullOrEmpty(prod.receipt))
            {
                Debug.Log("Couldn't get subscription info: this product doesn't have a valid receipt.");
                return null;
            }

            if (!IsProductAvailableForSubscriptionManager(prod.receipt))
            {
                Debug.Log("Couldn't get subscription info: this product is not available for SubscriptionManager class, " +
                    "only products that are purchase by 1.19+ SDK can use this class.");
                return null;
            }

            // Now actually get the subscription info using SubscriptionManager class.
            Dictionary<string, string> introPriceDict = null;

            if (sAppleExtensions != null)
                introPriceDict = sAppleExtensions.GetIntroductoryPriceDictionary();

            string introJson = (introPriceDict == null || !introPriceDict.ContainsKey(prod.definition.storeSpecificId)) ?
                null : introPriceDict[prod.definition.storeSpecificId];

            SubscriptionManager p = new SubscriptionManager(prod, introJson);
            return p.getSubscriptionInfo();
        }

        /// <summary>
        /// Gets the name of the store.
        /// </summary>
        /// <returns>The store name.</returns>
        /// <param name="store">Store.</param>
        public static string GetStoreName(IAPStore store)
        {
            switch (store)
            {
                case IAPStore.GooglePlay:
                    return GooglePlay.Name;
                case IAPStore.AmazonAppStore:
                    return AmazonApps.Name;
                case IAPStore.SamsungApps:
                    return SamsungApps.Name;
                case IAPStore.MacAppStore:
                    return MacAppStore.Name;
                case IAPStore.AppleAppStore:
                    return AppleAppStore.Name;
                case IAPStore.WinRT:
                    return WindowsStore.Name;
                case IAPStore.FacebookStore:
                    return FacebookStore.Name;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the type of the product.
        /// </summary>
        /// <returns>The product type.</returns>
        /// <param name="pType">P type.</param>
        public static ProductType GetProductType(IAPProductType pType)
        {
            switch (pType)
            {
                case IAPProductType.Consumable:
                    return ProductType.Consumable;
                case IAPProductType.NonConsumable:
                    return ProductType.NonConsumable;
                case IAPProductType.Subscription:
                    return ProductType.Subscription;
                default:
                    return ProductType.Consumable;
            }
        }

        /// <summary>
        /// Converts to UnityIAP AndroidStore.
        /// </summary>
        /// <returns>The android store.</returns>
        /// <param name="store">Store.</param>
        public static AndroidStore GetAndroidStore(IAPAndroidStore store)
        {
            switch (store)
            {
                case IAPAndroidStore.AmazonAppStore:
                    return AndroidStore.AmazonAppStore;
                case IAPAndroidStore.GooglePlay:
                    return AndroidStore.GooglePlay;
                case IAPAndroidStore.SamsungApps:
                    return AndroidStore.SamsungApps;
                case IAPAndroidStore.NotSpecified:
                    return AndroidStore.NotSpecified;
                default:
                    return AndroidStore.NotSpecified;
            }
        }

        /// <summary>
        /// Converts to UnityIAP AppStore.
        /// </summary>
        /// <returns>The app store.</returns>
        /// <param name="store">Store.</param>
        public static AppStore GetAppStore(IAPAndroidStore store)
        {
            switch (store)
            {
                case IAPAndroidStore.AmazonAppStore:
                    return AppStore.AmazonAppStore;
                case IAPAndroidStore.GooglePlay:
                    return AppStore.GooglePlay;
                case IAPAndroidStore.SamsungApps:
                    return AppStore.SamsungApps;
                case IAPAndroidStore.NotSpecified:
                    return AppStore.NotSpecified;
                default:
                    return AppStore.NotSpecified;
            }
        }

        /// <summary>
        /// Gets the current Amazon user ID (for other Amazon services).
        /// Only call this method after initialization has been done.
        /// Returns <c>null</c> if the current store is not Amazon store.
        /// </summary>
        /// <returns>The amazon user identifier.</returns>
        public static string GetAmazonUserId()
        {
            string userId = null;

            if (StoreExtensionProvider != null)
            {
                IAmazonExtensions aEtx = StoreExtensionProvider.GetExtension<IAmazonExtensions>();

                if (aEtx != null)
                    userId = aEtx.amazonUserId;
            }

            return userId;
        }

        /// <summary>
        /// Confirms a pending purchase. Use this if you register a <see cref="PrePurchaseProcessing"/>
        /// delegate and return a <see cref="PrePurchaseProcessResult.Suspend"/> in it so that UnityIAP
        /// won't inform the app of the purchase again. After confirming the purchase, either <see cref="PurchaseCompleted"/>
        /// or <see cref="PurchaseFailed"/> event will be called according to the input given by the caller.
        /// </summary>
        /// <param name="product">The pending product to confirm.</param>
        /// <param name="purchaseSuccess">If true, <see cref="PurchaseCompleted"/> event will be called, otherwise <see cref="PurchaseFailed"/> event will be called.</param>
        public static void ConfirmPendingPurchase(Product product, bool purchaseSuccess)
        {
            if (sStoreController != null)
                sStoreController.ConfirmPendingPurchase(product);

            if (purchaseSuccess)
            {
                if (PurchaseCompleted != null)
                    PurchaseCompleted(GetIAPProductById(product.definition.id));
            }
            else
            {
                if (PurchaseFailed != null)
                    PurchaseFailed(GetIAPProductById(product.definition.id), CONFIRM_PENDING_PURCHASE_FAILED);
            }
        }

#endif

        #endregion

        #region PrePurchaseProcessing

#if EM_UIAP

        /// <summary>
        /// Available results for the <see cref="PrePurchaseProcessing"/> delegate.
        /// </summary>
        public enum PrePurchaseProcessResult
        {
            /// <summary>
            /// Continue the normal purchase processing.
            /// </summary>
            Proceed,
            /// <summary>
            /// Suspend the purchase, PurchaseProcessingResult.Pending will be returned to UnityIAP.
            /// </summary>
            Suspend,
            /// <summary>
            /// Abort the purchase, PurchaseFailed event will be called.
            /// </summary>
            Abort
        }

        /// <summary>
        /// Once registered, this delegate will be invoked before the normal purchase processing method.
        /// The return value of this delegate determines how the purchase processing will be done.
        /// If you want to intervene in the purchase processing step, e.g. adding custom receipt validation,
        /// this delegate is the place to go.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public delegate PrePurchaseProcessResult PrePurchaseProcessing(PurchaseEventArgs args);

        private static PrePurchaseProcessing sPrePurchaseProcessDel;

        /// <summary>
        /// Registers a <see cref="PrePurchaseProcessing"/> delegate.
        /// </summary>
        /// <param name="del"></param>
        public static void RegisterPrePurchaseProcessDelegate(PrePurchaseProcessing del)
        {
            sPrePurchaseProcessDel = del;
        }

#endif

        #endregion

        #region IStoreListener implementation

#if EM_UIAP

        private class StoreListener : IStoreListener
        {
            public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
            {
                // Purchasing has succeeded initializing.
                Debug.Log("In-App Purchasing OnInitialized: PASS");

                // Done initializing.
                sIsInitializing = false;

                // Overall Purchasing system, configured with products for this application.
                sStoreController = controller;

                // Store specific subsystem, for accessing device-specific store features.
                sStoreExtensionProvider = extensions;

                // Get the store extensions for later use.
                sAppleExtensions = sStoreExtensionProvider.GetExtension<IAppleExtensions>();
                sGooglePlayStoreExtensions = sStoreExtensionProvider.GetExtension<IGooglePlayStoreExtensions>();
                sAmazonExtensions = sStoreExtensionProvider.GetExtension<IAmazonExtensions>();
                sSamsungAppsExtensions = sStoreExtensionProvider.GetExtension<ISamsungAppsExtensions>();

                // Apple store specific setup.
                if (sAppleExtensions != null && Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    // Enable Ask To Buy simulation in sandbox if needed.
                    sAppleExtensions.simulateAskToBuy = EM_Settings.InAppPurchasing.SimulateAppleAskToBuy;

                    // Register a handler for Ask To Buy's deferred purchases.
                    sAppleExtensions.RegisterPurchaseDeferredListener(OnApplePurchaseDeferred);
                }

                // Fire event
                if (InitializeSucceeded != null)
                    InitializeSucceeded();
            }

            public void OnInitializeFailed(InitializationFailureReason error)
            {
                // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
                Debug.Log("In-App Purchasing OnInitializeFailed. InitializationFailureReason:" + error);

                // Done initializing.
                sIsInitializing = false;

                // Fire event
                if (InitializeFailed != null)
                    InitializeFailed();
            }

            public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
            {
                // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
                // this reason with the user to guide their troubleshooting actions.
                Debug.Log(string.Format("Couldn't purchase product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

                // Fire purchase failure event
                if (PurchaseFailed != null)
                    PurchaseFailed(GetIAPProductById(product.definition.id), failureReason.ToString());
            }

            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
            {
                Debug.Log("Processing purchase of product: " + args.purchasedProduct.transactionID);

                IAPProduct pd = GetIAPProductById(args.purchasedProduct.definition.id);

                if (sPrePurchaseProcessDel != null)
                {
                    var nextStep = sPrePurchaseProcessDel(args);

                    if (nextStep == PrePurchaseProcessResult.Abort)
                    {
                        Debug.Log("Purchase aborted.");

                        // Fire purchase failure event
                        if (PurchaseFailed != null)
                            PurchaseFailed(pd, PROCESSING_PURCHASE_ABORT);

                        return PurchaseProcessingResult.Complete;
                    }
                    else if (nextStep == PrePurchaseProcessResult.Suspend)
                    {
                        Debug.Log("Purchase suspended.");
                        return PurchaseProcessingResult.Pending;
                    }
                    else
                    {
                        // Proceed.
                        Debug.Log("Proceeding with purchase processing.");
                    }
                }

                bool validPurchase = true;  // presume validity if not validate receipt

                if (IsReceiptValidationEnabled())
                {
                    IPurchaseReceipt[] purchaseReceipts;
                    validPurchase = ValidateReceipt(args.purchasedProduct.receipt, out purchaseReceipts);
                }

                if (validPurchase)
                {
                    Debug.Log("Product purchase completed.");

                    // Fire purchase success event
                    if (PurchaseCompleted != null)
                        PurchaseCompleted(pd);
                }
                else
                {
                    Debug.Log("Couldn't purchase product: Invalid receipt.");

                    // Fire purchase failure event
                    if (PurchaseFailed != null)
                        PurchaseFailed(pd, PROCESSING_PURCHASE_INVALID_RECEIPT);
                }

                return PurchaseProcessingResult.Complete;
            }
        }

#endif

        #endregion

        #region Private Stuff

#if EM_UIAP

        /// <summary>
        /// Raises the purchase deferred event.
        /// Apple store only.
        /// </summary>
        /// <param name="product">Product.</param>
        private static void OnApplePurchaseDeferred(Product product)
        {
            Debug.Log("Purchase deferred: " + product.definition.id);

            if (PurchaseDeferred != null)
                PurchaseDeferred(GetIAPProductById(product.definition.id));
        }

        /// <summary>
        /// Raises the Apple promotional purchase event.
        /// Apple store only.
        /// </summary>
        /// <param name="product">Product.</param>
        private static void OnApplePromotionalPurchase(Product product)
        {
            Debug.Log("Attempted promotional purchase: " + product.definition.id);

            if (PromotionalPurchaseIntercepted != null)
                PromotionalPurchaseIntercepted(GetIAPProductById(product.definition.id));
        }

        /// <summary>
        /// Determines if receipt validation is enabled.
        /// </summary>
        /// <returns><c>true</c> if is receipt validation enabled; otherwise, <c>false</c>.</returns>
        private static bool IsReceiptValidationEnabled()
        {
            bool canValidateReceipt = false;    // disable receipt validation by default

            if (Application.platform == RuntimePlatform.Android)
            {
                // On Android, receipt validation is only available for Google Play store
                canValidateReceipt = EM_Settings.InAppPurchasing.ValidateGooglePlayReceipt;
                canValidateReceipt &= (GetAndroidStore(EM_Settings.InAppPurchasing.TargetAndroidStore) == AndroidStore.GooglePlay);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer ||
                     Application.platform == RuntimePlatform.OSXPlayer ||
                     Application.platform == RuntimePlatform.tvOS)
            {
                // Receipt validation is also available for Apple app stores
                canValidateReceipt = EM_Settings.InAppPurchasing.ValidateAppleReceipt;
            }

            return canValidateReceipt;
        }

        /// <summary>
        /// Validates the receipt. Works with receipts from Apple stores and Google Play store only.
        /// Always returns true for other stores.
        /// </summary>
        /// <returns><c>true</c>, if the receipt is valid, <c>false</c> otherwise.</returns>
        /// <param name="receipt">Receipt.</param>
        /// <param name="logReceiptContent">If set to <c>true</c> log receipt content.</param>
        private static bool ValidateReceipt(string receipt, out IPurchaseReceipt[] purchaseReceipts, bool logReceiptContent = false)
        {
            purchaseReceipts = new IPurchaseReceipt[0];   // default the out parameter to an empty array   

            // Does the receipt has some content?
            if (string.IsNullOrEmpty(receipt))
            {
                Debug.Log("Receipt Validation: receipt is null or empty.");
                return false;
            }

            bool isValidReceipt = true; // presume validity for platforms with no receipt validation.
                                        // Unity IAP's receipt validation is only available for Apple app stores and Google Play store.   
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS

            byte[] googlePlayTangleData = null;
            byte[] appleTangleData = null;

            // Here we populate the secret keys for each platform.
            // Note that the code is disabled in the editor for it to not stop the EM editor code (due to ClassNotFound error)
            // from recreating the dummy AppleTangle and GoogleTangle classes if they were inadvertently removed.

#if UNITY_ANDROID && !UNITY_EDITOR
            googlePlayTangleData = GooglePlayTangle.Data();
#endif

#if (UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS) && !UNITY_EDITOR
            appleTangleData = AppleTangle.Data();
#endif

            // Prepare the validator with the secrets we prepared in the Editor obfuscation window.
#if UNITY_5_6_OR_NEWER
            var validator = new CrossPlatformValidator(googlePlayTangleData, appleTangleData, Application.identifier);
#else
            var validator = new CrossPlatformValidator(googlePlayTangleData, appleTangleData, Application.bundleIdentifier);
#endif

            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(receipt);

                // If the validation is successful, the result won't be null.
                if (result == null)
                {
                    isValidReceipt = false;
                }
                else
                {
                    purchaseReceipts = result;

                    // For informational purposes, we list the receipt(s)
                    if (logReceiptContent)
                    {
                        Debug.Log("Receipt contents:");
                        foreach (IPurchaseReceipt productReceipt in result)
                        {
                            if (productReceipt != null)
                            {
                                Debug.Log(productReceipt.productID);
                                Debug.Log(productReceipt.purchaseDate);
                                Debug.Log(productReceipt.transactionID);
                            }
                        }
                    }
                }
            }
            catch (IAPSecurityException)
            {
                isValidReceipt = false;
            }
#endif

            return isValidReceipt;
        }

        private static bool IsProductAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information, " +
                    "the 'Store' or 'Payload' field is missing.");
                return false;
            }

            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing.");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing.");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later.");
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                        {
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            return false;
        }

#endif

        #endregion
    }
}