using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
public enum PlayerColor { White, Black }
public class ChessBoard : MonoBehaviour
{


    [Header("Prefabs & Sprites")]
    public GameObject tilePrefab;
    public GameObject knightPrefab;


    [Header("Sprites")]
    public Sprite whiteKnightSprite;
    public Sprite blackKnightSprite;


    [Header("Board Settings")]
    public int boardSize = 8;
    public float tileSize = 1f;


    private Tile[,] tiles;

    private Knight playerKnight;
    private List<Knight> knights = new List<Knight>();
    private List<Tile> legalMoves;

    private int enemyCounter = 0;


    public void Init()
    {
        GenerateBoard();
        PlaceInitialKnights();
        SelectChessPiece(playerKnight);
    }


    public void Restart()
    {
        foreach (var k in knights) if (k != null) Destroy(k.gameObject);
        knights.Clear();

        if (playerKnight != null)
            Destroy(playerKnight.gameObject);

        if (tiles != null)
        {
            foreach (var t in tiles) if (t != null) Destroy(t.gameObject);
        }
        GenerateBoard();
        PlaceInitialKnights();
        SelectChessPiece(playerKnight);
    }

    void GenerateBoard()
    {
        tiles = new Tile[boardSize, boardSize];
        Vector2 origin = new Vector2(-boardSize / 2f * tileSize + tileSize / 2f, -boardSize / 2f * tileSize + tileSize / 2f);
        Tile tile;

        for (int x = 0; x < boardSize; x++)
        {

            for (int y = 0; y < boardSize; y++)
            {
                Vector2 pos = origin + new Vector2(x * tileSize, y * tileSize);
                tile = GOBuilder.Create(tilePrefab)
                    .WithName("Tile_{" + x + "}_{+" + y + "}")
                    .WithParent(transform)
                    .WithPosition(pos).Build().GetComponent<Tile>();
                tile.Init(x, y, this);
                tiles[x, y] = tile;
            }
        }
    }


    void PlaceInitialKnights()
    {
        enemyCounter = 1;
        int px, py, x, y = -1;
        px = Random.Range(0, boardSize);
        py = Random.Range(0, boardSize);
        do {
            x = Random.Range(0, boardSize);
            y = Random.Range(0, boardSize);      
        }
        while(px != x && py != y )
;
        SpawnKnight(px, py, PlayerColor.White);
        SpawnKnight(x, y, PlayerColor.Black);
    }


    Knight SpawnKnight(int x, int y, PlayerColor color)
    {
        Vector3 pos = tiles[x, y].transform.position;

        Knight knight = GOBuilder.Create(knightPrefab)
            .WithName("Knight" + color.ToString())
            .WithParent(transform)
            .WithPosition(pos)
            .Build().GetComponent<Knight>();

        if (color == PlayerColor.White)
        {
            playerKnight = knight;
        }
        knight.Init(x, y, color, this);
        knight.SetSprite(color == PlayerColor.White ? whiteKnightSprite : blackKnightSprite);
        knights.Add(knight);
        tiles[x, y].SetOccupant(knight);
        return knight;
    }


    public Tile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= boardSize || y >= boardSize) return null;
        return tiles[x, y];
    }


    public void MoveKnight(Tile to)
    {
        Tile from = GetTile(playerKnight.x, playerKnight.y);
        if (to == null) return;

        if (to.IsOccupied())
        {
            Knight other = to.GetOccupant();
            if (other != null && other.Color != playerKnight.Color)
            {
                knights.Remove(other);
                Destroy(other.gameObject);
                enemyCounter--;
            }
            else
            {
                return;
            }
        }


        from.ClearOccupant();
        playerKnight.SetPosition(to.x, to.y);
        to.SetOccupant(playerKnight);

        CheckWinCondition();
        SelectChessPiece(playerKnight);
    }

    void SelectChessPiece(ChessPiece piece)
    {
        legalMoves = piece.GetLegalMoves();
        foreach (Tile legalTile in legalMoves)
        {
            legalTile.Highlight(true);
        }
    }

    void CheckWinCondition()
    {
        if (enemyCounter <= 0)
        {
            GetComponentInParent<MiniGame>().Win();
        }
    }

    public void TileTouched(Tile t)
    {
        if (legalMoves.Contains(t))
        {
            foreach (Tile legalTile in legalMoves)
            {
                legalTile.Highlight(false);
            }
            legalMoves.Clear();
            MoveKnight(t);

        }
    }
    
    public Tile[,] GetTiles(){ return tiles; }
}