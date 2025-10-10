using System.Collections.Generic;
using UnityEngine;


public class Tower : ChessPiece
{
    override
    public List<Tile> GetLegalMoves()
    {
        List<Tile> moves = new List<Tile>();
        int[] dx = {1,0,-1,0};
        int[] dy = {0,1,0,-1};


        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x;
            int ny = y;
            while (true)
            {
                nx += dx[dir];
                ny += dy[dir];
                Tile t = board.GetTile(nx, ny);
                if (t == null) break;


                if (!t.IsOccupied())
                {
                    moves.Add(t);
                }
                else
                {
                if (t.GetOccupant().Color != this.Color)
                    moves.Add(t);
                    break;
                }
            }
        }
        return moves;
    }
}