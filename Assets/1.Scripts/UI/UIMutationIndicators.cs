using UnityEngine;
using UnityEngine.UI;

public class UIMutationIndicators : MonoBehaviour
{
    public RectTransform h_AlphaMut;
    public RectTransform h_BetaMut;

    public void Refresh(MiniGame iMiniGame)
    {
        h_AlphaMut.gameObject.SetActive( iMiniGame.AlphaMut !=null );
        h_BetaMut.gameObject.SetActive( iMiniGame.BetaMut !=null );
    }
}
