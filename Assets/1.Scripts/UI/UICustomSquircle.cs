using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class UICustomSquircle : UISquircle
{
    public bool ForceDrawing = false;
    void Update()
    {
        if (ForceDrawing)
        {
            material.SetColor("_Color", color);
            //SetAllDirty();
        }
    }
}
