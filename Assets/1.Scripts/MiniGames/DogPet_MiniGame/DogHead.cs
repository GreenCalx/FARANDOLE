using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DogHead : MonoBehaviour, ITapTracker
{
    float fadeOutAnimDuration = 0.5f;

    public UnityEvent tapCB;
    Rigidbody2D rb2d;
    public Collider2D h_C2D;
    public Rigidbody2D AnchorPointRB;
    public SpriteRenderer SR;
    public Sprite idleSprite;
    public Sprite tapSprite;
    public Sprite onWinSprite;
    public float tapAnimDuration = 0.1f;
    [Header("Pop Out Anim")]
    public ParticleSystem PS;
    public ParticleSystem OnPetPS;
    public float torque = 10f;
    Coroutine tapAnimCo;
    Vector3 baseScale;

    Quaternion baseRot;
    Vector3 animScale;
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return 0; }
    Bounds m_Bounds;
    bool isBounded = false;
    public bool bounceRandomOnTap = false;
    SpringJoint2D m_Spring;
    public bool DrawSpring;
    LineRenderer m_SpringLR;
    bool m_SpringAnchorDrag = false;
    public bool SpringAnchorDrag
    {
        get { return ((AnchorPointRB != null) && m_SpringAnchorDrag); }
        set { m_SpringAnchorDrag = value; }
    }
    public float m_SpringAnchorDragStrength;
    public Material springMat;
    public float springLRWidthStart = 0.1f;
    public float springLRWidthEnd = 0.1f;
    Vector3[] springPoints;
    public void Init()
    {
        rb2d = GetComponent<Rigidbody2D>();

        SR.sprite = idleSprite;
        baseScale = SR.transform.localScale;
        animScale = baseScale * 0.9f;
        baseRot = transform.rotation;

        springPoints = new Vector3[2];
    }

    public void InitBounds(Bounds iBounds)
    {
        m_Bounds = iBounds;
        isBounded = true;
    }

    public void Reset()
    {
        rb2d.constraints = RigidbodyConstraints2D.None;
        SR.transform.localScale = baseScale;
        SR.sprite = idleSprite;

        transform.position = Vector3.zero;
        transform.rotation = baseRot;

        AnchorPointRB.linearVelocity = Vector3.zero;
        AnchorPointRB.transform.position = Vector3.zero;
        
        m_Spring = GetComponent<SpringJoint2D>();
        if (DrawSpring)
        {
            m_SpringLR = GOBuilder.Create()
                    .WithName("SpringLR")
                    .WithParent(transform)
                    .WithPosition(Vector3.zero)
                    .WithLineRenderer(springMat)
                    .BuildAs<LineRenderer>();
            m_SpringLR.positionCount = 2;
            m_SpringLR.startWidth = springLRWidthStart;
            m_SpringLR.endWidth = springLRWidthEnd;
            SpringLRUpdate();
        }
            
    }
    
    void SpringLRUpdate()
    {
        if (m_SpringLR==null)
            return;
        springPoints[0] = transform.position;
        springPoints[1] = AnchorPointRB!=null ? AnchorPointRB.transform.position : Vector3.zero;
        m_SpringLR.SetPositions(springPoints);
    }


    void OnDestroy()
    {
        if (tapAnimCo != null)
        {
            StopCoroutine(tapAnimCo);
            tapAnimCo = null;
        }

    }

    void OnDisable()
    {
        if (m_SpringLR != null)
            Destroy(m_SpringLR.gameObject);
    }
    void FixedUpdate()
    {
        if (isBounded)
        {
            if (!m_Bounds.Contains(transform.position))
            {
                transform.position = m_Bounds.ClosestPoint(transform.position);
                return;
            }
        }

        if (m_SpringAnchorDrag)
        {
            m_Spring.anchor = AnchorPointRB.transform.position;
        }
    }

    void Update()
    {
        if (DrawSpring)
        {
            SpringLRUpdate();
        }
    }

    public void StopAnim()
    {
        if (tapAnimCo!=null)
            StopCoroutine(tapAnimCo);
        tapAnimCo = null;
    }
    public bool OnTap(Vector2 iVec2)
    {
        if (!h_C2D.bounds.Contains(iVec2))
            return false;

        tapCB.Invoke();
        OnPetPS.Play();

        return false;
    }
    public void TapEffect(int force)
    {
        SR.sprite = tapSprite;
        if (rb2d)
        {
            Vector3 force_dir = Vector3.zero;
            force_dir = bounceRandomOnTap ? UnityEngine.Random.insideUnitCircle.normalized : -Vector3.up;
            force_dir *= force;
            rb2d.AddForce(force_dir, ForceMode2D.Impulse);
            if (m_SpringAnchorDrag)
            {
                AnchorPointRB.AddForce(force_dir * m_SpringAnchorDragStrength, ForceMode2D.Impulse);
            }
        }
        if (tapAnimCo != null)
        {
            StopCoroutine(tapAnimCo);
            tapAnimCo = null;
        }
        tapAnimCo = StartCoroutine(TapAnim());
    }

    IEnumerator TapAnim()
    {
        float startTime = Time.time;
        SR.transform.localScale = baseScale;
        while ((Time.time - startTime) < tapAnimDuration)
        {
            float frac = (Time.time - startTime) / tapAnimDuration;
            if (frac < 0.5f)
                SR.transform.localScale = Vector3.Lerp(baseScale, animScale, frac * 2);
            else
                SR.transform.localScale = Vector3.Lerp(animScale, baseScale, frac);
            yield return null;
        }
        SR.transform.localScale = baseScale;
        SR.sprite = idleSprite;

    }

    public void FadeOutAnim()
    {
        if (tapAnimCo != null)
            StopCoroutine(tapAnimCo);
        tapAnimCo = StartCoroutine(FadeOutAnimCo());
    }

    IEnumerator FadeOutAnimCo()
    {
        rb2d.linearVelocity = Vector3.zero;
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        // spin

        // shrink to zero scale
        float startAnimTime = Time.time;
        float lerpFactor = 1f;
        while (lerpFactor > 0f)
        {
            rb2d.AddTorque(torque, ForceMode2D.Impulse);

            lerpFactor = 1f - ((Time.time - startAnimTime) / fadeOutAnimDuration);
            SR.transform.localScale = baseScale * lerpFactor;
            if (DrawSpring)
            {
                m_SpringLR.startWidth = Utils.Lerp(0f , springLRWidthStart, lerpFactor);
            }
            yield return null;
        }
        rb2d.constraints = RigidbodyConstraints2D.None;
    }
}
