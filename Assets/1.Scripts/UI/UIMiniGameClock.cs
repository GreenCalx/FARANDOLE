using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class UIMiniGameClock : MonoBehaviour
{
    public TextMeshProUGUI m_UIText;
    float animDuration = 0.5f;
    Vector3 initScale;
    public string text
    {
        set
        {
            if (m_UIText.text != value)
            {
                Animate();
                m_UIText.text = value;
            }
            
        }
        get { return m_UIText.text; }
    }

    void Start()
    {
        initScale = transform.localScale;
        animDuration = GameData.GetSettings.GameClockScaleAnimDuration;
    }

    void Animate()
    {
        m_UIText.transform.localScale = Vector3.zero;
        transform.DOScale(initScale, animDuration)
            .SetEase(Ease.Linear);
    }
}
