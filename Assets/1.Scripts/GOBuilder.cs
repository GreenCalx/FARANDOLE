using UnityEngine;
using System.Collections.Generic;
using static Utils;

public class GOBuilder
{
    protected GameObject GO;
    private GOBuilder() { GO = new GameObject(); }
    private GOBuilder(GameObject iPrefab) { GO = GameObject.Instantiate(iPrefab); }

    public static GOBuilder Create() => new GOBuilder();
    public static GOBuilder Create(GameObject iPrefab) => new GOBuilder(iPrefab);

    public GameObject Build() => GO;

    public GOBuilder WithName(string iName)
    {
        GO.name = iName;
        return this;
    }

    public GOBuilder WithPosition(Vector2 iPos)
    {
        GO.transform.position = iPos;
        return this;
    }

    public GOBuilder WithPosition(Vector3 iPos)
    {
        GO.transform.position = iPos;
        return this;
    }

    public GOBuilder WithLocalPosition(Vector2 iPos)
    {
        GO.transform.localPosition = iPos;
        return this;
    }

    public GOBuilder WithLocalPosition(Vector3 iPos)
    {
        GO.transform.localPosition = iPos;
        return this;
    }

    public GOBuilder WithParent(Transform iParent)
    {
        GO.transform.parent = iParent;
        return this;
    }

    public GOBuilder WithRB2D(RigidbodyType2D iRBType)
    {
        Rigidbody2D rb2d = GO.AddComponent<Rigidbody2D>();
        rb2d.bodyType = iRBType;
        return this;
    }

    public GOBuilder WithBoxCollider2D(Vector2 iOffset, Vector2 iSize, Collider2D.CompositeOperation iCompOp = Collider2D.CompositeOperation.None)
    {
        BoxCollider2D bc2d = GO.AddComponent<BoxCollider2D>();
        bc2d.size = iSize;
        bc2d.offset = iOffset;
        bc2d.compositeOperation = iCompOp;
        return this;
    }

    public GOBuilder WithBoxCollider2DAndMesh(Vector2 iOffset, Vector2 iSize, out Mesh oMesh, Collider2D.CompositeOperation iCompOp = Collider2D.CompositeOperation.None)
    {
        BoxCollider2D bc2d = GO.AddComponent<BoxCollider2D>();
        bc2d.size = iSize;
        bc2d.offset = iOffset;
        bc2d.compositeOperation = iCompOp;
        oMesh = bc2d.CreateMesh(true, true);
        return this;
    }

    public GOBuilder WithMeshFilter(Mesh iMesh, bool iUnwrap = true)
    {
        if (iUnwrap)
            UnwrapMesh(iMesh);
        MeshFilter mf = GO.AddComponent<MeshFilter>();
        mf.mesh = iMesh;
        return this;
    }

    public GOBuilder WithRenderer(Material iMat)
    {
        MeshRenderer mr = GO.AddComponent<MeshRenderer>();
        mr.material = iMat;
        return this;
    }

    public GOBuilder WithCompositeCollider2D(bool iGenerateGeom = true)
    {
        CompositeCollider2D cc2d = GO.AddComponent<CompositeCollider2D>();
        if (iGenerateGeom)
            cc2d.GenerateGeometry();
        return this;
    }

    public GOBuilder WithLineRenderer(Material iMat)
    {
        LineRenderer LR = GO.AddComponent<LineRenderer>();
        LR.material = iMat;
        return this;
    }

}
