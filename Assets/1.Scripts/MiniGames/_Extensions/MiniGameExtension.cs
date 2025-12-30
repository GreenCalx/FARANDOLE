using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class MiniGameExtension
{
    public abstract EMiniGameMods Tag { get; }
}