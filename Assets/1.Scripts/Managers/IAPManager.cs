using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Services.Core;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Cysharp.Threading.Tasks;
using System.Threading;

public class IAPManager : MonoBehaviour
{
    private StoreController m_StoreController;
    private PurchaseService purchaseService;
    public Transform m_HandleStorePage;

    private static IAPManager instance = null;
    public static IAPManager Get => instance;

    readonly string kPremiumAccStoreKey = "premium_account";
    readonly string kLocalPremiumUnlocked = "PremiumFeatures";
    StandardPurchasingModule m_PurchasingModule;
    bool m_Initialized
    {
        get { return m_StoreController != null; }
    }
    bool m_InTransaction = false;
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
        m_StoreController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        m_StoreController.OnPurchaseFailed += OnPurchaseFailed;
        m_StoreController.OnStoreDisconnected += OnStoreDisconnected;

        // Connect to the store
        await m_StoreController.Connect();

        m_StoreController.OnProductsFetched += OnProductsFetched;
        m_StoreController.OnPurchasesFetched += OnPurchasesFetched;

        // Fetch products
        var initialProductsToFetch = new List<ProductDefinition>
        {
            new(kPremiumAccStoreKey, ProductType.NonConsumable),
        };

        m_StoreController.FetchProducts(initialProductsToFetch);

        m_PurchasingModule = StandardPurchasingModule.Instance();

#if UNITY_EDITOR
            PlayerPrefs.SetInt(kLocalPremiumUnlocked, 0);
            m_PurchasingModule.useFakeStoreAlways = true;
            m_PurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.Default;
#endif

    }
    
    void OnStoreDisconnected(StoreConnectionFailureDescription iReason)
    {
        Debug.Log("Store disconnected");
    }
    void OnProductsFetched(List<Product> products)
    {
        Debug.Log("OnProductsFetched");
        // Handle fetched products  
        m_StoreController.FetchPurchases();
    }
    void OnPurchasesFetched(Orders orders)
    {
        Debug.Log("OnPurchasesFetched");
        // Process purchases, e.g. check for entitlements from completed orders
        CheckPremiumPurchased();
    }

    public void OnPurchasePending(PendingOrder iPendingOrder)
    {
        Debug.Log("Pending Order");
        m_StoreController.ConfirmPurchase(iPendingOrder);
    }

    public void OnPurchaseConfirmed(Order iOrder)
    {
        Debug.Log("OnPurchaseConfirmed Order");
        CheckPremiumPurchased();
        CloseStore();
        m_InTransaction = false;
    }
    
    private void OnPurchaseFailed(FailedOrder iOrder)
    {
        Debug.LogError("Purchase failed.");
    }

    // -------------------------------

    public void BuyPremiumContent()
    {
        if (m_InTransaction)
            return;

        if (m_Initialized)
        {
            Product product = m_StoreController.GetProductById(kPremiumAccStoreKey);
            if (product != null && product.availableToPurchase)
            {
                m_StoreController.PurchaseProduct(product);
                Debug.Log("Purchase product called on : " + kPremiumAccStoreKey);
                m_InTransaction = true;
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
        if (PlayerPrefs.GetInt(kLocalPremiumUnlocked, 0) == 1)
            return true;

        var purchases = m_StoreController.GetPurchases();
        foreach(Order order in purchases)
        {
            if (order is ConfirmedOrder)
            {
                Debug.Log("Product " + kPremiumAccStoreKey + " purchase found. ");
                PlayerPrefs.SetInt(kLocalPremiumUnlocked, 1);
                return true;
            }
        }
        return false;
    }

    public void DebugPremiumUnlock()
    {
        PlayerPrefs.SetInt(kLocalPremiumUnlocked, 1);
    }

    public void OpenStore()
    {
        //m_HandleStorePage.gameObject.SetActive(true);
    }

    public void CloseStore()
    {
        //m_HandleStorePage.gameObject.SetActive(false);
        CheckPremiumPurchased();
    }
}
