using System;
using UnityEngine;

public class KnightChessMinigame : MiniGame
{
    public GameObject boardPrefab;
    private ChessBoard board;
    public int[] boardSizes;
    public int[] knightMargins;
    public int[] enemyCounters;

    public ChessPiece playerPiece;


    public override void Reset()
    {
        board = GOBuilder.Create(boardPrefab)
            .WithName("board")
            .WithParent(this.transform)
            .Build().GetComponent<ChessBoard>();

        board.Init(boardSizes[MGM.miniGamesDifficulty - 1]);
        Tile[,] tiles = board.GetTiles();
        foreach (Tile t in tiles)
        {
            PC.AddTapTracker(t);
        }

        float boardDim = Math.Min(PG.bounds.size.x, PG.bounds.size.y) - 0.45f;
        board.transform.localScale = new Vector3(boardDim / board.boardSize, boardDim / board.boardSize, boardDim / board.boardSize);
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
/*
    void difficultyParameters(int difficulty)
    {
        if (difficulty == 1)
        {
            knightSpawnMargin = 1;
            boardSize = 6;
            enemyCounter = 1;
        }
        if (difficulty == 2)
        {
            knightSpawnMargin = 1;
            boardSize = 7;
            enemyCounter = 1;
        }
        if (difficulty == 3)
        {
            knightSpawnMargin = 0;
            boardSize = 8;
            enemyCounter = 1;
        }
        if (difficulty == 4)
        {
            knightSpawnMargin = 2;
            boardSize = 8;
            enemyCounter = 2;
        }
        if (difficulty == 5)
        {
            knightSpawnMargin = 0;
            boardSize = 8;
            enemyCounter = 2;
        }
    }
*/
    void PlaceInitialPieces()
    {
        int n_enemies = enemyCounters[MGM.miniGamesDifficulty - 1];
        int boardSize = boardSizes[MGM.miniGamesDifficulty - 1];
        int knightMargin = knightMargins[MGM.miniGamesDifficulty - 1];
        int[] positions = new int[2 + n_enemies * 2];

        positions[0] = UnityEngine.Random.Range(0, boardSize);
        positions[1] = UnityEngine.Random.Range(0, boardSize);
        board.SpawnPiece(positions[0], positions[1], PlayerColor.White, PieceType.Knight);
        for (int i = 1; i <= n_enemies; i++)
        {
            do
            {
                positions[2 * i] = UnityEngine.Random.Range(knightMargin, boardSize - knightMargin);
                positions[2 * i + 1] = UnityEngine.Random.Range(knightMargin, boardSize - knightMargin);
            } while (board.containsPositon(positions, positions[2 * i], positions[2 * i + 1], i));
            board.SpawnPiece(positions[2 * i], positions[2 * i + 1], PlayerColor.Black, PieceType.Knight);
        }

    }
}
