using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    private static IAPManager instance = null;
    public static IAPManager Get => instance;

    readonly string kPremiumAccKey = "premium_account";
    readonly string kLocalPremiumUnlocked = "PremiumFeatures";

    bool m_Initialized
    {
        get { return storeController != null && storeExtensionProvider != null; }
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
        InitializePurchasing();
    }

    void InitializePurchasing()
    {
        if (m_Initialized)
            return;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(kPremiumAccKey, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
        Debug.Log("Unity IAP inialized : OK");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Unity IAP inialized : KO : " + error);
    }
    public void OnInitializeFailed(InitializationFailureReason error, string? err)
    {
        Debug.Log("Unity IAP inialized : KO : " + err);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (String.Equals(purchaseEvent.purchasedProduct.definition.id, kPremiumAccKey, StringComparison.Ordinal))
        {
            Debug.Log("Achat réussi : contenu premium débloqué !");
            // Ici, débloquez le contenu premium dans votre jeu
            PlayerPrefs.SetInt(kLocalPremiumUnlocked, 1);
            PlayerPrefs.Save();
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Achat échoué : " + product.definition.id + " - " + failureReason);
    }

    public void RestorePurchases()
    {
        if (!m_Initialized)
        {
            Debug.Log("Unity IAP not initialized");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            var appleExtensions = storeExtensionProvider.GetExtension<IAppleExtensions>();
            appleExtensions.RestoreTransactions(
                (result, message) =>
                {
                    if (result)
                    {
                        Debug.Log("Restore purchases succeeded");
                    }
                    else
                    {
                        Debug.Log("Restore purchases failed with message : " + message);
                    }
                });
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            // On Android, purchases are restored automatically on initialization
            Debug.Log("Android: Purchases are restored automatically");
        }
    }

    public void BuyPremiumContent()
    {
        if (m_Initialized)
        {
            Product product = storeController.products.WithID(kPremiumAccKey);
            if (product != null && product.availableToPurchase)
            {
                storeController.InitiatePurchase(product);
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
        return PlayerPrefs.GetInt(kLocalPremiumUnlocked, 0) == 1 || IsPremiumPurchased();
    }

    public bool IsPremiumPurchased()
    {
        if (!m_Initialized)
            return false;
        Product product = storeController.products.WithID(kPremiumAccKey);
        if (product != null)
        {
            return product.hasReceipt;
        }
        return false;

    }
}
