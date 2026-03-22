using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class KnightChessMinigame : MiniGame
{
    public GameObject boardPrefab;
    private ChessBoard board;
    public int[] boardSizes;
    public int[] knightMargins;
    public int[] enemyCounters;

    public int[] maxDistWithPlayer;


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
        if (GameIsLost)
            return;
        MGM.WinMiniGame();
    }

    public override void Lose()
    {
        base.Lose();
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
        int nEnemies = enemyCounters[MGM.miniGamesDifficulty - 1];
        int maxDist = maxDistWithPlayer[MGM.miniGamesDifficulty - 1];

        Knight player = (Knight) board.SpawnPieceAtRandom(
            0,
            PlayerColor.White,
            PieceType.Knight
        );

        Tile startTile = board.GetTile(player.x, player.y);

        HashSet<Tile> visited = new HashSet<Tile>();
        HashSet<Tile> reachableTiles = new HashSet<Tile>();

        Queue<(Tile tile, int depth)> queue = new Queue<(Tile, int)>();

        queue.Enqueue((startTile, 0));
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            var (current, depth) = queue.Dequeue();

            if (depth == maxDist)
            {
                reachableTiles.Add(current);
                continue;
            }

            if (depth > maxDist)
                continue;

            foreach (Tile next in Knight.GetLegalMovesFromPos(current.x, current.y, board))
            {
                if (visited.Add(next))
                {
                    queue.Enqueue((next, depth + 1));
                }
            }
        }

        reachableTiles.Remove(startTile);

        List<Tile> spawnList = reachableTiles.ToList();

        for (int i = 0; i < nEnemies && spawnList.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, spawnList.Count);
            Tile spawnTile = spawnList[index];
            spawnList.RemoveAt(index);

            board.SpawnPiece(
                spawnTile,
                PlayerColor.Black,
                PieceType.Knight
            );
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
