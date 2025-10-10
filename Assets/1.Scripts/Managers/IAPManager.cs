using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
// using Unity.Services.IAP;
// using Unity.Services.IAP.AppStore;
// using Unity.Services.IAP.AppleStore;
// using Unity.Services.IAP.GooglePlay;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Cysharp.Threading.Tasks;
using System.Threading;

public class IAPManager : MonoBehaviour
{
    private StoreController m_StoreController;
    private PurchaseService purchaseService;
    //private IExtensionProvider storeExtensionProvider;

    private static IAPManager instance = null;
    public static IAPManager Get => instance;

    readonly string kPremiumAccStoreKey = "premium_account";
    readonly string kLocalPremiumUnlocked = "PremiumFeatures";

    bool m_Initialized
    {
        get { return m_StoreController != null && purchaseService != null; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        AsyncStart();
    }

    async UniTaskVoid AsyncStart()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await InitializeIAP();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to initialized IAP Manager : " + e);
        }
    }

    private async UniTask InitializeIAP()
    {
        m_StoreController = UnityIAPServices.StoreController();
        m_StoreController.OnPurchasePending += OnPurchasePending;

        // Connect to the store
        await m_StoreController.Connect();

        m_StoreController.OnProductsFetched += OnProductsFetched;
        m_StoreController.OnPurchasesFetched += OnPurchasesFetched ;

        // Fetch products
        var initialProductsToFetch = new List<ProductDefinition>  
        {  
            new(kPremiumAccStoreKey, ProductType.NonConsumable),  
        };  
    
        m_StoreController.FetchProducts(initialProductsToFetch);  
    }
    void OnProductsFetched(List<Product> products)  
    {
        // Handle fetched products  
        m_StoreController.FetchPurchases();
    }
    void OnPurchasesFetched(Orders orders)
    {
        // Process purchases, e.g. check for entitlements from completed orders
        CheckPremiumPurchased();
    }
    
    void OnPurchasePending(PendingOrder iPendingOrder)
    {
        // 
    }

    // -------------------------------

    public void BuyPremiumContent()
    {
        if (m_Initialized)
        {
            Product product = m_StoreController.GetProductById(kPremiumAccStoreKey);
            if (product != null && product.availableToPurchase)
            {
                m_StoreController.PurchaseProduct(product);
            }
            else
            {
                Debug.LogError("Produit non disponible.");
            }
        }
        else
        {
            Debug.LogError("Unity IAP non initialisé.");
        }
    }

    public bool CanAccessPremiumContent()
    {
        return PlayerPrefs.GetInt(kLocalPremiumUnlocked, 0) == 1 || CheckPremiumPurchased();
    }

    public bool CheckPremiumPurchased()
    {
        if (!m_Initialized)
            return false;

        var purchases = m_StoreController.GetPurchases();
        foreach(Order order in purchases)
        {
            if (order is ConfirmedOrder)
            {
                Debug.Log("Product " + kPremiumAccStoreKey + " purchased !");
                PlayerPrefs.SetInt(kLocalPremiumUnlocked, 1);
                return true;
            }
        }
        return false;
    }
}
