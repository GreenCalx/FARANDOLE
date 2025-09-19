using System.Collections.Generic;
using UnityEngine;


public class Pawn : ChessPiece
{

    override public List<Tile> GetLegalMoves()
    {
        List<Tile> moves = new List<Tile>();
        int dir = (Color == PlayerColor.White) ? 1 : -1;

        Tile forward = board.GetTile(x, y + dir);
        if (forward != null && !forward.IsOccupied())
            moves.Add(forward);

        Tile diagLeft = board.GetTile(x - 1, y + dir);
        Tile diagRight = board.GetTile(x + 1, y + dir);
        if (diagLeft != null && diagLeft.IsOccupied() && diagLeft.GetOccupant().Color != this.Color)
            moves.Add(diagLeft);
        if (diagRight != null && diagRight.IsOccupied() && diagRight.GetOccupant().Color != this.Color)
            moves.Add(diagRight);

        return moves;
    }
}