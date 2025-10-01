using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using Cysharp.Threading.Tasks;
public enum PlayerColor { White, Black }
public class ChessBoard : MonoBehaviour
{


    [Header("Prefabs & Sprites")]
    public GameObject tilePrefab;
    public GameObject knightPrefab;
    public GameObject queenPrefab;
    public GameObject blackKnightPrefab;
    

    [Header("Board Settings")]
    public int boardSize = 8;
    public float tileSize = 1f;


    private Tile[,] tiles;

    private Knight playerPiece;
    private List<Knight> knights = new List<Knight>();
    private List<Tile> legalMoves;
    private int knightSpawnMargin;
    private int enemyCounter = 0;

    private int diff;
    public void Init(int difficulty)
    {
        difficultyParameters(difficulty);
        GenerateBoard();
        PlaceInitialPieces();
        SelectChessPiece(playerPiece);
    }

    public void Restart()
    {
        foreach (var k in knights) if (k != null) Destroy(k.gameObject);
        knights.Clear();

        if (playerPiece != null)
            Destroy(playerPiece.gameObject);

        if (tiles != null)
        {
            foreach (var t in tiles) if (t != null) Destroy(t.gameObject);
        }
        GenerateBoard();
        PlaceInitialPieces();
        SelectChessPiece(playerPiece);
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

    void PlaceInitialPieces(){
        int[] positions = new int[2+enemyCounter * 2];

        positions[0] = Random.Range(0, boardSize);
        positions[1] = Random.Range(0, boardSize);
        SpawnKnight(positions[0], positions[1], PlayerColor.White);
        for (int i = 1; i < 1 + enemyCounter; i++)
        {
            do
            {
                positions[2 * i] = Random.Range(knightSpawnMargin, boardSize - knightSpawnMargin);
                positions[2 * i + 1] = Random.Range(knightSpawnMargin, boardSize - knightSpawnMargin);
            } while (containsPositon(positions, positions[2 * i], positions[2 * i + 1], i));
            SpawnKnight(positions[2 * i], positions[2 * i + 1], PlayerColor.Black);
        }

    }
    
    private bool containsPositon(int[] pos, int x, int y, int numberOfPlacedPieces) {
        for (int i = 0; i < numberOfPlacedPieces; i++) {
            if (x == pos[i] && y == pos[i + 1]) {
                return true;
            }
        }
    return false;
    }

    Knight SpawnKnight(int x, int y, PlayerColor color)
    {
        Vector3 pos = tiles[x, y].transform.position;

        Knight knight = GOBuilder.Create(color == PlayerColor.White ? knightPrefab : blackKnightPrefab)
            .WithName("Knight" + color.ToString())
            .WithParent(transform)
            .WithPosition(pos)
            .Build().GetComponent<Knight>();

        if (color == PlayerColor.White)
        {
            playerPiece = knight;
        }
        knight.Init(x, y, color, this);
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
        Tile from = GetTile(playerPiece.x, playerPiece.y);
        if (to == null) return;

        if (to.IsOccupied())
        {
            Knight other = to.GetOccupant();
            if (other != null && other.Color != playerPiece.Color)
            {
                knights.Remove(other);
                other.GetComponent<ChessPiece>().Die();
                playerPiece.SpecialPose().Forget();
                enemyCounter--;
            }
            else
            {
                return;
            }
        }


        from.ClearOccupant();
        playerPiece.SetPosition(to.x, to.y);
        to.SetOccupant(playerPiece);

        CheckWinCondition();
        SelectChessPiece(playerPiece);
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
    
    public Tile[,] GetTiles() { return tiles; }
}
