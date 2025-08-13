using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
public class Card : MonoBehaviour, ITapTracker
{
    public UnityEvent<int> tapCB;
    private Vector3 baseScale;
    private Vector3 animScale;
    public float onTapScalingCoef = 1.2f;
    public Collider2D cardCollider;
    public float tapAnimDuration = 0.1f;
    Coroutine tapAnimCo;

    public bool hidden;
    public int index = -1;

    public string content;
    public int timeToReadOnWrong = 500; // in ms
    public float timeToReadOnRight = 0.2f;

    public float shrinkAnimDuration = 0.25f;
    public ParticleSystem winParticles;
    
    
    void Start()
    {
        hidden = true;
        cardCollider = GetComponent<Collider2D>();
        baseScale = transform.localScale;
        animScale = baseScale * onTapScalingCoef;
    }


    public void OnTap(Vector2 iVec2)
    {
        if (cardCollider.bounds.Contains(iVec2))
        {
            tapCB.Invoke(index);
        }
    }

    public void TapEffect()
    {
        if(hidden && tapAnimCo == null)
        {
            hidden = false;
            tapAnimCo = StartCoroutine(TapAnim());
        }
    }

    public async void HideCard()
    {
        await Task.Delay(timeToReadOnWrong);
        hidden = true;
        if (tapAnimCo != null)
        {
            StopCoroutine(TapAnim());
            tapAnimCo = null;
        }
        StartCoroutine(TapAnim());
    }

    public void WinCard()
    {
        StartCoroutine(WinAnim());

    }
    private IEnumerator WinAnim()
    {
        yield return new WaitForSeconds(timeToReadOnRight);
        float startTime = Time.time;
        baseScale = transform.localScale;

        winParticles.Play();
        while ((Time.time - startTime) < shrinkAnimDuration)
        {
            float frac = (Time.time - startTime) / shrinkAnimDuration;
            transform.localScale = (1 - frac) * baseScale;
            yield return null;

        }
        transform.position = new Vector3(10, 0, 0);
    }

    IEnumerator TapAnim()
    {
        float startTime = Time.time;
        transform.localScale = baseScale;
        Quaternion to = transform.rotation * Quaternion.Euler(0,transform.rotation.y + 180,0);

        while ((Time.time - startTime) < tapAnimDuration)
        {
            float frac = (Time.time - startTime) / tapAnimDuration;
            if (frac < 0.5f)
                transform.localScale = Vector3.Slerp(baseScale, animScale, frac*2);
            else
                transform.localScale = Vector3.Slerp(animScale, baseScale, frac);

            transform.rotation = Quaternion.Slerp(transform.rotation,to,frac);
            yield return null;
        }
        transform.rotation = to;
        transform.localScale = baseScale;
    }
}
