using UnityEngine;

public class UIPauseMenu : MonoBehaviour
{
    public InjectBlurPass h_blurEffect;
    public Transform h_BtnLayout;
    public Transform h_PauseText;
    public UIButton h_TryAgainBtn;
    public UIButton h_ExitBtn;
    float m_TimeScale;
    bool m_Toggle = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Toggle()
    {
        m_Toggle = !m_Toggle;
        if (m_Toggle)
            Pause();
        else
            UnPause();
    }

    // Update is called once per frame
    void Pause()
    {
        m_TimeScale = Time.timeScale;
        Time.timeScale = 0f;
        h_blurEffect.gameObject.SetActive(true);
        h_PauseText.gameObject.SetActive(true);
        h_BtnLayout.gameObject.SetActive(true);
    }

    void UnPause()
    {
        h_blurEffect.gameObject.SetActive(false);
        h_PauseText.gameObject.SetActive(false);
        h_BtnLayout.gameObject.SetActive(false);
        Time.timeScale = m_TimeScale;
    }
}
