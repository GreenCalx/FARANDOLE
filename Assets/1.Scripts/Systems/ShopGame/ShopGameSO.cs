using UnityEngine;

[CreateAssetMenu(fileName = "ShopGameSO", menuName = "Scriptable Objects/ShopGameSO")]
public class ShopGameSO : ScriptableObject
{
    public float WaitLineItemSpacing;
    public int MaxItems;
    public float WaitLineCustomerSpacing;
    public int MaxCustomers;
}