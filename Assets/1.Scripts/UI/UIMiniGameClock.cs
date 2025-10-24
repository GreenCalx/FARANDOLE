using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class UIMiniGameClock : MonoBehaviour
{
    public TextMeshProUGUI m_UIText;
    float animDuration = 0.5f;
    Vector3 initScale;
    bool initDone = false;
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

        initDone = true;
    }

    void Animate()
    {
        // Otherwise there is a bug at init where UIGame calls setText before this class
        // calls Start, thus initScale is still 0 and because we modify the localScale here,
        // when start is called initScale will be set to Vector3.zero.
        if (!initDone)
            return;

        m_UIText.transform.localScale = Vector3.zero;
        transform.DOScale(initScale, animDuration)
            .SetEase(Ease.Linear);
    }
}
