using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
//using Unity.Android.Gradle.Manifest;

public abstract class ChessPiece : MonoBehaviour
{
    public int x, y;
    public PlayerColor Color { get; private set; }

    public Sprite normalPose;
    public Sprite specialPose;
    private SpriteRenderer sr;
    protected ChessBoard board;

    private ParticleSystem particles;
    public float poseTime;

    public void Init(int x, int y, PlayerColor color, ChessBoard board)
    {
        this.x = x; this.y = y; this.Color = color; this.board = board;
        sr = GetComponent<SpriteRenderer>();
        if(color == PlayerColor.Black)
            particles = GetComponentInChildren<ParticleSystem>();
        transform.localScale = Vector3.one * (board.tileSize * 0.8f);
    }


    public void SetSprite(Sprite s)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = 1;
    }


    public void SetPosition(int newX, int newY)
    {
        x = newX; y = newY;
        transform.position = board.GetTile(x, y).transform.position;
    }

    public abstract List<Tile> GetLegalMoves();

    public async UniTask SpecialPose()
    {
        sr.sprite = specialPose;
        await UniTask.WaitForSeconds(poseTime);
        sr.sprite = normalPose;
    }

    public async void Die()
    {
        particles.Play();
        //await SpecialPose();
        sr.enabled = false;
        await UniTask.WaitWhile(() => particles.isPlaying);
        Destroy(this);
    }
}
