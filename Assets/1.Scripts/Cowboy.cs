using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class Cowboy : MonoBehaviour, ITapTracker
{
    public enum State
    {
        Iddle,
        Dodge,
        Distracted,
        Hit
    }
    public UnityEvent hitCB;
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
    private Vector3 baseScale;
    public Collider2D hitbox;
     [HideInInspector] public float difficultyTimeCoef;

    private float nextStateTime;

    private State currentState = State.Iddle;
    public bool stopPropagation => true;
    public int GetDisplayPriority(){ return cowboySR.sortingOrder; }
    void Start()
    {
        SetState(State.Iddle);
        baseScale = transform.localScale;
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
        else if (currentState == State.Hit)
        {
            float frac = nextStateTime / deathAnimDuration;
            transform.localScale = Vector3.Slerp(Vector3.zero,baseScale, frac);
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
                break;
            case State.Dodge:
                cowboySR.sprite = dodgeSprite;
                nextStateTime = Random.Range(minDodgeTime, maxDodgeTime);
                break;
            case State.Distracted:
                cowboySR.sprite = distractedSprite;
                nextStateTime = Random.Range(minDistractedTime, maxDistractedTime);
                break;
            case State.Hit:
                cowboySR.sprite = hitSprite;
                nextStateTime = deathAnimDuration;
                break;
        }
    }


    public async UniTaskVoid StateLogic(float waitTime)
    {
        await UniTask.WaitForSeconds(waitTime / difficultyTimeCoef);
    }

    public bool OnTap(Vector2 pos)
    {
        if (hitbox.bounds.Contains(pos))
        {
            if (currentState == State.Distracted)
            {
                SetState(State.Hit);
                hitCB.Invoke();
            }
            else if (currentState == State.Iddle)
            {
                SetState(State.Dodge);
            }
            return true;
        }
        return false;
    }

    public async void DestroySelf()
    {
        await UniTask.WaitForSeconds(deathAnimDuration);
        if(this.gameObject != null)
            Destroy(this.gameObject);
    }




}
