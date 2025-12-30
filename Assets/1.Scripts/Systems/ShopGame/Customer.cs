using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Customer : CustomerAgent
{
    public CustomerCollectionSO customerCollection;
    public SpriteRenderer customerRenderer;
    public Transform handle_desiredItemDisplay;
    public SpriteRenderer desiredItemDisplay;
    public ParticleSystem OnCommandSuccessPS;
    public ParticleSystem OnCommandFailedPS;
    public UnityEvent<Customer> OnCommandSuccessCB;
    public bool CanReceiveOrder = false;
    public bool SuccessfullOrder = false;
    [Header("Internals")]
    public ShopCustomerSO customerType;
    public ShopItemSO desiredItem;
    public async Task WaitTurnToOrder()
    {
        while (!CanReceiveOrder)
        { await Task.Yield(); }
        return;
    }

    void Start()
    {
        // pick customer
        int selectedCustomer = UnityEngine.Random.Range(0, customerCollection.customers.Count);
        customerType = customerCollection.customers[selectedCustomer];
        customerRenderer.sprite = customerType.sprite;

        if (desiredItem==null)
        {
            int selectedItemCmd = UnityEngine.Random.Range(0, customerType.availableItems.items.Count);
            desiredItem = customerType.availableItems.items[selectedItemCmd];
        }
        desiredItemDisplay.sprite = desiredItem.icon;
    }


    public void OnCommandSuccess()
    {
        if (OnCommandSuccessPS != null)
        {
            OnCommandSuccessPS.Play();
        }
        Destroy(handle_desiredItemDisplay.gameObject);
        OnCommandSuccessCB?.Invoke(this);
    }

    public void OnCommandFailure()
    {
        if (OnCommandFailedPS != null)
        {
            OnCommandFailedPS.Play();
        }
    }
}