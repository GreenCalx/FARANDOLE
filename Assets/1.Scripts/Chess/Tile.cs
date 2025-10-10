
using UnityEngine;
public class Tile : MonoBehaviour, ITapTracker
{
    public bool stopPropagation => true;

    public int x, y;
    private ChessBoard board;
    private ChessPiece occupant;
    private SpriteRenderer sr;

    private BoxCollider2D col;
    Color normalColor = new Color(1f,1f,1f,1f);
    Color highlightColor = new Color(0.6f,1f,0.6f,1f);

    public void Init(int x, int y, ChessBoard board)
    {
        this.x = x; this.y = y; this.board = board;
        transform.localScale = Vector3.one * board.tileSize;
        sr = GetComponent<SpriteRenderer>();
        bool dark = (x + y) % 2 == 1;
        sr.color = dark ? new Color(0.5f,0.5f,0.5f,1f) : new Color(1f,1f,1f,1f);

        col = GetComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }


    public void Highlight(bool on)
    {
        if (sr == null) return;
        if (on) sr.color = highlightColor;
        else
        {
            bool dark = (x + y) % 2 == 1;
            sr.color = dark ? new Color(0.5f,0.5f,0.5f,1f) : new Color(1f,1f,1f,1f);
        }
    }

    public bool OnTap(Vector2 touch)
    {
        if (col.bounds.Contains(touch))
        {
            board.TileTouched(this);
            return true;
        }
        return false;
    }

    public int GetDisplayPriority(){ return 0; }
    public void SetOccupant(ChessPiece k) { occupant = k; }
    public void ClearOccupant() { occupant = null; }
    public bool IsOccupied() { return occupant != null; }
    public ChessPiece GetOccupant() { return occupant; }
}