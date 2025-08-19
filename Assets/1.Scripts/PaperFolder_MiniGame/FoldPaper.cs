using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class FoldPaper : MonoBehaviour, ISwipeTracker
{
    [Header("FoldPaper_Internals")]
    MeshFilter mf;
    public bool enabled { get; set; }
    Rect paper;
    public int paper_w, paper_h;
    public float coef_w, coef_h;
    public Mesh paperMesh;
    public List<FoldPaperCell> paperCells;
    public float foldSpeed = 5f;

    public void OnHorizontalSwipe(float iXVal)
    {
        if (paper_w <= 1)
            return;

        int half_w = paper_w / 2;
        if (iXVal > 0f)
        {
            Debug.Log("Swipe Right");
            foreach (FoldPaperCell cell in paperCells.Where(e => e.position.x < half_w))
            {
                if (cell == null)
                    continue;
                //Destroy(cell.gameObject);
                //cell.RotateCellAround(GetCenter(), Vector3.up, 180f);
                cell.RotateCellAround(Vector3.up, -180f);
                cell.transform.localPosition = cell.transform.position + new Vector3(0f, 0f, 0.1f);
            }
        }
        else
        {
            Debug.Log("Swipe Left");
            foreach (FoldPaperCell cell in paperCells.Where(e => e.position.x >= half_w))
            {
                if (cell == null)
                    continue;
                //cell.RotateCellAround(GetCenter(), Vector3.up, -180f);

                cell.RotateCellAround(Vector3.up, -180f);
                cell.transform.localPosition = cell.transform.position + new Vector3(0f, 0f, 0.1f);
            }
        }

        paper_w >>= 1;
        //CreatePaperMesh();
        Recenter();
    }
    public void OnVerticalSwipe(float iYVal)
    {
        if (paper_h <= 1)
            return;

        int half_h = paper_h / 2;
        if (iYVal > 0f)
        {
            Debug.Log("Swipe Up");
            foreach (FoldPaperCell cell in paperCells.Where(e => e.position.y < half_h))
            {
                if (cell == null)
                    continue;
                //cell.RotateCellAround(GetCenter(), Vector3.right, 180f);
                cell.RotateCellAround(Vector3.right, 180f);
                cell.transform.localPosition = cell.transform.position + new Vector3(0f, 0f, 0.1f);
            }
        }
        else
        {
            Debug.Log("Swipe Down");
            foreach (FoldPaperCell cell in paperCells.Where(e => e.position.y >= half_h))
            {
                if (cell == null)
                    continue;
                //cell.RotateCellAround(GetCenter(), Vector3.right, -180f);
                cell.RotateCellAround(Vector3.right, -180f);
                cell.transform.localPosition = cell.transform.position + new Vector3(0f, 0f, 0.1f);
            }
        }
        
        paper_h >>= 1;
        //CreatePaperMesh();
        Recenter();
    }

    public void Init(PaperFolderMiniGame iMG)
    {
        paperCells = new List<FoldPaperCell>();
        
        paper_w = 1;
        paper_h = 1;
        paper_w <<= iMG.MGM.miniGamesDifficulty;
        paper_h <<= iMG.MGM.miniGamesDifficulty;
        Debug.Log(paper_w);
        Debug.Log(paper_h);

        float step_w = 1f / (float)paper_w;
        float step_h = 1f / (float)paper_h;
        for (int i = 0; i < paper_w; i++)
        {
            for (int j = 0; j < paper_h; j++)
            {
                GameObject newCell = GOBuilder.Create()
                                    .WithName("PaperCell_" + i + ":" + j)
                                    .WithParent(transform)
                                    .WithLocalPosition(new Vector3(i, j))
                                    .WithMeshFilter()
                                    .WithRenderer(iMG.paperMat)
                                    .Build();
                FoldPaperCell newPaperCell = newCell.AddComponent<FoldPaperCell>();
                newPaperCell.Init(new Vector2(i,j));
                newPaperCell.CreatePaperCellMesh(
                    new Vector2(1, 1),  // size
                    new Vector2(i*step_w, j*step_h),      // uvmin
                    new Vector2((i+1)*step_w, (j+1)*step_h)       // uvmax
                    );
                // Adjust cell pivot
                // by rotating accordingly
                Debug.DrawRay(newPaperCell.transform.position, Vector3.forward * 5f, Color.red, 20f);

                paperCells.Add(newPaperCell);
            }
        }

        coef_w = iMG.max_size_w / paper_w;
        coef_h = iMG.max_size_h / paper_h;
        transform.localScale = new Vector3(coef_w, coef_h, 1);

        Recenter();
    }

    void Recenter()
    {
        transform.localPosition = GetCenter();
    }

    Vector3 GetCenter()
    {
        return new Vector3( (-paper_w / 2f)*coef_w, (-paper_h / 2f)*coef_h);
    }

    Vector3 GetCenterX()
    {
        return new Vector3(GetCenter().x, 0f, 0f);
    }

    Vector3 GetCenterY()
    {
        return new Vector3(0f, GetCenter().y, 0f);
    }

    public void CreatePaperMesh()
    {
        paperMesh = new Mesh();

        // List<Vector3> vertices = new List<Vector3>();
        // List<int> triangles = new List<int>();
        Color c1 = Color.red;
        Color c2 = Color.green;
        Color c3 = Color.blue;
        Color c = Color.white;
        VertexHelper vh = new VertexHelper();

        int half_w = paper_w / 2;
        int half_h = paper_h / 2;
        // Add unit Square
        vh.AddVert(new Vector3(0, 0), c1, new Vector2(0f, 0f));
        vh.AddVert(new Vector3(0, paper_h), c1, new Vector2(0f, 1f));
        vh.AddVert(new Vector3(paper_w, paper_h), c1, new Vector2(1f, 1f));
        vh.AddVert(new Vector3(paper_w, 0), c1, new Vector2(1f, 0f));

        // Add vertical cuts
        vh.AddVert(new Vector3(half_w, 0), c2, new Vector2(0.5f, 0f));
        vh.AddVert(new Vector3(half_w, paper_h), c2, new Vector2(0.5f, 1f));

        // Add horizontal cuts
        vh.AddVert(new Vector3(0, half_h), c3, new Vector2(0f, 0.5f));
        vh.AddVert(new Vector3(paper_w, half_h), c3, new Vector2(1f, 0.5f));

        // Add Pivot vertice
        vh.AddVert(new Vector3(half_w, half_h), c, new Vector2(0.5f, 0.5f));

        // build triangles
        vh.AddTriangle(8, 4, 0);
        vh.AddTriangle(0, 6, 8);

        vh.AddTriangle(5, 8, 6);
        vh.AddTriangle(6, 1, 5);

        vh.AddTriangle(7, 3, 4);
        vh.AddTriangle(4, 8, 7);

        vh.AddTriangle(2, 7, 8);
        vh.AddTriangle(2, 8, 5);

        // Build mesh
        vh.FillMesh(paperMesh);
        mf.mesh = paperMesh;
    }

    void FoldLeft()
    {

    }
}
