using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class MiniGameExtension
{
    public abstract EMiniGameMods Tag { get; }
    public virtual void ResetMutation() {}
    public virtual void ApplyMutation(float  iMutChannelValue) {}
}