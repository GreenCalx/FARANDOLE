using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIVisualSlider : MonoBehaviour
{
    public Transform ToSlide;
    public Transform LeftPoint;
    public Transform RightPoint;

    Vector3 centroid;

    void Start()
    {
        UpdateVisual(0f);
    }
    public void UpdateVisual(float iValue)
    {
        float remaped = Utils.Remap(iValue, -1f, 1f, 0f, 1f);
        ToSlide.position = Vector3.Lerp(LeftPoint.position, RightPoint.position, remaped);
    }
}
