using System.Collections.Generic;
using UnityEngine;


public class King : ChessPiece
{
    override public List<Tile> GetLegalMoves()
    {
        List<Tile> moves = new List<Tile>();
        int[] dx = {1,1,0,-1,-1,-1,0,1};
        int[] dy = {0,1,1,1,0,-1,-1,-1};


        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x;
            int ny = y;
            nx += dx[dir];
            ny += dy[dir];
            Tile t = board.GetTile(nx, ny);
            if (t == null)
                continue;
                
            if (!t.IsOccupied())
            {
                moves.Add(t);
            }
            else if (t.GetOccupant().color != this.color)
                moves.Add(t);
        }
        
        return moves;
    }
}