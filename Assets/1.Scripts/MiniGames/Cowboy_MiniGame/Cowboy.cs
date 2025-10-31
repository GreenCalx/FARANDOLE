using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

using DG.Tweening;
public class Cowboy : MonoBehaviour, ITapTracker, IRendered, ISpawnable
{
    public enum State
    {
        Iddle,
        Dodge,
        Distracted,
        Hit
    }
    public UnityEvent<Cowboy> hitCB;
    public SpriteRenderer cowboySR;
    public Sprite iddleSprite;
    public Sprite dodgeSprite;
    public Sprite distractedSprite;
    public Sprite hitSprite;

    public float minIddleTime;
    public float maxIddleTime;

    public float minDodgeTime;
    public float maxDodgeTime;
    public float minDistractedTime;
    public float maxDistractedTime;

    public float deathAnimDuration;

    public float patrolTime;
    private Vector3 baseScale;
    public Collider2D hitbox;
    [HideInInspector] public float difficultyTimeCoef = 1;

    private float nextStateTime;

    private State currentState = State.Iddle;
    public bool stopPropagation => true;
    float xMinBound, xMaxBound, startX;
    float firstDestination, secondDestination;
    public int GetDisplayPriority() { return cowboySR.sortingOrder; }
    void Start()
    {
        baseScale = transform.localScale;
    }

    public void SetMovement(float minBound, float maxBound)
    {
        xMinBound = minBound;
        xMaxBound = maxBound;
        startX = Random.Range(xMinBound, xMaxBound);
        bool startGoingLeft = Random.value > 0.5f;
        transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        firstDestination = startGoingLeft ? xMinBound : xMaxBound;
        secondDestination = startGoingLeft ? xMaxBound : xMinBound;

        transform.DOMoveX(firstDestination, patrolTime * Mathf.Abs(startX - firstDestination) / (xMaxBound - xMinBound))
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                     transform.DOMoveX(secondDestination, patrolTime)
                              .SetEase(Ease.Linear)
                              .SetLoops(-1, LoopType.Yoyo); // boucle infinie
                 });

        SetState(State.Iddle);
    }
    
    public void DoMovement()
    {
        transform.DOPlay();
    }

    public void StopMovement()
    {
        transform.DOPause();
    }

    public void DeathAnimation(Vector2 iDestination)
    {
        // transform.DOMove(iDestination, deathAnimDuration, false)
        //     .SetEase(Ease.Linear)
        //     .OnComplete(() => GameObject.Destroy(gameObject));
        transform.DOScale(Vector3.zero, deathAnimDuration)
            .SetEase(Ease.InOutQuint);
        // transform.DORotate(new Vector3(0f, 0f, 360f), deathAnimDuration, RotateMode.FastBeyond360)
        //             .SetEase(Ease.Linear)
        //             .OnComplete(() => GameObject.Destroy(gameObject));
    }

    void FixedUpdate()
    {
        nextStateTime -= Time.fixedDeltaTime;

        if (nextStateTime <= 0)
        {
            switch (currentState)
            {
                case State.Iddle:     
                    SetState(State.Distracted);
                    break;
                case State.Dodge:
                    SetState(State.Iddle);
                    break;
                case State.Distracted:
                    SetState(State.Iddle);
                    break;
            }
        }

    }

    void SetState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.Iddle:
                cowboySR.sprite = iddleSprite;
                nextStateTime = Random.Range(minIddleTime, maxIddleTime);
                DoMovement();
                break;
            case State.Dodge:
                cowboySR.sprite = dodgeSprite;
                nextStateTime = Random.Range(minDodgeTime, maxDodgeTime);
                StopMovement();
                break;
            case State.Distracted:
                cowboySR.sprite = distractedSprite;
                nextStateTime = Random.Range(minDistractedTime, maxDistractedTime);
                StopMovement();
                break;
            case State.Hit:
                cowboySR.sprite = hitSprite;
                nextStateTime = deathAnimDuration;
                break;
        }
    }

    // ITapTracker
    public bool OnTap(Vector2 pos)
    {
        if (Utils.IsContained2D(pos, hitbox.bounds))
        {
            if (currentState == State.Distracted)
            {
                SetState(State.Hit);
                hitCB.Invoke(this);
            }
            else if (currentState == State.Iddle)
            {
                SetState(State.Dodge);
            }
            return true;
        }
        return false;
    }

    // IRendered
    public Renderer GetRenderer()
    {
        return cowboySR;
    }
    public Renderer GetStickerRenderer()
    {
        return null;
    }
}
