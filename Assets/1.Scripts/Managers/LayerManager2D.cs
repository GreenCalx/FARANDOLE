using UnityEngine;
using System.Collections.Generic;

public class LayerManager2D : MonoBehaviour, IManager
{
    public List<List<Transform>> layers;
    public int MaxDepth = 3;
    #region IManager
    public void Init(GameManager iGameManager)
    {
        layers = new List<List<Transform>>(MaxDepth);
        for (int i = 0; i <= MaxDepth; i++)
        {
            layers.Add(new List<Transform>());
        }
    }
    public bool IsReady()
    {
        return (layers != null) && (layers.Count == 1 + MaxDepth);
    }
    #endregion

    public void ClearLayers()
    {
        foreach (List<Transform> layer in layers)
        {
            layer.Clear();
        }
    }
    public void Place(Transform iT, int iDepth)
    {
        if (layers[iDepth].Contains(iT))
            return;
        iT.position = new Vector3(iT.position.x, iT.position.y, iDepth);
        layers[iDepth].Add(iT);
    }

    public void PlaceTop( Transform iT)
    {
        Place(iT,0);
    }

    public void PlaceBot(Transform iT)
    {
        Place(iT,MaxDepth - 1);
    }

    public void PlaceAtSame(Transform iToPlace, Transform iRelative)
    {
        int relative_depth = SeekTransformLayer(iRelative);
        if (relative_depth < 0)
            return; // failed to find iRelative
        Place(iToPlace, relative_depth);
    }

    public void PlaceAbove(Transform iToPlace, Transform iRelative)
    {
        int relative_depth = SeekTransformLayer(iRelative);
        if (relative_depth < 0)
            return;  // Failed to find iRelative

        if (relative_depth == 0)
            PlaceTop(iToPlace); // can't go above top
        else
            Place(iToPlace, relative_depth - 1);
    }

    public void PlaceUnder(Transform iToPlace, Transform iRelative)
    {
        int relative_depth = SeekTransformLayer(iRelative);
        if (relative_depth < 0)
            return; // Failed to find iRelative

        if (relative_depth == MaxDepth - 1)
            PlaceBot(iToPlace); // can't go under bot
        else
            Place(iToPlace, relative_depth + 1);
    }


    int SeekTransformLayer(Transform iT)
    {
        for (int i = 0; i < MaxDepth; i++)
        {
            if (layers[i].Contains(iT))
                return i;
        }
        return -1;
    }
}
