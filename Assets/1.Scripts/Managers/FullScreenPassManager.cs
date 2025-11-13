using UnityEngine;
using UnityEngine.Rendering.Universal;
public class FullScreenPassManager : MonoBehaviour
{
    public MeltRendererFeature meltRendererFeature;
    public RippleRendererFeature rippleRendererFeature;
    private static FullScreenPassManager instance = null;
    public static FullScreenPassManager Get => instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    public bool rippleEnabled = false;
    public bool meltEnabled = false;

    void Start()
    {
        DisableAll();
    }

    public void DisableAll()
    {
        Debug.Log(" Disabled");
        meltRendererFeature.EnableBlit = false;
        rippleRendererFeature.EnableBlit = false;
    }

    public void EnableRippleSolo(bool iState)
    {
        DisableAll();
        Debug.Log(" Ripple enabled : " + iState);
        rippleEnabled = iState;
        rippleRendererFeature.EnableBlit = iState;
    }

    public void EnableMeltSolo(bool iState)
    {
        DisableAll();
        Debug.Log(" Melt enabled" + iState);
        meltEnabled = iState;
        meltRendererFeature.EnableBlit = iState;
    }
    
}
