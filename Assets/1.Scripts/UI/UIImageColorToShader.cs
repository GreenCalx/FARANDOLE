using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Plugins.Animate_UI_Materials;

[RequireComponent(typeof(GraphicMaterialOverride))]
public class UIImageColorToShader : MonoBehaviour
{
    public Image target;
    public bool continuousRefresh = true;
    public Color currentColor;
    Color initColor;
    List<GraphicPropertyOverrideColor> matColorOverrides;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initColor = target.color;
        currentColor = initColor;

        matColorOverrides = new List<GraphicPropertyOverrideColor>(GetComponentsInChildren<GraphicPropertyOverrideColor>());
        ResetColors();
    }

    public void ResetColors()
    {
        foreach ( var c in matColorOverrides)
        {
            c.PropertyValue = initColor;
        }
    }

    void OnDisable()
    {
        if (target == null)
            return;
        ResetColors();
    }

    // Update is called once per frame
    void Update()
    {
        if (continuousRefresh)
        {
            foreach (var c in matColorOverrides)
            {
                c.PropertyValue = target.color;
            }
            currentColor = target.color;
        }
    }
}
