using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Cysharp.Threading.Tasks;
using static Utils;
using static MeshUtils;
using static Constants;

[Serializable]
public class RoomPlan
{
    public Rect area;
    public float zDepth;
    public Vector2 min
    {
        get { return area.min; }
    }

    public Vector2 max
    {
        get { return area.max; }
    }

    public float width
    {
        get { return area.width; }
    }

    public float height
    {
        get { return area.height; }
    }
    public Vector3 BottomLeftPoint => new Vector3(min.x, max.y, zDepth);
    public Vector3 BottomRightPoint => new Vector3(max.x, max.y, zDepth);
    public Vector3 UpperLeftPoint => new Vector3(min.x, min.y, zDepth);
    public Vector3 UpperRightPoint => new Vector3(max.x, min.y, zDepth);
}
public struct RoomObject
{
    public Transform transform;
    public Renderer renderer;
    public RoomObject(Transform iTransform, Renderer iRenderer)
    {
        transform = iTransform;
        renderer = iRenderer;
    }
}

public struct RoomCubemap
{
    Mesh[] meshes;
    public void Init()
    {
        meshes = new Mesh[6];
        for (int i = 0; i < 6; i++)
        {
            meshes[i] = new Mesh();
        }
    }
    public readonly Mesh ground => meshes[0];
    public Mesh BuildGround(PerspectiveRoom iRoom)
    {
        meshes[0] = MeshUtils.WallQuad_FullRect(
            iRoom.m_NearPlan.UpperLeftPoint,
            iRoom.m_NearPlan.UpperRightPoint,
            iRoom.m_FarPlan.UpperLeftPoint,
            iRoom.m_FarPlan.UpperRightPoint
        );
        return meshes[0];
    }
    public readonly Mesh sky => meshes[1];
    public readonly Mesh far => meshes[2];
    public Mesh BuildFar(PerspectiveRoom iRoom)
    {
            meshes[2] = MeshUtils.WallQuad_FullRect(
            iRoom.m_FarPlan.UpperLeftPoint,
            iRoom.m_FarPlan.UpperRightPoint,
            iRoom.m_FarPlan.BottomLeftPoint,
            iRoom.m_FarPlan.BottomRightPoint
        );
        return meshes[2];
    }
    public readonly Mesh near => meshes[3];
    public readonly Mesh leftWall => meshes[4];
    public Mesh BuildLeftWall(PerspectiveRoom iRoom)
    {
        Debug.Log("BuildLeftWall");
            meshes[4] = MeshUtils.WallQuad_FullRect(
            iRoom.m_FarPlan.UpperLeftPoint,
            iRoom.m_NearPlan.UpperLeftPoint,

            iRoom.m_FarPlan.BottomLeftPoint,
            iRoom.m_NearPlan.BottomLeftPoint

        );
        return meshes[4];
    }
    public readonly Mesh rightWall => meshes[5];
    public Mesh BuildRightWall(PerspectiveRoom iRoom)
    {
        Debug.Log("BuildRightWall");
        meshes[5] = MeshUtils.WallQuad_FullRect(
        iRoom.m_FarPlan.UpperRightPoint,
        iRoom.m_NearPlan.UpperRightPoint,
        iRoom.m_FarPlan.BottomRightPoint,
        iRoom.m_NearPlan.BottomRightPoint
        );
        return meshes[5];
    }
}
[Serializable]
public class RowInfo
{
    public Vector2 min, max;
    public Rect area;
    public int rowDepth;
    public float objectScale;
    public RowInfo(RoomRow iRoomRow)
    {
        min = iRoomRow.min;
        max = iRoomRow.max;
        area = iRoomRow.area;
        rowDepth = iRoomRow.rowDepth;
        objectScale = iRoomRow.objectScale;
    }
}
public class RoomRow
{
    public List<RoomObject> objects;
    public Vector2 min, max;
    public float range => max.x - min.x;
    public Rect area;
    public int rowDepth;
    public float objectScale;
    public LineRenderer lineTracer;
    public RoomRow(Rect iArea, int iDepth)
    {
        min = iArea.min;
        max = iArea.max;
        area = iArea;
        rowDepth = iDepth;
        objectScale = 1f;
        objects = new List<RoomObject>();
    }

    public RowInfo GetInfo()
    {
        return new RowInfo(this);
    }
    public void AddToRow(Transform iTransform, Renderer iRenderer, bool iMutateScaling)
    {
        objects.Add(new RoomObject(iTransform, iRenderer));
        if (iMutateScaling)
        {
            iTransform.localScale = new Vector3(objectScale, objectScale, objectScale);
            iTransform.position = new Vector3(Mathf.Clamp(iTransform.position.x, min.x, max.x), min.y, rowDepth);
        }
    }

    public bool Remove(Transform iTransform)
    {
        List<RoomObject> to_rm = objects.Where(e => e.transform == iTransform).ToList();
        if (to_rm.Count > 0)
        {
            foreach (RoomObject ro in to_rm)
            {
                objects.Remove(ro);
            }
            return true;
        }
        return false;
    }
}

public class RoomRowPlacer : IEnumerator<float>
{
    public float Current { get { return currFrac; } }
    object IEnumerator.Current { get { return Current; } }
    int index = 0;
    float fracStep = 0f;
    int rowDepth = 0;
    int rowSubdivision = 0;
    int currDepth = 0;
    float currFrac = 0f;
    bool m_LRPadding;
    public void Init(int iRows, int iRowSubdivision, bool iLRPadding)
    {
        m_LRPadding = iLRPadding;
        rowDepth        = iRows;
        rowSubdivision = iLRPadding ? iRowSubdivision + 2 : iRowSubdivision;
        if (rowSubdivision <= 1)
            fracStep = 0.5f;
        else
            fracStep = 1f / Math.Max(rowSubdivision - 1, 0);

    }
    public void Reset()
    {
        currDepth   = 0;
        index       = 0;
        currFrac    = 0f;
    }
    public bool MoveNext()
    {
        if (index >= rowSubdivision * rowDepth)
            return false;
        currFrac = (++index % rowSubdivision) * fracStep;
        return true;
    }

    public float At(int iRowIndex)
    {
        if (m_LRPadding)
            return ((iRowIndex+1) % (rowSubdivision)) * fracStep;
        else
            return ((iRowIndex) % rowSubdivision) * fracStep;
    }
    void IDisposable.Dispose()
    {

    }
}
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class PerspectiveRoom : MonoBehaviour
{
    [Header("RemoveMe")]
    public bool cmd_Build = false;
    public bool cmd_Clean = false;
    public bool cmd_ContinuousRefresh = false;

    [Header("Tweaks")]
    public int m_Depth = 3;
    public RoomPlan m_NearPlan;
    public RoomPlan m_FarPlan;
    public float nearObjectScale = 1f;
    public float farObjectScale = 0.2f;
    [Range(0f,1f)]
    public float m_XDeadZone = 0f;
    public bool AutoScaleFromVanishingPoint = false;

    public bool m_TraceLines = true;
    public Material m_LineTracerMat;
    public Material m_RowTracerMat;
    public float m_LineWidth = 0.5f;
    public float m_LineWidthDepthFalloff = 2f;
    public string m_TracesSortingLayer = "PerspectiveRoom";
    [Header("CubeMap")]
    public RoomCubemap cubemap;
    public Material groundMat;
    public Material backMat;
    public Material wallMat_Left;
    public Material wallMat_Right;
    [Header("Internal View")]
    public float m_FOV = 90; // auto
    public LayerManager2D r_LM2D;
    List<RoomRow> m_Rows;
    float m_FarLineWidth = 0;
    public RoomRowPlacer m_RoomRowPlacer;
    LineRenderer farPlaneLineTracer;
    LineRenderer nearPlaneLineTracer;
    List<LineRenderer> perspectiveLines;

    public void Init(LayerManager2D iLM2D)
    {
        r_LM2D = iLM2D;
    }

    public void Init(LayerManager2D iLM2D, int iNumberOfRows)
    {
        r_LM2D = iLM2D;
        m_Depth = iNumberOfRows;
    }

    public void InitRoomPlacer(int iRowSubdivision, bool iLRPadding)
    {
        m_RoomRowPlacer = new RoomRowPlacer();
        m_RoomRowPlacer.Init(m_Depth, iRowSubdivision, iLRPadding);
    }
    public void Build()
    {
        if (!m_TraceLines)
        {
            // its dirty but quick to do the work for now
            // TODO : build line tracers only when required properly
            m_LineWidth = 0f;
        }

        // ----------------------------------------------------
        // Rebase Z position on active camera to have a meaningful depth value
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y,
                                        Camera.main.transform.position.z);

        // ----------------------------------------------------
        // Invariants compute
        m_FarLineWidth = m_LineWidth / m_LineWidthDepthFalloff;

        // ----------------------------------------------------
        // Trace planes
        BuildPlan("nearPlan", m_NearPlan, m_LineWidth, ref nearPlaneLineTracer);
        r_LM2D?.PlaceObject(nearPlaneLineTracer);

        BuildPlan("FarPlan", m_FarPlan, m_FarLineWidth, ref farPlaneLineTracer);
        r_LM2D?.PlaceObject(farPlaneLineTracer);

        // Deduce FOV from plans
        Vector3 relative = m_FarPlan.area.min - m_NearPlan.area.min;
        m_FOV = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;


        // ----------------------------------------------------
        // Trace Perspective Lines
        perspectiveLines = new List<LineRenderer>(4);
        perspectiveLines.Add(
            BuildLine(
                "min -> min",
                new Vector3(m_NearPlan.min.x, m_NearPlan.min.y, m_NearPlan.zDepth),
                new Vector3(m_FarPlan.min.x, m_FarPlan.min.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );
        perspectiveLines.Add(
            BuildLine(
                "max -> max",
                new Vector3(m_NearPlan.max.x, m_NearPlan.max.y, m_NearPlan.zDepth),
                new Vector3(m_FarPlan.max.x, m_FarPlan.max.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );
        perspectiveLines.Add(
            BuildLine(
                "min -> max",
                new Vector3(m_NearPlan.min.x, m_NearPlan.max.y, m_NearPlan.zDepth),
                new Vector3(m_FarPlan.min.x, m_FarPlan.max.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );
        perspectiveLines.Add(
            BuildLine(
                "max -> min",
                new Vector3(m_NearPlan.max.x, m_NearPlan.min.y, m_NearPlan.zDepth),
                new Vector3(m_FarPlan.max.x, m_FarPlan.min.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );
        foreach (LineRenderer lr in perspectiveLines)
        { r_LM2D?.PlaceObject(lr); }

        // ----------------------------------------------------
        // Init rows
        m_Rows = new List<RoomRow>(m_Depth);
        for (int i = 0; i <= m_Depth; i++)
        {
            float frac = (float)i / m_Depth;
            string name = "Row " + i;
            Rect rowArea = new Rect(
                Utils.Lerp(m_NearPlan.min.x, m_FarPlan.min.x, frac),
                Utils.Lerp(m_NearPlan.min.y, m_FarPlan.min.y, frac),
                Utils.Lerp(m_NearPlan.width, m_FarPlan.width, frac),
                Utils.Lerp(m_NearPlan.height, m_FarPlan.height, frac)
            );
            float lineWidth = Utils.Lerp(m_LineWidth, m_FarLineWidth, frac);
            float zDepth = Utils.Lerp(m_NearPlan.zDepth, m_FarPlan.zDepth, frac);
            float objectScale = ComputeRowObjectScale(frac);
            m_Rows.Add(BuildRow(name, rowArea, lineWidth, zDepth, i, objectScale));
        }
        foreach (RoomRow rr in m_Rows)
        { r_LM2D?.PlaceObject(rr.lineTracer); }

        // ----------------------------------------------------
        // Init Room meshes
        cubemap.Init();
        MeshRenderer mr_ground = GOBuilder.Create()
                            .WithName("Ground")
                            .WithParent(transform)
                            .WithPosition(Vector3.zero)
                            .WithMeshFilter(cubemap.BuildGround(this), false)
                            .WithRenderer(groundMat)
                            .BuildAs<MeshRenderer>();
        mr_ground.sortingLayerName = Constants.LYR_PROOM;
        r_LM2D?.PlaceObject(mr_ground);

        MeshRenderer mr_far = GOBuilder.Create()
                            .WithName("Backroom")
                            .WithParent(transform)
                            .WithPosition(Vector3.zero)
                            .WithMeshFilter(cubemap.BuildFar(this), false)
                            .WithRenderer(backMat)
                            .BuildAs<MeshRenderer>();
        mr_far.sortingLayerName = Constants.LYR_PROOM;
        r_LM2D?.PlaceObject(mr_far);

        MeshRenderer mr_wallLeft = GOBuilder.Create()
                            .WithName("LeftWall")
                            .WithParent(transform)
                            .WithPosition(Vector3.zero)
                            .WithMeshFilter(cubemap.BuildLeftWall(this), false)
                            .WithRenderer(wallMat_Left)
                            .BuildAs<MeshRenderer>();
        mr_wallLeft.sortingLayerName = Constants.LYR_PROOM;
        r_LM2D?.PlaceObject(mr_wallLeft);

        MeshRenderer mr_wallRight = GOBuilder.Create()
                            .WithName("RightWall")
                            .WithParent(transform)
                            .WithPosition(Vector3.zero)
                            .WithMeshFilter(cubemap.BuildRightWall(this), false)
                            .WithRenderer(wallMat_Right)
                            .BuildAs<MeshRenderer>();
        mr_wallRight.sortingLayerName = Constants.LYR_PROOM;
        r_LM2D?.PlaceObject(mr_wallRight);

    }

    float ComputeRowObjectScale(float iFrac)
    {
        if (AutoScaleFromVanishingPoint)
        {
            //  TODO
            return 1f;
        }

        return Utils.Lerp(nearObjectScale, farObjectScale, iFrac);
    }

    public RoomRow BuildRow(string iName, Rect iArea, float iLineWidth, float iZDepth, int iRowDepth, float iRowObjectScale)
    {
        RoomRow ret = new RoomRow(iArea, iRowDepth);

        ret.lineTracer = GOBuilder.Create()
                        .WithName(iName)
                        .WithParent(transform)
                        .WithPosition(iArea.center)
                        .WithLineRenderer(m_RowTracerMat)
                        .BuildAs<LineRenderer>();

        Vector3[] pos = new Vector3[4];
        pos[0] = new Vector3(iArea.max.x, iArea.max.y, iZDepth);
        pos[1] = new Vector3(iArea.max.x, iArea.min.y, iZDepth);
        pos[2] = new Vector3(iArea.min.x, iArea.min.y, iZDepth);
        pos[3] = new Vector3(iArea.min.x, iArea.max.y, iZDepth);

        ret.lineTracer.positionCount = 4;
        ret.lineTracer.loop = true;
        ret.lineTracer.startWidth = iLineWidth;
        ret.lineTracer.endWidth = iLineWidth;
        ret.lineTracer.SetPositions(pos);
        ret.lineTracer.sortingOrder = 1;
        ret.lineTracer.sortingLayerName = m_TracesSortingLayer;
        ret.lineTracer.rendererPriority = 8;

        ret.rowDepth = iRowDepth;
        ret.objectScale = iRowObjectScale;

        r_LM2D?.PlaceObject(ret.lineTracer);

        return ret;
    }

    public void BuildPlan(string iPlanName, RoomPlan iPlan, float iLineWidth, ref LineRenderer oLineTracer)
    {
        oLineTracer = GOBuilder.Create()
                            .WithName(iPlanName)
                            .WithParent(transform)
                            .WithPosition(iPlan.area.center)
                            .WithLineRenderer(m_LineTracerMat)
                            .BuildAs<LineRenderer>();

        Vector3[] pos = new Vector3[4];
        pos[0] = new Vector3(iPlan.area.max.x, iPlan.area.max.y, iPlan.zDepth);
        pos[1] = new Vector3(iPlan.area.max.x, iPlan.area.min.y, iPlan.zDepth);
        pos[2] = new Vector3(iPlan.area.min.x, iPlan.area.min.y, iPlan.zDepth);
        pos[3] = new Vector3(iPlan.area.min.x, iPlan.area.max.y, iPlan.zDepth);

        oLineTracer.positionCount = 4;
        oLineTracer.loop = true;
        oLineTracer.startWidth = iLineWidth;
        oLineTracer.endWidth = iLineWidth;
        oLineTracer.SetPositions(pos);

        oLineTracer.sortingOrder = 0;
        oLineTracer.sortingLayerName = m_TracesSortingLayer;
        oLineTracer.rendererPriority = 10;
    }

    public LineRenderer BuildLine(string iName, Vector3 iMin, Vector3 iMax, float iWidthAtMin, float iWidthAtMax)
    {
        LineRenderer line = GOBuilder.Create()
                            .WithName(iName)
                            .WithParent(transform)
                            .WithLineRenderer(m_LineTracerMat)
                            .BuildAs<LineRenderer>();

        Vector3[] pos = new Vector3[2];
        pos[0] = iMin;
        pos[1] = iMax;

        line.positionCount = 2;
        line.loop = false;
        line.startWidth = iWidthAtMin;
        line.endWidth = iWidthAtMax;
        line.SetPositions(pos);
        line.sortingOrder = 0;
        line.sortingLayerName = m_TracesSortingLayer;
        //line.rendererPriority = 9;

        return line;

    }

    // --------------------------------------------
    // Accessors

    public RoomRow GetRowAt(int iDepth)
    {
        int rowDepth = Mathf.Clamp(iDepth, 0, m_Depth);
        RoomRow retval = m_Rows[rowDepth];
        if (retval.rowDepth != rowDepth)
        {
            Debug.LogWarning("PerspectiveRoom::GetRowAt(" + rowDepth + ") doesn't have a reflexive index in m_Rows.");
            foreach (RoomRow rr in m_Rows)
            {
                if (rr.rowDepth == iDepth)
                    return rr;
            }
            Debug.LogWarning("PerspectiveRoom::GetRowAt(" + rowDepth + ") doesn't have a row with input index.");
            return null;
        }
        return retval;
    }

    public void Clean()
    {
#if UNITY_EDITOR

        // Note : Dirty hack to delete every child
        // because for some reason this forloop
        // only iterates over some of them ?
        for (int i=0; i < 5; i++)
        {
            foreach (Transform t in transform)
            {
                DestroyImmediate(t.gameObject);
            }
        }
#else
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
#endif
    }


    // --------------------------------------------
    // Public API

    public Vector3 GetVanishingPoint()
    {
        return m_FarPlan.area.center;
    }

    public void PlaceAllOnLayers()
    {
        if (m_Rows.Count < 0)
            return;

        for (int i = m_Rows.Count - 1; i >= 0; i--)
        {
            if (m_Rows[i] == null)
                continue;
            if (m_Rows[i].objects.Count == 0)
                continue;

            foreach (RoomObject ro in m_Rows[i].objects)
            {
                Debug.Log("Placed " + ro.renderer.gameObject.name);
                r_LM2D.PlaceObject(ro.renderer);
            }
        }
    }

    public RowInfo AddToRoom(Transform iCaller, int iRowDepth, Renderer iRenderer)
    {
        RoomRow targetRow = GetRowAt(iRowDepth);
        if (targetRow == null)
        {
            Debug.Log("PerspectiveRoom::AddToRoom(" + iCaller + gameObject.name + ", " + iRowDepth + ")");
            return null;
        }
        targetRow.AddToRow(iCaller, iRenderer, true);
        return targetRow.GetInfo();
    }

    public bool RemoveFromRoom(Transform iCaller)
    {
        foreach (RoomRow rr in m_Rows)
        {
            if (rr.Remove(iCaller))
                return true;
        }
        return false;
    }

    // Sets the inaccessible percentage of a row
    //from Xmin and from Xmax
    public void SetXDeadzone(float iDeadzone)
    {
        m_XDeadZone = iDeadzone;
    }

    public float XMinForRow(int iRowDepth)
    {
        return m_Rows[iRowDepth].min.x + (m_XDeadZone*m_Rows[iRowDepth].range/2f);
    }

    public float XMaxForRow(int iRowDepth)
    {
        return m_Rows[iRowDepth].max.x - + (m_XDeadZone*m_Rows[iRowDepth].range/2f);
    }

    public float GetXRowLerp(int iRowDepth, float iLerpValue)
    {
        float lerp = Mathf.Clamp(iLerpValue, 0f, 1f);
        Debug.Log( "row depth : " + iRowDepth +  " /lerp : " + lerp);
        return Utils.Lerp(XMinForRow(iRowDepth), XMaxForRow(iRowDepth), lerp);
    }

    public void RandomPositionInRow(Transform iTarget, int iRowDepth)
    {
        // iTarget.transform.position
    }


    void Update()
    {
#if UNITY_EDITOR
        if (cmd_ContinuousRefresh)
        {
            Clean();
            Build();
            return;
        }
        if (cmd_Clean)
        {
            Clean();
            cmd_Clean = false;
        }
        if (cmd_Build)
        {
            Build();
            cmd_Build = false;
        }
#endif
    }
}
