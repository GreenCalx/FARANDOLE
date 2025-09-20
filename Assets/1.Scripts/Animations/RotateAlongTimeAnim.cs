using UnityEngine;
using static Utils;
public class RotateAlongTimeAnim : MonoBehaviour
{
    public GameClock clock;
    [Header("Tweaks")]
    public float offsetAngle = 90;

    bool init = false;
    Quaternion initRot;
    float current_angle = 0f;
    float last_angle = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(GameClock iClock)
    {
        initRot = transform.rotation;
        clock = iClock;
        init = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
            return;
        if (!clock.IsFrozen)
        {
            current_angle = initRot.z + Utils.Lerp(360f, 0f, clock.GetSeconds());
            transform.Rotate(0f, 0f, current_angle - last_angle);
            last_angle = current_angle;
        }
        else
            transform.rotation = initRot;
    }
}
