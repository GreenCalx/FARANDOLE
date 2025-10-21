using System.Collections.Generic;
//using Codice.CM.Client.Differences.Graphic;
using UnityEngine;



public class Pawn : ChessPiece
{
    public Queen queenComponent;
    private bool enPassant;
    override public List<Tile> GetLegalMoves()
    {
        List<Tile> moves = new List<Tile>();
        int dir = (color == PlayerColor.White) ? 1 : -1;

        Tile forward = board.GetTile(x, y + dir);
        if ((forward != null))
        {
            if (forward.IsOccupied() && color == PlayerColor.White)
                enPassant = true;
            if (!forward.IsOccupied())
                moves.Add(forward);
        }


        Tile diagLeft = board.GetTile(x - 1, y + dir);
        Tile diagRight = board.GetTile(x + 1, y + dir);
        if (diagLeft != null && (enPassant || diagLeft.IsOccupied() && diagLeft.GetOccupant().color != this.color))
            moves.Add(diagLeft);
        if (diagRight != null && (enPassant || diagRight.IsOccupied() && diagRight.GetOccupant().color != this.color))
            moves.Add(diagRight);

        return moves;
    }
    
    public Queen Promote()
    {
        queenComponent.enabled = true;
        queenComponent.Init(x, y, color, board);
        this.enabled = false;
        Tile tile = board.GetTile(x, y);
        tile.SetOccupant(queenComponent);
        return queenComponent;
    }
}