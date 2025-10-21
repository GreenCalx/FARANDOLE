using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapOnWin : MonoBehaviour
{
    SpriteRenderer m_SR;
    public Sprite baseSprite;
    public Sprite onWinSprite;
    MiniGame m_TrackedMG;

    void Start()
    {
        m_SR = GetComponent<SpriteRenderer>();
        m_TrackedMG = GetComponentInParent<MiniGame>();
    }
    void OnEnable()
    {
        if (m_TrackedMG && m_TrackedMG.MGM)
        {
            m_SR.sprite = baseSprite;
            m_TrackedMG.MGM.autoSwappersOnWin.Add(this);
        }
    }
    void OnDisable()
    {
        if (m_TrackedMG && m_TrackedMG.MGM)
        {
            m_TrackedMG.MGM.autoSwappersOnWin.Remove(this);
        }
    }
    public void OnWinSwap()
    {
        m_SR.sprite = onWinSprite;
    }
}
