using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Plugins.Animate_UI_Materials;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(GraphicMaterialOverride))]
public class UILRColorToShader : MonoBehaviour
{
    public UILineRenderer target;
    public bool continuousRefresh = true;
    public Color currentColor;
    Color initColor;
    GraphicPropertyOverrideColor matColorOverride;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initColor = target.color;
        currentColor = initColor;

        matColorOverride = GetComponentInChildren<GraphicPropertyOverrideColor>();
        matColorOverride.PropertyValue = initColor;
    }

    void OnDisable()
    {
        if (target == null)
            return;
        matColorOverride.PropertyValue = initColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (continuousRefresh)
        {
            matColorOverride.PropertyValue = target.color;
            currentColor = target.color;
        }
    }
}
