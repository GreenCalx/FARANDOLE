using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
//using Unity.Android.Gradle.Manifest;

public abstract class ChessPiece : MonoBehaviour
{
    public int x, y;
    public PlayerColor color { get; private set; }

    public Sprite normalPose;
    public Sprite attackPose;
    public Sprite deathPose;
    public Sprite blackPiece;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    protected ChessBoard board;

    public float deathPushAnim = 0.5f;
    private ParticleSystem particles;
    public float poseTime;

    public void Init(int x, int y, PlayerColor pcolor, ChessBoard board)
    {
        this.x = x; this.y = y; this.color = pcolor; this.board = board;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        particles = GetComponentInChildren<ParticleSystem>();
        if (color == PlayerColor.Black)
        {
            sr.sprite = blackPiece;
        }
        else
        {
            sr.sprite = normalPose;
        }
        transform.localScale = Vector3.one * (board.tileSize * 0.9f);
    }


    public void SetSprite(Sprite s)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = 1;
    }

    public void SetPosition(int newX, int newY, float duration = 0.1f)
    {
        x = newX; y = newY;
        transform.DOMove(board.GetTile(x, y).transform.position, duration);
    }

    public void GetPos(out int posX, out int posY)
    {
        posX = x;
        posY = y;
    }
    public abstract List<Tile> GetLegalMoves();

    public async UniTask SpecialPose()
    {
        sr.sprite = attackPose;
        await UniTask.WaitForSeconds(poseTime);
        if (gameObject != null) //in case of promote
            sr.sprite = normalPose;
    }

    public async void Die(Vector2 expulsionVector)
    {

        rb.AddForce(expulsionVector * 5f, ForceMode2D.Impulse);
        sr.sprite = deathPose;
        await UniTask.WaitForSeconds(deathPushAnim);
        playParticles();
        sr.enabled = false;
        await UniTask.WaitForSeconds(particles.main.startLifetime.constantMax);

        if (this.gameObject!=null)
            Destroy(this.gameObject);
    }

    public void playParticles()
    {
        var main = particles.main;
        if (color == PlayerColor.White)
            main.startColor = new Color(1, 1, 1, 1);
        else
            main.startColor = new Color(0, 0, 0, 1);
        particles.Play();
    }

    
    public virtual Vector2 GetExpulsionVector()
    {
        return Vector2.zero;
    }
}
