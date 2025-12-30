using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public static class ItemFactory
{
    public static ShopItem Make(ShopItemSO iItemDef)
    {
        GameObject newItem = new GameObject();
        newItem.name = iItemDef.itemName;
        newItem.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        // Decorate here
        SpriteRenderer sr = newItem.AddComponent<SpriteRenderer>();

        sr.material = iItemDef.mat;
        sr.sprite = iItemDef.icon;

        ShopItem item = newItem.AddComponent<ShopItem>();
        item.SR = sr;
        item.Init(iItemDef);

        return item;
    }
}
