using UnityEngine;
using UnityEngine.Events;
public class Mole : MonoBehaviour, ITapTracker
{


    public UnityEvent tapCB;
    private float minTime;

    private float maxTime;

    private float timeBeforeGoingOut;
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return 0; }

    public Collider2D col;

    public Animator anim;

    public ParticleSystem bloodParticle;

    private bool inHole = false;
    public void Init(float minHoleDuration, float maxHoleDuration)
    {
        minTime = minHoleDuration;
        maxTime = maxHoleDuration;
        ResetClock();
        inHole = true;
    }

    void FixedUpdate()
    {
        if (inHole)
        {
            timeBeforeGoingOut -= Time.fixedDeltaTime;
            if (timeBeforeGoingOut <= 0)
            {
                anim.SetTrigger("Out");
                inHole = false;
                ResetClock();
            }         
        }


    }
    public bool OnTap(Vector2 pos)
    {
        if (!inHole && col.bounds.Contains(new Vector3(pos.x, pos.y, transform.position.z)))
        {
            bloodParticle.Play();
            tapCB.Invoke();
            anim.SetTrigger("Hit");
            return true;
        }
        return false;
    }

    void ResetClock()
    {
        timeBeforeGoingOut = Random.Range(minTime, maxTime);
    }

    public void MoleInHole()
    {
        ResetClock();
        Debug.Log("mole in hole " + name);
        inHole = true;
    }
}
