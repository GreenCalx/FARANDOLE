using System;
using UnityEngine;

public class KnightChessMinigame : MiniGame
{
    public GameObject boardPrefab;
    private ChessBoard board;
    public int[] boardSizes;
    public int[] knightMargins;
    public int[] enemyCounters;


    public override void Reset()
    {
        board = GOBuilder.Create(boardPrefab)
            .WithName("board")
            .WithParent(this.transform)
            .WithPosition(new Vector3(PG.bounds.center.x,PG.bounds.center.y, 0))
            .Build().GetComponent<ChessBoard>();

        board.GenerateBoard(new Vector2Int(boardSizes[MGM.miniGamesDifficulty - 1],boardSizes[MGM.miniGamesDifficulty - 1]), ShapeBoard());
        Tile[,] tiles = board.GetTiles();
        foreach (Tile t in tiles)
        {
            if(t != null)
                PC.AddTapTracker(t);
        }

        float boardDim = Math.Min(PG.bounds.size.x, PG.bounds.size.y) - 0.45f;
        board.transform.localScale = new Vector3(boardDim / board.boardSize.x, boardDim / board.boardSize.y, boardDim / board.boardSize.x);

        PlaceInitialPieces();
    }

    public override void Init()
    {
        // Reset();
    }

    public override void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public override void Stop()
    {
        Clean();
        IsActiveMiniGame = false;
        IsInPostGame = false;
    }
    public override void Win()
    {
        MGM.WinMiniGame();
    }

    public override void Lose()
    {

    }
    public override bool SuccessCheck()
    {
        return false;
    }
    private void Clean()
    {
        board.CleanAll(PC);
        Destroy(board.gameObject);
    }

    void PlaceInitialPieces()
    {
        int n_enemies = enemyCounters[MGM.miniGamesDifficulty - 1];
        int knightMargin = knightMargins[MGM.miniGamesDifficulty - 1];

        board.SpawnPiece(board.getRandomUnoccupiedTile(Vector2Int.zero), PlayerColor.White, PieceType.Knight);
        for (int i = 1; i <= n_enemies; i++)
        {
            board.SpawnPiece(board.getRandomUnoccupiedTile(new Vector2Int(knightMargin,knightMargin)), PlayerColor.Black, PieceType.Knight);
        }

    }

    bool[,] ShapeBoard()
    {
        int boardSize = boardSizes[MGM.miniGamesDifficulty - 1];
        int half = boardSize / 2;
        bool[,] removeTile = new bool[boardSize, boardSize];
        if (MGM.miniGamesDifficulty > 2)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if ((x < half && y >= half) || (x >= half && y < half))
                    {
                        removeTile[x, y] = true;
                    }
                    else
                    {
                        removeTile[x, y] = false;
                    }
                }
            }
        }
        return removeTile;
    }
}
