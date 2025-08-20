using UnityEngine;
using System.Collections.Generic;

public class LayerManager2D : MonoBehaviour, IManager
{
    internal class Layer
    {
        public Layer(int iMin, int iMax)
        {
            min = iMin;
            max = iMax;
            size = Mathf.Abs(Mathf.Abs(iMax) - Mathf.Abs(iMin));
            renderers = new Renderer[size];
            nextSlot = 1;
        }
        public Renderer[] renderers;
        public int min, max;
        public int size;
        public int nextSlot;

        public void Clear()
        {
            int i = 1;
            while (renderers[i] != null)
            {
                renderers[i] = null;
                i++;
            }
            nextSlot = 1;
        }
        public void PlaceRoot(Renderer iRenderer)
        {
            if (renderers[0] != null)
            {
                Debug.LogWarning("Replacing Root in LayerManager2D. Should not happen.");
            }
            iRenderer.sortingOrder = min;
            renderers[0] = iRenderer;
        }
        public void PlaceNew(Renderer iRenderer)
        {
            iRenderer.sortingOrder = min + nextSlot;
            renderers[nextSlot] = iRenderer;
            nextSlot++;
        }
    }
    Layer forgroundLayer;
    Layer backgroundLayer;
    Layer objectLayer;
    public int layerSize = 500;
    
    #region IManager
    public void Init(GameManager iGameManager)
    {
        // TODO : Everyone in the same native array for contiguity ?
        backgroundLayer = new Layer(-layerSize, 0);
        objectLayer = new Layer(0, layerSize);
        forgroundLayer = new Layer(layerSize, layerSize * 2);
    }
    public bool IsReady()
    {
        return (forgroundLayer != null) && (backgroundLayer != null) && (objectLayer!=null);
    }
    #endregion

    public void ClearLayers()
    {
        objectLayer.Clear();
        backgroundLayer.Clear();
        forgroundLayer.Clear();
    }
    public void PlaceObject(Renderer iRenderer)
    {
        objectLayer.PlaceNew(iRenderer);
    }

    public void PlaceForgroundRoot(Renderer iRenderer)
    {
        forgroundLayer.PlaceRoot(iRenderer);
    }

    public void PlaceBackgroundRoot(Renderer iRenderer)
    {
        backgroundLayer.PlaceRoot(iRenderer);
    }

    public void PlaceForground(Renderer iRenderer)
    {
        forgroundLayer.PlaceNew(iRenderer);
    }

    public void PlaceBackground(Renderer iRenderer)
    {
        backgroundLayer.PlaceNew(iRenderer);
    }

    // public void PlaceAtSame(Transform iToPlace, Transform iRelative)
    // {
    //     int relative_depth = SeekTransformLayer(iRelative);
    //     if (relative_depth < 0)
    //         return; // failed to find iRelative
    //     Place(iToPlace, relative_depth);
    // }

    // public void PlaceAbove(Transform iToPlace, Transform iRelative)
    // {
    //     int relative_depth = SeekTransformLayer(iRelative);
    //     if (relative_depth < 0)
    //         return;  // Failed to find iRelative

    //     if (relative_depth == 0)
    //         PlaceTop(iToPlace); // can't go above top
    //     else
    //         Place(iToPlace, relative_depth - 1);
    // }

    // public void PlaceUnder(Transform iToPlace, Transform iRelative)
    // {
    //     int relative_depth = SeekTransformLayer(iRelative);
    //     if (relative_depth < 0)
    //         return; // Failed to find iRelative

    //     if (relative_depth == MaxDepth - 1)
    //         PlaceBot(iToPlace); // can't go under bot
    //     else
    //         Place(iToPlace, relative_depth + 1);
    // }


    // int SeekTransformLayer(Transform iT)
    // {
    //     for (int i = 0; i < MaxDepth; i++)
    //     {
    //         if (layers[i].Contains(iT))
    //             return i;
    //     }
    //     return -1;
    // }
}
