using UnityEngine;
using System.Collections.Generic;

public class LayerManager2D : MonoBehaviour, IManager
{
    internal class Layer
    {
        public Layer(int iMin, int iMax, bool iZDisplace, int iReservedSize = 5)
        {
            reservedSize = iReservedSize;
            min = iMin + iReservedSize;
            max = iMax;
            size = Mathf.Abs(Mathf.Abs(iMax) - Mathf.Abs(iMin));
            renderers = new Renderer[size];
            nextSlot = reservedSize + 1;
            zDisplace = iZDisplace;
            zStep = 1f / size;
        }
        public Renderer[] renderers;
        public int reservedSize;
        public int min, max;
        public int size;
        public int nextSlot;
        public bool zDisplace;
        public float zStep = 0.02f;
        public void Clear()
        {
            int i = reservedSize + 1;
            while (renderers[i] != null)
            {
                renderers[i] = null;
                i++;
            }
            nextSlot = reservedSize + 1;
        }

        public void PlaceInReserve(Renderer iRenderer)
        {
            for (int i = 0; i < reservedSize; i++)
            {
                if (renderers[i] == null)
                {
                    iRenderer.sortingOrder = min + i;
                    renderers[i] = iRenderer;
                    TryUpdatePosition(i);
                    //Debug.Log("LM2D Reserved : " + renderers[i].gameObject.name + " sorting order : " + renderers[i].sortingOrder);
                    return;
                }
            }
            //Debug.LogWarning("Failed to place in reserve");
        }

        public int PlaceNew(Renderer iRenderer)
        {
            iRenderer.sortingOrder = min + nextSlot;
            renderers[nextSlot] = iRenderer;
            TryUpdatePosition(nextSlot);

            nextSlot++;
            return reservedSize + nextSlot - 1;
        }

        public void ReplaceExisting(int iIndex, Renderer iNewRend)
        {
            renderers[iIndex] = iNewRend;
        }

        public void TryUpdatePosition(int iIndex)
        {
            if (!zDisplace)
                return;
            Renderer rend = renderers[iIndex];
            rend.transform.position = new Vector3(rend.transform.position.x, rend.transform.position.y, -zStep * rend.sortingOrder);
        }
    }
    Layer forgroundLayer;
    Layer backgroundLayer;
    Layer objectLayer;
    public int layerSize = 500;
    public bool zDisplace = true;

    #region IManager
    public void Init(GameManager iGameManager)
    {
        // TODO : Everyone in the same native array for contiguity ?
        backgroundLayer = new Layer(-layerSize, 0, zDisplace);
        objectLayer = new Layer(0, layerSize, zDisplace);
        forgroundLayer = new Layer(layerSize, layerSize * 2, zDisplace);
    }
    public bool IsReady()
    {
        return (forgroundLayer != null) && (backgroundLayer != null) && (objectLayer != null);
    }
    #endregion

    public void ClearLayers()
    {
        objectLayer.Clear();
        backgroundLayer.Clear();
        forgroundLayer.Clear();
    }
    public int PlaceObject(Renderer iRenderer)
    {
        return objectLayer.PlaceNew(iRenderer);
    }

    public void ReplaceObject(int iIndex, Renderer iNewRenderer)
    {
        objectLayer.ReplaceExisting(iIndex, iNewRenderer);
    }

    public void PlaceForgroundReserve(Renderer iRenderer)
    {
        forgroundLayer.PlaceInReserve(iRenderer);
    }

    public void PlaceBackgroundReserve(Renderer iRenderer)
    {
        backgroundLayer.PlaceInReserve(iRenderer);
    }

    public void PlaceForground(Renderer iRenderer)
    {
        forgroundLayer.PlaceNew(iRenderer);
    }

    public void PlaceBackground(Renderer iRenderer)
    {
        backgroundLayer.PlaceNew(iRenderer);
    }

    public void FullRefresh()
    {

    }
}
