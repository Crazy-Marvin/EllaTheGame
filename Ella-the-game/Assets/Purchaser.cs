using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : MonoBehaviour
{
    //public const string kProductIDRemoveAds = "remove_ads";
    public const string kProductIDRemoveAds = "1";

    private StoreController _storeController;
    private bool _initialized;

    private async void Start()
    {
        _storeController = UnityIAPServices.StoreController();

        _storeController.OnStoreDisconnected += OnStoreDisconnected;
        _storeController.OnProductsFetched += OnProductsFetched;
        _storeController.OnProductsFetchFailed += OnProductsFetchFailed;
        _storeController.OnPurchasesFetched += OnPurchasesFetched;
        _storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        _storeController.OnPurchasePending += OnPurchasePending;

        await _storeController.Connect();

        var productsToFetch = new List<ProductDefinition>
        {
            new ProductDefinition(kProductIDRemoveAds, ProductType.NonConsumable)
        };

        _storeController.FetchProducts(productsToFetch);
    }

    public void BuyRemoveAdsPackage()
    {
        Debug.Log("BuyRemoveAdsPackage CLICKED");

        if (_storeController == null)
        {
            Debug.Log("StoreController is null");
            return;
        }

        Debug.Log("Calling PurchaseProduct...");
        _storeController.PurchaseProduct(kProductIDRemoveAds);
    }

    public void RestorePurchases()
    {
        if (_storeController == null)
        {
            Debug.Log("RestorePurchases FAIL. StoreController is null.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            _storeController.RestoreTransactions((success, error) =>
            {
                Debug.Log($"RestorePurchases result: {success}, error: {error}");
            });
        }
        else
        {
            // On Google Play, purchases are restored when FetchPurchases() is called.
            Debug.Log("RestorePurchases on this platform will happen through FetchPurchases().");
            _storeController.FetchPurchases();
        }
    }

    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log($"OnProductsFetched: PASS. Count = {products.Count}");

        _initialized = true;

        _storeController.FetchPurchases();
    }

    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.Log($"OnProductsFetchFailed: {failure}");
    }

    private void OnPurchasesFetched(Orders orders)
    {
        Debug.Log("OnPurchasesFetched: received orders.");

        foreach (var confirmedOrder in orders.ConfirmedOrders)
        {
            var product = confirmedOrder.CartOrdered.Items().FirstOrDefault()?.Product;
            if (product == null)
                continue;

            if (string.Equals(product.definition.id, kProductIDRemoveAds, StringComparison.Ordinal))
            {
                GrantRemoveAds();
            }
        }
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.Log($"OnPurchasesFetchFailed: {failure}");
    }

    private void OnPurchasePending(PendingOrder order)
    {
        foreach (var item in order.CartOrdered.Items())
        {
            if (string.Equals(item.Product.definition.id, kProductIDRemoveAds, StringComparison.Ordinal))
            {
                GrantRemoveAds();
            }
        }

        _storeController.ConfirmPurchase(order);
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription failure)
    {
        Debug.Log($"OnStoreDisconnected: {failure}");
    }

    private void GrantRemoveAds()
    {
        PlayerPrefs.SetInt("NoAds", 1);
        PlayerPrefs.Save();
        Debug.Log("Remove Ads granted.");
    }
}