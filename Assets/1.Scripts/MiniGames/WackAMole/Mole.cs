using UnityEngine;
using UnityEngine.Events;
public class Mole : MonoBehaviour, ITapTracker, IRendered, ISpawnable
{

    public SpriteRenderer m_SR;
    public UnityEvent tapCB;
    private float timeToSpendOut;

    private float timeOut;

    private float timeBeforeGoingOut;
    private float elapsedTimeOut;
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return 0; }

    public Collider2D col;

    public Animator anim;

    public ParticleSystem bloodParticle;
    public UnityEvent<int,int> OnHideCB;
    public Vector2Int matrixCoordinates;
    public bool isOut;
    public void Init(float iTimeOut)
    {
        timeToSpendOut = iTimeOut;
        elapsedTimeOut = 0f;
        isOut = false;
    }

    #region ISpawnable

    #endregion

    void Update()
    {
        if (isOut)
        {
            elapsedTimeOut += Time.fixedDeltaTime;
            if (elapsedTimeOut > timeToSpendOut)
            {
                GoIn();
            }
        }
    }
    
    public void GoOut()
    {
        anim.ResetTrigger("In");
        isOut = true;
        anim.SetTrigger("Out");
    }

    public void GoIn()
    {
        anim.SetTrigger("In");
        OnHideCB.Invoke(matrixCoordinates.x, matrixCoordinates.y);
        elapsedTimeOut = 0f;
    }

    public bool OnTap(Vector2 pos)
    {
        if (col.bounds.Contains(new Vector3(pos.x, pos.y, transform.position.z)))
        {
            bloodParticle.Play();
            tapCB.Invoke();
            anim.SetTrigger("Hit");
            OnHideCB.Invoke(matrixCoordinates.x, matrixCoordinates.y);
            elapsedTimeOut = 0f;
            return true;
        }
        return false;
    }

    public void OnWin()
    {
        if(isOut)
            anim.SetTrigger("Win");
    }
    // IRendered
    public Renderer GetRenderer()
    {
        return m_SR;
    }

    public Renderer GetStickerRenderer()
    {
        return null;
    }
}
