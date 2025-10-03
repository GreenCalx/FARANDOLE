using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using static Utils;
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
}

public class RoomRow
{
    List<Transform> objects;
    public int rowDepth;
    public Vector2 min, max;
    public Rect area;
    public LineRenderer lineTracer;
    public RoomRow(Rect iArea, int iDepth)
    {
        min = iArea.min;
        max = iArea.max;
        area = iArea;
        rowDepth = iDepth;

        objects = new List<Transform>();
    }

    public void AddToRow(Transform iTransform)
    {
        if (!objects.Contains(iTransform))
            objects.Add(iTransform);
    }

    public bool Remove(Transform iTransform)
    {
        if (objects.Contains(iTransform))
        {
            objects.Remove(iTransform);
            return true;
        }
        return false;
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


    [Header("Tweaks")]
    public int m_Depth = 3;
    public RoomPlan m_ForwardPlan;
    public RoomPlan m_FarPlan;

    public bool m_TraceLines = true;
    public Material m_LineTracerMat;
    public Material m_RowTracerMat;
    public float m_LineWidth = 0.5f;
    public float m_LineWidthDepthFalloff = 2f;
    public string m_TracesSortingLayer = "PerspectiveRoom";
    [Header("Internal View")]
    public float m_FOV = 90; // auto
    public LayerManager2D r_LM2D;
    List<RoomRow> m_Rows;
    float m_FarLineWidth = 0;

    LineRenderer farPlaneLineTracer;
    LineRenderer forwardPlaneLineTracer;
    List<LineRenderer> perspectiveLines;

    public void Build()
    {
        if (!m_TraceLines)
        {
            // its dirty but quick to do the work for now
            // TODO : build line tracers only when required properly
            m_LineWidth = 0f;
        }

        // ----------------------------------------------------
        // Invariants compute
        m_FarLineWidth = m_LineWidth / m_LineWidthDepthFalloff;

        // ----------------------------------------------------
        // Trace planes
        BuildPlan("ForwardPlan", m_ForwardPlan, m_LineWidth, ref forwardPlaneLineTracer);
        BuildPlan("FarPlan", m_FarPlan, m_FarLineWidth, ref farPlaneLineTracer);
        // Deduce FOV from plans
        Vector3 relative = m_FarPlan.area.min - m_ForwardPlan.area.min;
        m_FOV = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;


        // ----------------------------------------------------
        // Trace Perspective Lines
        perspectiveLines = new List<LineRenderer>(4);
        perspectiveLines.Add(
            BuildLine(
                "min -> min",
                new Vector3(m_ForwardPlan.min.x, m_ForwardPlan.min.y, m_ForwardPlan.zDepth),
                new Vector3(m_FarPlan.min.x, m_FarPlan.min.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );
        perspectiveLines.Add(
            BuildLine(
                "max -> max",
                new Vector3(m_ForwardPlan.max.x, m_ForwardPlan.max.y, m_ForwardPlan.zDepth),
                new Vector3(m_FarPlan.max.x, m_FarPlan.max.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );
        perspectiveLines.Add(
            BuildLine(
                "min -> max",
                new Vector3(m_ForwardPlan.min.x, m_ForwardPlan.max.y, m_ForwardPlan.zDepth),
                new Vector3(m_FarPlan.min.x, m_FarPlan.max.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );
        perspectiveLines.Add(
            BuildLine(
                "max -> min",
                new Vector3(m_ForwardPlan.max.x, m_ForwardPlan.min.y, m_ForwardPlan.zDepth),
                new Vector3(m_FarPlan.max.x, m_FarPlan.min.y, m_FarPlan.zDepth),
                m_LineWidth,
                m_FarLineWidth)
        );


        // ----------------------------------------------------
        // Init rows
        m_Rows = new List<RoomRow>(m_Depth);
        for (int i = 0; i <= m_Depth; i++)
        {
            float frac = (float)i / m_Depth;
            string name = "Row " + i;
            Rect rowArea = new Rect(
                Utils.Lerp(m_ForwardPlan.min.x, m_FarPlan.min.x, frac),
                Utils.Lerp(m_ForwardPlan.min.y, m_FarPlan.min.y, frac),
                Utils.Lerp(m_ForwardPlan.width, m_FarPlan.width, frac),
                Utils.Lerp(m_ForwardPlan.height, m_FarPlan.height, frac)
            );
            float lineWidth = Utils.Lerp(m_LineWidth, m_FarLineWidth, frac);
            float zDepth = Utils.Lerp(m_ForwardPlan.zDepth, m_FarPlan.zDepth, frac);
            m_Rows.Add(BuildRow(name, rowArea, lineWidth, zDepth));
        }


    }

    public RoomRow BuildRow(string iName, Rect iArea, float iLineWidth, float iDepth)
    {
        RoomRow ret = new RoomRow(iArea, iDepth);

        ret.lineTracer = GOBuilder.Create()
                        .WithName(iName)
                        .WithParent(transform)
                        .WithPosition(iArea.center)
                        .WithLineRenderer(m_RowTracerMat)
                        .BuildAs<LineRenderer>();

        Vector3[] pos = new Vector3[4];
        pos[0] = new Vector3(iArea.max.x, iArea.max.y, iDepth);
        pos[1] = new Vector3(iArea.max.x, iArea.min.y, iDepth);
        pos[2] = new Vector3(iArea.min.x, iArea.min.y, iDepth);
        pos[3] = new Vector3(iArea.min.x, iArea.max.y, iDepth);

        ret.lineTracer.positionCount = 4;
        ret.lineTracer.loop = true;
        ret.lineTracer.startWidth = iLineWidth;
        ret.lineTracer.endWidth = iLineWidth;
        ret.lineTracer.SetPositions(pos);
        ret.lineTracer.sortingOrder = 1;
        ret.lineTracer.sortingLayerName = m_TracesSortingLayer;
        ret.lineTracer.rendererPriority = 8;

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
        line.rendererPriority = 9;

        return line;

    }

    // --------------------------------------------
    // Accessors

    public RoomRow GetRowAt(int iDepth)
    {
        int rowDepth = Mathf.Clamp(iRowDepth, 0, m_Depth);
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

        foreach (Transform t in transform)
        {
            DestroyImmediate(t.gameObject);
        }

        // if (farPlaneLineTracer != null)
        //     DestroyImmediate(farPlaneLineTracer.gameObject);
        // if (forwardPlaneLineTracer != null)
        //     DestroyImmediate(forwardPlaneLineTracer.gameObject);
        // foreach (LineRenderer lr in perspectiveLines)
        // {
        //     if (lr == null)
        //         continue;
        //     DestroyImmediate(lr.gameObject);
        // }
        // perspectiveLines.Clear();
        // if (m_Rows != null)
        // {
        //     foreach (RoomRow rr in m_Rows)
        //     {
        //         if (rr==null)
        //             continue;
        //         DestroyImmediate(rr.gameObject);
        //     }
        //     m_Rows.Clear();
        // }

#endif
    }

    // --------------------------------------------
    // Public API

    public void AddToRoom(Transform iCaller, int iRowDepth)
    {
        RoomRow targetRow = GetRowAt(iRowDepth);
        if (targetRow == null)
        {
            Debug.Log("PerspectiveRoom::AddToRoom(" + iCaller / gameObject.name + ", " + iRowDepth + ")");
            return;
        }
        targetRow.AddObject(iCaller);
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


    void Update()
    {
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
    }
}
