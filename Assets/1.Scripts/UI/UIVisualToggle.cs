using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIVisualToggle : MonoBehaviour
{
    public Transform Point_On;
    public Transform Point_Off;
    public Transform ToggleKnob;
    public SpriteRenderer FreezeVisual;
    public TextMeshProUGUI Text_On;
    public TextMeshProUGUI Text_Off;
    public Color Color_On;
    public Color Color_Off;

    public bool freeze
    {
        set { FreezeVisual.enabled = value; }
    }
    public void SetOn()
    {
        ToggleKnob.transform.position = Point_On.position;
        Text_On.color = Color_On;
        Text_Off.color = Color_Off;
    }

    public void SetOff()
    {
        ToggleKnob.transform.position = Point_Off.position;
        Text_Off.color = Color_On;
        Text_On.color = Color_Off;
    }

    public void Toggle(bool iState)
    {
        if (iState)
            SetOn();
        else
            SetOff();
    }
}
