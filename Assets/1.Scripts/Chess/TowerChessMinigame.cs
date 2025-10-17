using System;

using UnityEngine;

public class TowerChessMinigame : MiniGame
{
    public GameObject boardPrefab;
    private ChessBoard board;
    public Vector2Int[] boardSizes;
    public int[] enemyCounters;


    public override void Reset()
    {
        board = GOBuilder.Create(boardPrefab)
            .WithName("board")
            .WithParent(this.transform)
            .WithPosition(new Vector3(PG.bounds.center.x,PG.bounds.center.y, 0))
            .Build().GetComponent<ChessBoard>();

        board.GenerateBoard(boardSizes[MGM.miniGamesDifficulty - 1], ShapeBoard());
        Tile[,] tiles = board.GetTiles();
        foreach (Tile t in tiles)
        {
            if(t != null)
                PC.AddTapTracker(t);
        }

        Vector2 boardDim = new Vector2(PG.bounds.size.x-0.35f, PG.bounds.size.y - 0.35f);
        float minScale = Mathf.Min(boardDim.x / board.boardSize.x, boardDim.y / board.boardSize.y);
        board.transform.localScale = new Vector3(minScale,minScale,minScale);

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
        board.SpawnPiece(board.GetTile(UnityEngine.Random.Range(0, boardSizes[MGM.miniGamesDifficulty - 1].x), 0), PlayerColor.White, PieceType.Tower);
        Vector2Int pos = new Vector2Int();
        pos.y = boardSizes[MGM.miniGamesDifficulty - 1].y - 1; 
        for (int i = 1; i <= n_enemies; i++)
        {
            do
            {
                pos.x = UnityEngine.Random.Range(0, boardSizes[MGM.miniGamesDifficulty - 1].x);
            }
            while (!board.canSpawn(pos.x, pos.y));
            board.SpawnPiece(board.GetTile(pos.x, pos.y), PlayerColor.Black, PieceType.Pawn);
        }
        board.blackPlays = true;

    }

    bool[,] ShapeBoard()
    {
        Vector2Int boardSize = boardSizes[MGM.miniGamesDifficulty - 1];
        bool[,] removeTile = new bool[boardSize.x, boardSize.y];
        return removeTile;
    }
}
