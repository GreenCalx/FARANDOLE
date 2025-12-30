using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CustomerCollectionSO", menuName = "Scriptable Objects/CustomerCollectionSO")]
public class CustomerCollectionSO : ScriptableObject
{
    public List<ShopCustomerSO> customers;
}
