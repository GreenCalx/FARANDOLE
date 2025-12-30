using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopCustomerSO", menuName = "Scriptable Objects/ShopCustomerSO")]
public class ShopCustomerSO : ScriptableObject
{
    public ShopItemCollectionSO availableItems;
    public Sprite sprite;
}
