using UnityEngine;
using TMPro;
public class FPSCounter : MonoBehaviour
{
    public float timer, refresh, avgFramerate;
    string display = "{0}";
    public TextMeshProUGUI m_Text;

    private void Update()
    {
        if (m_Text == null)
            return;

        //Change smoothDeltaTime to deltaTime or fixedDeltaTime to see the difference
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if(timer <= 0) avgFramerate = (int) (1f / timelapse);
        m_Text.text = string.Format(display,avgFramerate.ToString());
    }
}
