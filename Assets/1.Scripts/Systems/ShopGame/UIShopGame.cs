using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UIShopGame : MonoBehaviour
{
    public ShopOrderManager shopOrderManager;
    public Transform handle_PlayerActions;
    public GameObject prefab_commandBtn;
    private List<UIPlayerCommandBtn> commandBtns;
    public ShopItemCollectionSO itemCollection;
    void Start()
    {
        InitCommandButtons();
    }
    private void InitCommandButtons()
    {
        commandBtns = new List<UIPlayerCommandBtn>();
        foreach (ShopItemSO item in itemCollection.items)
        {
            GameObject newBtn = Instantiate(prefab_commandBtn);
            newBtn.transform.SetParent(handle_PlayerActions);

            UIPlayerCommandBtn as_btn = newBtn.GetComponent<UIPlayerCommandBtn>();
            if (as_btn == null)
            {
                //Debug.LogError("Button component missing on PlayerUI::prefab_commandBtn");
                Destroy(newBtn);
                continue;
            }
            as_btn.btn.clickCallback.AddListener(
                () =>
                {
                    as_btn.btn.interactable = false;
                    shopOrderManager.PlaceOrder(item, () => as_btn.btn.interactable = true);
                }
                
            );
            as_btn.SetCmdSprite(item.icon);
            commandBtns.Add(as_btn);
        }
    }
}