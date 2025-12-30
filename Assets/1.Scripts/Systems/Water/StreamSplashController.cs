using UnityEngine;

public class StreamSplashController : MonoBehaviour
{
    public PourStreamSimulator stream;
    public ParticleSystem splash;
    public StreamFilled cup;

    public float minInterval = 0.08f;

    float lastSplashTime;

    void LateUpdate()
    {
        if (!stream.IsHittingCup)
            return;

        if (Time.time - lastSplashTime < minInterval)
            return;

        lastSplashTime = Time.time;

        //splash.transform.position = stream.HitPoint;
        Vector2 splashPos = stream.HitPoint;
        splashPos.y = cup.splashAnchor.position.y;
        splash.transform.position = splashPos;

        splash.transform.rotation = Quaternion.identity;
        splash.transform.up = -stream.HitNormal;

        splash.Emit(Random.Range(3, 6));
    }
}
