using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public enum PlayerColor { White, Black }
public enum PieceType { Pawn, Knight, Bishop, Tower, King, Queen }

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
    public Vector2Int boardSize = new Vector2Int(8, 8); // <-- maintenant rectangulaire !
    public float tileSize = 1f;

    private Tile[,] tiles;

    public List<ChessPiece> enemies = new List<ChessPiece>();
    private List<ChessPiece> playerPieces = new List<ChessPiece>();
    private ChessPiece selectedPiece;

    private List<Tile> legalMoves = new List<Tile>();

    public bool blackPlays = false;

    
    public void GenerateBoard(Vector2Int size, bool[,] removeTiles)
    {
        boardSize = size;
        tiles = new Tile[boardSize.x, boardSize.y];

        Vector2 origin = new Vector2(
                -boardSize.x / 2f * tileSize + tileSize / 2f,
                -boardSize.y / 2f * tileSize + tileSize / 2f
        );

        Tile tile;

        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                if (removeTiles != null && removeTiles[x, y])
                    continue;

                Vector2 pos = origin + new Vector2(x * tileSize, y * tileSize);
                tile = GOBuilder.Create(tilePrefab)
                    .WithName($"Tile_{x}_{y}")
                    .WithParent(transform)
                    .WithLocalPosition(pos)
                    .Build()
                    .GetComponent<Tile>();

                tile.Init(x, y, this);
                tiles[x, y] = tile;
            }
        }
    }

    public bool canSpawn(int x, int y)
    {
        if (tiles[x, y] == null)
            return false;
        return !tiles[x, y].IsOccupied();
    }

    public ChessPiece SpawnPiece(Tile tile, PlayerColor color, PieceType type)
    {
        GameObject prefab = type switch
        {
            PieceType.Bishop => bishopPrefab,
            PieceType.Knight => knightPrefab,
            PieceType.Pawn => pawnPrefab,
            PieceType.Tower => towerPrefab,
            PieceType.Queen => queenPrefab,
            _ => kingPrefab,
        };

        ChessPiece cp = GOBuilder.Create(prefab)
            .WithName($"ChessPiece_{color}")
            .WithParent(transform)
            .WithPosition(tile.transform.position)
            .Build()
            .GetComponent<ChessPiece>();

        cp.Init(tile.x, tile.y, color, this);
        tile.SetOccupant(cp);

        if (color == PlayerColor.White)
        {
            playerPieces.Add(cp);
            SelectPiece(cp); // dernier ajouté sélectionné
        }
        else
        {
            enemies.Add(cp);
        }

        return cp;
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= boardSize.x || y >= boardSize.y)
            return null;

        return tiles[x, y];
    }

    public void MovePlayer(Tile to)
    {
        Tile from = GetTile(selectedPiece.x, selectedPiece.y);
        if (to == null) return;

        if (to.IsOccupied())
        {
            ChessPiece other = to.GetOccupant();
            if (other != null && other.color != selectedPiece.color)
            {
                enemies.Remove(other);
                other.GetComponent<ChessPiece>().Die();
                selectedPiece.SpecialPose().Forget();
            }
            else
                return;
        }

        MovePiece(from, to, selectedPiece);

        CheckWinCondition();

        if (blackPlays)
            MoveBlacks();

        FindLegalMoves();
    }

    void SelectPiece(ChessPiece piece)
    {
        selectedPiece = piece;
        ClearLegalMoves();
        FindLegalMoves();
    }

    void MoveBlacks()
    {
        List<Tile> positions;
        foreach (ChessPiece bcp in enemies)
        {
            positions = bcp.GetLegalMoves();
            Utils.Shuffle(positions);

            foreach(Tile pos in positions)
            {
                if (!pos.IsOccupied())
                {
                    bcp.GetPos(out int x, out int y);
                    MovePiece(GetTile(x, y), pos, bcp);
                    break;
                }
            }
        }
    }

    void MovePiece(Tile from, Tile to, ChessPiece piece)
    {
        from.ClearOccupant();

        piece.SetPosition(to.x, to.y);
        to.SetOccupant(piece);

                
        if (piece is Pawn pawn)
        {
            if (to.y == boardSize.y - 1 || to.y == 0) // Promotion selon la hauteur du plateau
            {
                piece.playParticles();
                piece = pawn.Promote();
            }
        }
    }

    void CheckWinCondition()
    {
        if (enemies.Count == 0)
            GetComponentInParent<MiniGame>().Win();
    }

    public void TileTouched(Tile t)
    {
        if (legalMoves.Contains(t))
        {
            ClearLegalMoves();
            MovePlayer(t);
        }
        else if (t.IsOccupied() && t.GetOccupant().color == PlayerColor.White)
        {
            SelectPiece(t.GetOccupant());
        }
    }

    public void ClearLegalMoves()
    {
        foreach (Tile legalTile in legalMoves)
            legalTile.Highlight(false);

        legalMoves.Clear();
    }

    void FindLegalMoves()
    {
        legalMoves = selectedPiece.GetLegalMoves();
        foreach (Tile legalTile in legalMoves)
            legalTile.Highlight(true);
    }

    public Tile[,] GetTiles() => tiles;

    public void CleanAll(PlayerController iPC)
    {
        foreach (Tile t in tiles)
        {
            if (t == null) continue;
            iPC.RemoveTapTracker(t);
            Destroy(t.gameObject);
        }

        foreach (var k in enemies) if (k != null) Destroy(k.gameObject);
        enemies.Clear();

        foreach (var k in playerPieces) if (k != null) Destroy(k.gameObject);
        playerPieces.Clear();
    }

    public Tile getRandomUnoccupiedTile(Vector2Int margin)
    {
        int x, y;
        do
        {
            x = Random.Range(margin.x, boardSize.x - margin.x);
            y = Random.Range(margin.y, boardSize.y - margin.y);
        } while (tiles[x, y] == null || tiles[x, y].IsOccupied());

        return tiles[x, y];
    }
}
