using UnityEngine;

public enum ITEM_TYPE
{
    NONE = 0,
    CROISSANT = 1,
    BREAD=2,
    FLATBREAD=3
}

[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public string itemName = "";
    public ITEM_TYPE itemType;
    public float craftTime = 1f;
    public Material mat;
    public Sprite icon;
    public string sortingMaskName = Constants.LYR_GAME;
    public int sortingOrder;
    public int sellValue = 5;
}