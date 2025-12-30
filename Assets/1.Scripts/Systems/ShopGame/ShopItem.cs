using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
public class ShopItem : MonoBehaviour
{
    public string itemName;
    public ITEM_TYPE itemType;
    public float craftDuration = 0f;
    private float elapsedCraftTime = 0f;
    public bool isCrafted = false;
    public SpriteRenderer SR; 
    public int value = 1;
    public void Init(ShopItemSO iItemDef)
    {
        itemName = iItemDef.itemName;
        craftDuration = iItemDef.craftTime;
        itemType = iItemDef.itemType;
        value = iItemDef.sellValue;

        SR.sortingOrder = iItemDef.sortingOrder;
        SR.sortingLayerName = iItemDef.sortingMaskName;
        Craft();
    }

    public async UniTask WaitCraft()
    {
        while (!isCrafted)
        { await Task.Yield(); }
        return;
    }

    public void Craft()
    {
        isCrafted = false;
        elapsedCraftTime = 0f;
        SR?.material.SetColor("_Color", Color.black);

    }

    public void OnCrafted()
    {
        isCrafted = true;
        SR?.material.SetColor("_Color", Color.white);
    }

    void Update()
    {
        if (!isCrafted)
        {
            elapsedCraftTime += Time.deltaTime;
            if (elapsedCraftTime >= craftDuration)
            {
                OnCrafted();
            }
        }
    }
}
