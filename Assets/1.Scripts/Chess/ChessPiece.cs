using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public int x, y;
    public PlayerColor Color { get; private set; }

    private SpriteRenderer sr;
    protected ChessBoard board;


    public void Init(int x, int y, PlayerColor color, ChessBoard board)
    {
        this.x = x; this.y = y; this.Color = color; this.board = board;
        sr = GetComponent<SpriteRenderer>();
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
}
