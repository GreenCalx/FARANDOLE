using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [Header("Mand Refs")]
    public GameObject prefab_DefaultCustomer;
    public Transform ref_spawnPoint;
    public Transform ref_exitPoint;
    public CustomerWaitLine ref_customerQ;
    public List<Customer> spawnedCustomers = new List<Customer>();

    public bool IsReady()
    {
        if (prefab_DefaultCustomer == null)
            return false;
        if (ref_spawnPoint == null)
            return false;
        if (ref_customerQ == null)
            return false;

        return true;
    }

    public void Setup(GameObject iCustomerPrefab)
    {
        prefab_DefaultCustomer = iCustomerPrefab;
    }

    public bool EnterShop()
    {
        if (ref_customerQ.IsFull())
            return false;

        GameObject newCustomer = Instantiate(prefab_DefaultCustomer);
        newCustomer.name = Random.Range(0, 9999).ToString();

        newCustomer.transform.position = ref_spawnPoint.position;
        newCustomer.transform.rotation = ref_spawnPoint.rotation;
        newCustomer.transform.parent = transform; // avoid scene hierarchy pollution

        Customer as_Customer = newCustomer.GetComponent<Customer>();
        if (as_Customer == null)
        {
            Destroy(newCustomer);
            return false;
        }
        ref_customerQ.Enqueue(as_Customer);
        as_Customer.OnDestinationReachCB.AddListener(()=>CustomerCanOrder(as_Customer));
        as_Customer.OnCommandSuccessCB.AddListener(LeaveShop);

        spawnedCustomers.Add(as_Customer);

        return true;
    }

    public void CustomerCanOrder(Customer iCustomer)
    {
        iCustomer.CanReceiveOrder = (iCustomer == ref_customerQ.Peek());
    }

    public void LeaveShop(Customer iCustomer)
    {
        iCustomer.targetPosition = ref_exitPoint.position;
        iCustomer.OnDestinationReachCB.RemoveAllListeners();
        iCustomer.OnDestinationReachCB.AddListener(() => 
        {
            try
            {
                Destroy(iCustomer.gameObject);
            } catch (MissingReferenceException e)
            {
                // swallow
            }
        } );
    }

    public void ClearCustomers()
    {
        spawnedCustomers.ForEach(c => c.OnDestinationReachCB?.Invoke());
        spawnedCustomers = new List<Customer>();
    }

    public void AllLeaveShop()
    {
        Customer c = null;
        while(ref_customerQ.Peek() != null)
        {
            c = ref_customerQ.Dequeue();
            LeaveShop(c); 
        }
    }

}
