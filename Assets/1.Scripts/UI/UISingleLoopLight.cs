using UnityEngine;
using UnityEngine.UI;

public class UISingleLoopLight : MonoBehaviour
{
    public Image lightImage;

    public void TurnOn(Color iColor)
    {
        lightImage.color = iColor;
        lightImage.enabled = true;
        
    }

    public void TurnOff()
    {
        lightImage.enabled = false;
    }
}
