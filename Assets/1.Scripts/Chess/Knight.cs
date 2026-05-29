using System.Collections.Generic;
using UnityEngine;


public class Knight : ChessPiece
{
    override public List<Tile> GetLegalMoves()
    {
        List<Tile> moves = new List<Tile>();
        int[] dx = { 1, 2, 2, 1, -1, -2, -2, -1 };
        int[] dy = { 2, 1, -1, -2, -2, -1, 1, 2 };
        for (int i = 0; i < 8; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
            Tile t = board.GetTile(nx, ny);
            if (t == null) continue;

            if (!t.IsOccupied() || (t.IsOccupied() && t.GetOccupant().color != this.color))
            {
                moves.Add(t);
            }
        }
        return moves;
    }
    static public List<Tile> GetLegalMovesFromPos(int posx, int posy, ChessBoard board)
    {
        List<Tile> moves = new List<Tile>();
        int[] dx = { 1, 2, 2, 1, -1, -2, -2, -1 };
        int[] dy = { 2, 1, -1, -2, -2, -1, 1, 2 };
        for (int i = 0; i < 8; i++)
        {
            int nx = posx + dx[i];
            int ny = posy + dy[i];
            Tile t = board.GetTile(nx, ny);
            if (t != null)
                moves.Add(t);
        }
        return moves;
    }
    override
    public Vector2 GetExpulsionVector()
    {
        return -1f * Vector2.right;
    }
}