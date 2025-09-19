using System;
using UnityEngine;

public class ChessMinigame : MiniGame
{
    public GameObject boardPrefab;
    private ChessBoard board;

    public void ResetGame()
    {
        board.Restart();
    }

    public override void Init()
    {

        board = GOBuilder.Create(boardPrefab)
            .WithName("board")
            .WithParent(this.transform)
            .Build().GetComponent<ChessBoard>();
        board.Init();
        Tile[,] tiles = board.GetTiles();
        foreach (Tile t in tiles)
        {
            PC.AddTapTracker(t);
        }

        float boardDim = Math.Min(PG.bounds.size.x, PG.bounds.size.y) - 0.45f;
        board.transform.localScale = new Vector3(boardDim / board.boardSize, boardDim / board.boardSize, boardDim / board.boardSize);
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
        Tile[,] tiles = board.GetTiles();
        foreach (Tile t in tiles)
        {
            PC.RemoveTapTracker(t);
        }
        Destroy(board.gameObject);
    }
}
