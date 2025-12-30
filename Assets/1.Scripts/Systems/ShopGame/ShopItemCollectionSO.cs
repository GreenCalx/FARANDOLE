using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopItemCollectionSO", menuName = "Scriptable Objects/ShopItemCollectionSO")]
public class ShopItemCollectionSO : ScriptableObject
{
    public List<ShopItemSO> items;
}
