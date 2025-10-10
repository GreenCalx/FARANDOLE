using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using Cysharp.Threading.Tasks;
public enum PlayerColor { White, Black }
public enum PieceType {Pawn,Knight,Bishop,Tower, King, Queen}
public class ChessBoard : MonoBehaviour
{


    [Header("Prefabs & Sprites")]
    public GameObject tilePrefab;

    public GameObject knightPrefab;
    public GameObject towerPrefab;
    public GameObject pawnPrefab;
    public GameObject bishopPrefab;
    public GameObject queenPrefab;
    public GameObject kingPrefab;




    [Header("Board Settings")]
    public int boardSize = 8;
    public float tileSize = 1f;


    private Tile[,] tiles;

    private List<ChessPiece> enemies = new List<ChessPiece>();
    private ChessPiece playerPiece;
    private List<Tile> legalMoves;

    private int diff;

    public bool blackPlays = false;
    public void Init(int size)
    {
        GenerateBoard(size); //
        SelectChessPiece(playerPiece);
    }

    public void Restart()
    {
        if (tiles != null)
        {
            foreach (var t in tiles) if (t != null) Destroy(t.gameObject);
        }
        GenerateBoard(boardSize);
        SelectChessPiece(playerPiece);
    }

    void GenerateBoard(int size)
    {
        boardSize = size;
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



    public bool containsPositon(int[] pos, int x, int y, int numberOfPlacedPieces)
    {
        for (int i = 0; i < numberOfPlacedPieces; i++)
        {
            if (x == pos[i] && y == pos[i + 1])
            {
                return true;
            }
        }
        return false;
    }

    public void SpawnPiece(int x, int y, PlayerColor color, PieceType type)
    {
        Vector3 pos = tiles[x, y].transform.position;

        GameObject prefab;

        switch (type)
        {
            case PieceType.Bishop:
                prefab = bishopPrefab;
                break;
            case PieceType.Knight:
                prefab = knightPrefab;
                break;
            case PieceType.Pawn:
                prefab = pawnPrefab;
                break;
            case PieceType.Tower:
                prefab = towerPrefab;
                break;
            case PieceType.Queen:
                prefab = queenPrefab;
                break;
            default :
                prefab = kingPrefab;
                break;
        }

        ChessPiece cp = GOBuilder.Create(prefab)
            .WithName("ChessPiece" + color.ToString())
            .WithParent(transform)
            .WithPosition(pos)
            .Build().GetComponent<ChessPiece>();

        if (color == PlayerColor.White)
        {
            playerPiece = cp;
        }
        else
        {
            enemies.Add(cp);
        }
        cp.Init(x, y, color, this);
        tiles[x, y].SetOccupant(cp);
    }


    public Tile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= boardSize || y >= boardSize) return null;
        return tiles[x, y];
    }


    public void MovePlayer(Tile to)
    {
        Tile from = GetTile(playerPiece.x, playerPiece.y);
        if (to == null) return;

        if (to.IsOccupied())
        {
            ChessPiece other = to.GetOccupant();
            if (other != null && other.Color != playerPiece.Color)
            {
                enemies.Remove(other);
                other.GetComponent<ChessPiece>().Die();
                playerPiece.SpecialPose().Forget();
            }
            else
            {
                return;
            }
        }


        MovePiece(from, to, playerPiece);

        CheckWinCondition();
        if (blackPlays)
        {
            MoveBlacks();
        }
        SelectChessPiece(playerPiece);


    }

    void MoveBlacks()
    {
        List<Tile> positions;
        foreach (ChessPiece bcp in enemies)
        {
            positions = bcp.GetLegalMoves();
            Utils.Shuffle<Tile>(positions);
            for (int i = 0; i < positions.Count; i++)
            {
                if (!positions[i].IsOccupied())
                {
                    int x, y;
                    bcp.GetPos(out x, out y);
                    MovePiece(GetTile(x, y), positions[i], bcp);
                }
            }
        }
    }

    void MovePiece(Tile from, Tile to, ChessPiece piece)
    {
        from.ClearOccupant();
        if (piece is Pawn pawn)
        {
            if (piece.y == boardSize - 1 || piece.y == 0) //Promote
            {
                enemies.Remove(piece);
                SpawnPiece(to.x, to.y, piece.Color, PieceType.Queen);
            }
        }
        else
        {
            piece.SetPosition(to.x, to.y);
            to.SetOccupant(piece);
        }       
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
        if (enemies.Count == 0)
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
            MovePlayer(t);

        }
    }

    public Tile[,] GetTiles() { return tiles; }

    public void CleanAll(PlayerController iPC)
    {
        foreach (Tile t in tiles)
        {
            iPC.RemoveTapTracker(t);
            Destroy(t.gameObject);
        }
        foreach (var k in enemies) if (k != null) Destroy(k.gameObject);
        enemies.Clear();

        if (playerPiece != null)
            Destroy(playerPiece.gameObject);
    }
}
