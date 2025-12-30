using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerCommandBtn : MonoBehaviour
{
    public UIButton btn;
    public Image itemImage;

    public void SetCmdSprite(Sprite iSprite)
    {
        if (itemImage == null)
            return;
        itemImage.sprite = iSprite;
    }
}
