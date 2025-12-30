using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
public class ShopOrderManager : MonoBehaviour
{
    public ItemWaitLine ref_itemQ;
    public CustomerWaitLine ref_customerQ;
    public UIShopGame ref_UIShopOrders;
    private UnityEvent<ShopItem> OnOrderReceivedCB;
    public ShopGameSO ShopGameDescriptor;
    public UnityEvent<int> OnOrderSuccessfullCB;
    public bool ShopClosed = false;

    public async UniTaskVoid Setup()
    {
        OnOrderReceivedCB = new UnityEvent<ShopItem>();
        OnOrderReceivedCB.AddListener(MatchOrder);

        await UniTask.Yield();

        ref_itemQ.Setup(ShopGameDescriptor.WaitLineItemSpacing, ShopGameDescriptor.MaxItems);
        ref_customerQ.Setup(ShopGameDescriptor.WaitLineCustomerSpacing, ShopGameDescriptor.MaxCustomers);
    }

    public void Reset()
    {
        OnOrderSuccessfullCB.RemoveAllListeners();
        ref_itemQ.Flush();
        ref_customerQ.Flush();
    }
    public bool IsReady()
    {
        if (ref_itemQ == null)
            return false;
        if (ref_customerQ == null)
            return false;
        if (OnOrderReceivedCB == null)
            return false;

        return true;
    }

    public async UniTaskVoid PlaceOrder(ShopItemSO iItem, UnityAction iOrderDoneCallback)
    {
        if (ShopClosed)
            return;
        
        if (ref_itemQ.IsFull())
        {
            // TODO : Feedback upon full queue button click
            iOrderDoneCallback.Invoke();
            return;
        }

        ShopItem newItem = ItemFactory.Make(iItem);
        newItem.transform.parent = ref_itemQ.transform;
        ref_itemQ.Enqueue(newItem);

        await newItem.WaitCraft();

        OnOrderReceivedCB?.Invoke(newItem);
        iOrderDoneCallback.Invoke();
    }

    async void MatchOrder(ShopItem iItem)
    {
        // wait to be top of order stack
        await ref_itemQ.WaitForItemAtPeekAndCrafted(iItem);
        // Wait for customers if nobody in shop
        await ref_customerQ.WaitForCustomer();
        // Wait for customer to be eligible to fetch order
        await ref_customerQ.Peek().WaitTurnToOrder();

        if (ref_itemQ.Peek().itemType == ref_customerQ.Peek().desiredItem.itemType)
            OnOrderSuccessfull();
        else
            OnOrderFailed();
    }

    void OnOrderSuccessfull()
    {
        OnOrderSuccessfullCB?.Invoke(ref_itemQ.Peek().value);
        ref_itemQ.Dequeue();
        ref_customerQ.Peek().SuccessfullOrder = true;
        ref_customerQ.Dequeue();
    }
    void OnOrderFailed()
    {
        ref_itemQ.Dequeue();
        ref_customerQ.Peek().SuccessfullOrder = false;
        ref_customerQ.Peek().OnCommandFailure();
    }

    public void OpenShop()
    {
        ref_UIShopOrders.gameObject.SetActive(true);
    }

    public void CloseShop()
    {
        ref_UIShopOrders.gameObject.SetActive(false);
    }
}
